using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using DO;
using BO;

namespace BL
{/// <summary>
/// part of BL class
/// </summary>
    partial class BLImp : IBL

    {
        /// <summary>
        /// Update that parcel was delivered
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDeliverParcel(int id)
        {//if drone not found
            if (!drones.Any(dr => dr.Id == id))
                throw new GetException($"ID OF DRONE {id} DOESN'T EXIST\n");
            DroneToList dr = drones.Find(dr => dr.Id == id);
            DO.Parcel? parcelConnected = Dal.GetParcelList(pc => pc.DroneId == id && pc.PickedUp <= DateTime.Now && pc.Delivered == null).FirstOrDefault();
            //find all parcels that are connected to this drone

            //if any of the drone's parcels is picked up but not delivered
            if (parcelConnected != null && parcelConnected.Value.Id != 0)
            {
                DO.Parcel p = (DO.Parcel)parcelConnected;
                p.Delivered = DateTime.Now;
                DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                //if the drone can deliver it taking in account his battery status
                //then update drone and parcel
                if (dr.Battery - Dal.DroneElectricConsumations()[(int)p.Weight + 1] * (BO.LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude })) >= 0)
                    dr.Battery -= Dal.DroneElectricConsumations()[(int)p.Weight + 1] * (BO.LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }));
                else throw new BatteryException($"DRONE {dr.Id} HASNT ENOUGH BATTERY\n");

                Location l1 = new Location { Latitude = cs.Latitude, Longitude = cs.Longitude };
                dr.Location = l1;
                dr.Status = Enums.DroneStatuses.Vacant;
                dr.NumOfDeliveredParcel++;
                dr.DeliveryId = 0;

                drones[drones.FindIndex(dr => dr.Id == id)] = dr;
                try
                {
                    Dal.UpdateParcel(p);
                }
                catch (ParcelExeption d)
                {
                    throw new GetException($"Id of parcel {p.Id} not found", d);
                }
            }
        }
        /// <summary>
        /// put the parcel in the drone
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneToPickUpAParcel(int id)
        {//if drone not found
            if (!drones.Any(dr => dr.Id == id))
                throw new GetException("$ID OF DRONE {dr.Id} DOESN'T EXIST\n");
            DroneToList dr = drones.Find(dr => dr.Id == id);
            List<DO.Parcel> parcelConnected = (List<DO.Parcel>)Dal.GetParcelList(pc => pc.DroneId == id);


            int index = parcelConnected.FindIndex(pc => pc.Scheduled < DateTime.Now && pc.PickedUp == null);
            //if any parcel has to be delivered
            if (index != -1)
            {
                DO.Parcel p = parcelConnected[index];
                DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                //if hte drone has enough battery to deliver the parcel
                //then make the wanted changes
                if (dr.Battery >= Dal.DroneElectricConsumations()[0] *
                    BO.LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }))
                {
                    dr.Status = Enums.DroneStatuses.InDelivery;
                    p.PickedUp = DateTime.Now;
                    dr.Battery -= Dal.DroneElectricConsumations()[0] *
                         BO.LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude });
                    dr.Location.Latitude = cs.Latitude;
                    dr.Location.Longitude = cs.Longitude;
                    drones[drones.FindIndex(dr => dr.Id == id)] = dr;
                    Dal.UpdateParcel(p);
                }
                else throw new BatteryException($"DRONE { dr.Id } DOESN'T HAVE ENOUGH BATTERY\n");
            }
        }
        /// <summary>
        /// assign a parcel to a drone
        /// </summary>
        /// <param name="idC"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateAssignParcelToDrone(int idC)
        {
            try
            {
                //if drone not found
                if (!drones.Any(dr => dr.Id == idC))
                    throw new GetException($"ID OF DRONE {idC} DOESN'T EXIST\n");
                DroneToList drone = drones.Find(d => d.Id == idC);
                if (drone.Status != Enums.DroneStatuses.Vacant)
                    throw new BadStatusException("Drone is not available for delivery");
                //find the heaviest parcel the drone can take
                var parcelId = nextParcel(drone);
                if (parcelId == null)
                    throw new BadStatusException("Can't do it - battery is too low, please send to charge");


                //if found a parcel that the drone can carry, update wanted states
                drone.DeliveryId = parcelId;
                drone.Status = Enums.DroneStatuses.InDelivery;
                Dal.UpdateParcelToDrone((int)parcelId, drone.Id);
            }
            catch (DO.ParcelExeption ex)
            {
                throw new GetException("Problem with parcel scheduling", ex);
            }
        }

        /// <summary>
        /// function to release a drone from charging
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="duration"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateReleaseDroneFromCharge(int idC, TimeSpan duration)
        {
            if (!drones.Any(dr => dr.Id == idC))
                throw new GetException($"ID OF DRONE {idC} DOESN'T EXIST");
            if (drones[drones.FindIndex(dr => dr.Id == idC)].Status != Enums.DroneStatuses.Maintenance)
                throw new GetException($"THE DRONE {idC} ISN'T IN MAINTENANCE");
            if ((drones[drones.FindIndex(dr => dr.Id == idC)].Battery + ((duration.TotalSeconds * 1 / 3600) + (duration.TotalMinutes * 1 / 60) + (duration.TotalHours)) * Dal.DroneElectricConsumations()[4]) > 100)
            {
                drones[drones.FindIndex(dr => dr.Id == idC)].Battery = 100;
            }
            else
                drones[drones.FindIndex(dr => dr.Id == idC)].Battery += ((duration.TotalSeconds * 1 / 3600) + (duration.TotalMinutes * 1 / 60) + (duration.TotalHours)) * Dal.DroneElectricConsumations()[4];
            drones[drones.FindIndex(dr => dr.Id == idC)].Status = Enums.DroneStatuses.Vacant;
            Dal.DeleteDroneCharge(idC);
        }
        /// <summary>
        /// Send a drone to charge
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneSentToCharge(int id)
        {//if drone not found
            if (!Dal.GetDroneList(dr => dr.Id == id).Any())
            {
                throw new GetException($"id {id} doesn't exist ");
            }
            DroneToList drone = drones.Find(dr => dr.Id == id);
            // if the drown isn't vacant it can't be charged.
            if (drone.Status != Enums.DroneStatuses.Vacant)
            {
                return;
            }
            BO.BaseStation bc = getClosestBase(drone.Location);
            DO.BaseStation myBase = Dal.GetBaseStationsList(bas => bas.Latitude == bc.Location.Latitude && bas.Longitude == bc.Location.Longitude).FirstOrDefault();
            if ((Dal.DroneElectricConsumations()[0]) *
               bc.Distances(drone) <= drone.Battery)
            {
                drone.Battery -= (Dal.DroneElectricConsumations()[0]) *
                BO.LocationFuncs.Distance(drone.Location, new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude });
                drone.Location = new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude };
                drone.Status = Enums.DroneStatuses.Maintenance;
                drones[drones.FindIndex(dr => dr.Id == drone.Id)] = drone;
                Dal.AddDroneCharge(drone.Id, bc.Id);
            }
            else throw new BatteryException($"The drone {drone.Id} doesn't have enough battery to get to the close station.");
        }
        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerInfo(int idC, string name, string phone)
        {//if customer not found
            if (!Dal.GetCustomerList().Any(cu => cu.Id == idC))
            {
                throw new GetException($"id {idC} doesn't exist ");
            }
            //do the wanted changes

            DO.Customer bs = Dal.GetCustomerList().FirstOrDefault(cus => cus.Id == idC);
            if (name != "")
                bs.Name = name;
            if (phone != "")
                bs.Phone = phone;
            Dal.UpdateCustomerInfoFromBL(bs);
        }
        /// <summary>
        /// Update baseStatiom numof Slots or name
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="numOfSlots"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateBaseStation(int myId, int numOfSlots, string name)
        {//if BaseStation not found
            if (!Dal.GetBaseStationsList(null).Any(bs => bs.Id == myId))
            {
                throw new GetException($"id {myId} doesn't exist ");
            }
            DO.BaseStation bs = Dal.GetBaseStationsList(null).FirstOrDefault(bs => bs.Id == myId);
            if (numOfSlots != 0)
                bs.NumOfSlots = numOfSlots;
            if (name != "Base ")
                bs.Name = name;
            try
            {
                Dal.UpdateBaseStationFromBl(bs);
            }
            catch (BaseExeption p)
            {
                throw new GetException($"The Base station {bs.Id} doesn't exist", p);
            }

        }
        /// <summary>
        /// Updae name to drone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateNameDrone(int id, BO.Enums.DroneNames name)
        {//if drone not found
            if (!Dal.GetDroneList().Any(dr => dr.Id == id))
            {
                throw new GetException($"id {id} doesn't exist ");
            }
            //Update wanted rows

            DO.Drone dr = Dal.GetDroneList().FirstOrDefault(dr => dr.Id == id);
            dr.Model = (DroneNames)name;
            try
            {
                Dal.UpdateDrone(dr);
            }
            catch (DO.DroneException p)
            {
                throw new GetException($"The Drone {dr.Id} doesn't exist", p);
            }
            drones[drones.FindIndex(dr => dr.Id == id)].Model = name;
        }
        internal int? nextParcel(DroneToList drone)
        {
            lock (Dal)
            {
                return Dal.GetParcelList(p => p.Scheduled == null
                                           && (Enums.WeightCategories)(p.Weight) <= drone.MaxWeight
                                           && drone.RequiredBattery(this, (int)p.Id) < drone.Battery)
                          .OrderByDescending(p => p.Priority)
                          .ThenByDescending(p => p.Weight)
                          .FirstOrDefault().Id;
            }
        }
    }
}
