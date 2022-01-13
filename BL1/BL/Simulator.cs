using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using BO;
using System.Runtime.CompilerServices;
using System.Threading;
using static BL.BLImp;


namespace BL
{
    class Simulator
    {
        enum Maintenance { Starting, Going, Charging }
        private const double VELOCITY =0.5;
        private const int DELAY = 500;
        private const double TIME_STEP = DELAY / 1000.0;
        private const double STEP = VELOCITY / TIME_STEP;

        public Simulator(BLImp bLImp, int droneId, Action updateDrone, Func<bool> checkStop)
        {

            var bl = bLImp;
            var dal = bl.Dal;
            var drone = bl.GetDrone(droneId);
            int? parcelId = null;
            int? baseStationId = null;
            BaseStation bs = null;
            double distance = 0.0;
            int batteryUsage = 0;
            DO.Parcel? parcel = null;
            bool pickedUp = false;
            Customer customer = null;
            Maintenance maintenance = Maintenance.Starting;

            void initDelivery(int id)
            {
                parcel = dal.GetParcel(id);
                ///get use of battery
                batteryUsage = (int)Enum.Parse(typeof(Enums.BatteryUsage), parcel?.Weight.ToString());
                pickedUp = parcel?.PickedUp is not null;
                customer = bl.GetCustomer((int)(pickedUp ? parcel?.TargetId : parcel?.SenderId));
            }

            do
            {
                //(var next, var id) = drone.nextAction(bl);

                switch (drone)
                {
                    case Drone { Status: Enums.DroneStatuses.Vacant }:
                        //if false it means the worker is off!
                        if (!sleepDelayTime()) break;

                        lock (bl) lock (dal)
                            {//next parcel
                                parcelId = bl.Dal.GetParcelList(p => p.Scheduled == null
                                                                  && (Enums.WeightCategories)(p.Weight) <= drone.MaxWeight
                                                                  && drone.RequiredBattery(bl, p.Id) < drone.Battery)
                                                 .OrderByDescending(p => p.Priority)
                                                 .ThenByDescending(p => p.Weight)
                                                 .FirstOrDefault().Id;
                                switch (parcelId, drone.Battery)
                                {
                                    case (0, 100):
                                        break;

                                    case (0, _):
                                        baseStationId = bl.FindClosestBaseStation(drone)?.Id;
                                        if (baseStationId != null)
                                        {
                                            drone.Status = Enums.DroneStatuses.Maintenance;
                                            maintenance = Maintenance.Starting;
                                            dal.AddDroneCharge(droneId, (int)baseStationId);
                                        }
                                        break;
                                    case (_, _):
                                        try
                                        {
                                            dal.ParcelSchedule((int)parcelId, droneId);
                                            drone.DeliveryId = (int)parcelId;
                                            initDelivery((int)parcelId);
                                            drone.Status = Enums.DroneStatuses.InDelivery;
                                        }
                                        catch (DO.ParcelExeption ex) { throw new GetException("Internal error getting parcel", ex); }
                                        break;
                                }
                            }
                        break;

                    case Drone { Status: Enums.DroneStatuses.Maintenance }:
                        switch (maintenance)
                        {
                            case Maintenance.Starting:
                                lock (bl) lock (dal)
                                    {
                                        try { bs = bl.GetBaseStation(baseStationId ?? dal.GetDroneChargeBaseStationId(drone.Id)); }
                                        catch (DO.ParcelExeption ex) { throw new GetException("Internal error getting parcel", ex); }
                                        distance = drone.Distances(bs);
                                        maintenance = Maintenance.Going;
                                    }
                                break;

                            case Maintenance.Going:
                                if (distance < 0.01 || drone.Battery == 0.0)
                                    lock (bl)
                                    {
                                        drone.Location = bs.Location;
                                        maintenance = Maintenance.Charging;
                                    }
                                else
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl)
                                    {
                                        double delta = distance < STEP ? distance : STEP;
                                        distance -= delta;
                                        drone.Battery = Max(0.0, drone.Battery - delta * bl.BatteryUsages[DRONE_FREE]);
                                    }
                                }
                                break;

                            case Maintenance.Charging:
                                if (drone.Battery == 100)
                                    lock (bl) lock (dal)
                                        {
                                            drone.Status = Enums.DroneStatuses.Vacant;
                                            dal.DeleteDroneCharge(droneId);

                                        }
                                else
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl) drone.Battery = Min(100, drone.Battery + bl.BatteryUsages[DRONE_CHARGE] * TIME_STEP);
                                }
                                break;
                            default:
                                throw new GetException("Internal error: wrong maintenance substate");
                        }
                        break;

                    case Drone { Status: Enums.DroneStatuses.InDelivery }:
                        lock (bl) lock (dal)
                            {
                                try
                                {
                                    if (parcelId == null) initDelivery((int)drone.DeliveryId);
                                }
                                catch (DO.ParcelExeption ex)
                                {
                                    throw new GetException("Internal error getting parcel", ex);
                                }
                                distance = drone.Distances(customer);
                            }

                        if (distance < 0.01 || drone.Battery == 0)
                            lock (bl) lock (dal)
                                {
                                    drone.Location = customer.Location;
                                    if (pickedUp)
                                    {
                                        dal.ParcelDelivery((int)parcel?.Id);
                                        drone.Status = Enums.DroneStatuses.Vacant;
                                    }
                                    else
                                    {
                                        dal.ParcelPickup((int)parcel?.Id);
                                        customer = bl.GetCustomer((int)parcel?.TargetId);
                                        pickedUp = true;
                                    }
                                }
                        else
                        {
                            if (!sleepDelayTime()) break;
                            lock (bl)
                            {
                                double delta = distance < STEP ? distance : STEP;
                                double proportion = delta / distance;
                                drone.Battery = Max(0.0, drone.Battery - delta * bl.BatteryUsages[pickedUp ? batteryUsage : DRONE_FREE]);
                                double lat = drone.Location.Latitude + (customer.Location.Latitude - drone.Location.Latitude) * proportion;
                                double lon = drone.Location.Longitude + (customer.Location.Longitude - drone.Location.Longitude) * proportion;
                                drone.Location = new() { Latitude = lat, Longitude = lon };
                            }
                        }
                        break;

                    default:
                        throw new GetException("Internal error: not available after Delivery...");

                }
              DroneToList d = bl.drones.Find(dr => dr.Id == drone.Id);
                d.Location = drone.Location;
                d.Status = drone.Status;
                d.Battery = drone.Battery;

                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].Location = drone.Location;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)] .Status=drone.Status;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].Battery = drone.Battery;
                updateDrone();
            } while (!checkStop());
        }

        private static bool sleepDelayTime()
        {
            try
            {
                Thread.Sleep(DELAY);
            }
            catch (ThreadInterruptedException)
            {
                return false;
            }
            return true;
        }
        private double Max(double a, double b) => a > b ? a : b;
        private double Min(double a, double b) => a < b ? a : b;
    }

}

