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
        private const double VELOCITY = 0.5;
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
                DroneToList d = bl.drones.Find(dr => dr.Id == drone.Id); //a temo unit for changes in UI
                switch (drone)
                {//incase drone vacant
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
                                {//if no parcel a drone fully charged///remains waiting
                                    case (0, 100):
                                        break;
                                    //if no parcel but need to be chargwed so go to base
                                    case (0, _):
                                        baseStationId = bl.FindClosestBaseStation(drone)?.Id;
                                        if (baseStationId != null)
                                        {
                                            drone.Status = Enums.DroneStatuses.Maintenance;
                                            maintenance = Maintenance.Starting;
                                            dal.AddDroneCharge(droneId, (int)baseStationId);
                                        }
                                        break;
                                    case (_, _):// if parcel found go to take it
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
                    //case drone in maintenace satus
                    case Drone { Status: Enums.DroneStatuses.Maintenance }:
                        switch (maintenance)
                        {//if he is not yet at the base///go further
                            case Maintenance.Starting:
                                lock (bl) lock (dal)
                                    {
                                        try { bs = bl.GetBaseStation(baseStationId ?? dal.GetDroneChargeBaseStationId(drone.Id)); }
                                        catch (DO.ParcelExeption ex) { throw new GetException("Internal error getting parcel", ex); }
                                        distance = drone.Distances(bs);
                                        maintenance = Maintenance.Going;
                                    }
                                break;
                            //carry on the drone's way
                            case Maintenance.Going://if got there
                                if (distance < 0.01 || drone.Battery == 0.0)
                                    lock (bl)
                                    {
                                        drone.Location = bs.Location;
                                        maintenance = Maintenance.Charging;
                                    }
                                else//get more charge
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
                            //if in charging
                            case Maintenance.Charging:
                                if (drone.Battery == 100)//if full release it
                                    lock (bl) lock (dal)
                                        {
                                            drone.Status = Enums.DroneStatuses.Vacant;
                                            dal.DeleteDroneCharge(droneId);

                                        }
                                else//get more baterry
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl) drone.Battery = Min(100, drone.Battery + bl.BatteryUsages[DRONE_CHARGE] * TIME_STEP);
                                }
                                break;
                            default:
                                throw new GetException("Internal error: wrong maintenance substate");
                        }
                        break;
                        //if drone in delivery status
                    case Drone { Status: Enums.DroneStatuses.InDelivery }:
                        lock (bl) lock (dal)
                            {
                                try
                                {
                                    if (parcelId == null) initDelivery((int)drone.DeliveryId);//if parcel not found//got o find
                                }
                                catch (DO.ParcelExeption ex)
                                {
                                    throw new GetException("Internal error getting parcel", ex);
                                }
                                distance = drone.Distances(customer);
                            }

                        if (distance < 0.01 || drone.Battery == 0)//on parcels location
                            lock (bl) lock (dal)
                                {
                                    drone.Location = customer.Location;
                                    if (pickedUp)//if the drone already picked up so is arrived to delivery
                                    {
                                        dal.ParcelDelivery((int)parcel?.Id);
                                        drone.Status = Enums.DroneStatuses.Vacant;
                                        d.NumOfDeliveredParcel++;
                                    }
                                    else//take the parcel to the next customer
                                    {
                                        dal.ParcelPickup((int)parcel?.Id);
                                        customer = bl.GetCustomer((int)parcel?.TargetId);
                                        pickedUp = true;
                                    }
                                }
                        else//if drone in the way..so go further
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
              
                d.Location = drone.Location;
                d.Status = drone.Status;
                d.Battery = drone.Battery;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].NumOfDeliveredParcel = d.NumOfDeliveredParcel;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].Location = drone.Location;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].Status = drone.Status;
                bl.drones[bl.drones.FindIndex(dr => dr.Id == drone.Id)].Battery = drone.Battery;
                updateDrone();
            } while (!checkStop());//while no stopped thread carry on loop 
        }
        /// <summary>
        /// func that put the thread on sleep for a short while
        /// </summary>
        /// <returns> if living thred</returns>
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
        /// <summary>
        /// calculate min
        /// </summary>
        /// <param name="a">param to compare</param>
        /// <param name="b">param to compare</param>
        /// <returns></returns>
        private double Max(double a, double b) => a > b ? a : b;
        /// <summary>
        /// calculate min
        /// </summary>
        /// <param name="a">param to compare</param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Min(double a, double b) => a < b ? a : b;
    }

}

