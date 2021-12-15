
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using DO;
using DLAPI;
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
        public void UpdateDeliverParcel(int id)
        {//if drone not found
            if (!drones.Any(dr => dr.Id == id))
                throw new GetException($"ID OF DRONE {id} DOESN'T EXIST\n");
            DroneToList dr = drones.Find(dr => dr.Id == id);
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList();
            //find all parcels that are connected to this drone
            List<DO.Parcel> parcelConnected = parcels.FindAll(pc => pc.DroneId == id);
            //if any of the drone's parcels is picked up but not delivered
            if (parcelConnected.Any(ps => ps.PickedUp <= DateTime.Now && ps.Delivered == DateTime.MinValue))
            {
               DO.Parcel p = parcelConnected.Find(ps => ps.PickedUp <= DateTime.Now && ps.Delivered == DateTime.MinValue);
                p.Delivered = DateTime.Now;
               DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                //if the drone can deliver it taking in account his battery status
                //then update drone and parcel
                if (dr.BatteryStatus - myDal.DroneElectricConsumations()[(int)p.Weight + 1] * (BO.LocationFuncs.Distance(dr.DroneLocation, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude })) >= 0)
                    dr.BatteryStatus -= myDal.DroneElectricConsumations()[(int)p.Weight + 1] * (BO.LocationFuncs.Distance(dr.DroneLocation, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }));
                else throw new BatteryException ($"DRONE {dr.Id} HASNT ENOUGH BATTERY\n");

                Location l1 = new Location { Latitude = cs.Latitude, Longitude = cs.Longitude };
                dr.DroneLocation = l1;
                dr.Status = Enums.DroneStatuses.Vacant;
                drones[drones.FindIndex(dr => dr.Id == id)] = dr;
                try
                {
                    myDal.UpdateParcel(p);
                }
                catch (ParcelExeption d) {
                    throw new GetException($"Id of parcel {p.Id} not found" , d);
                }
            }
        }
        /// <summary>
        /// put the parcel in the drone
        /// </summary>
        /// <param name="id"></param>
        public void UpdateDroneToPickUpAParcel(int id)
        {//if drone not found
            if (!drones.Any(dr => dr.Id == id))
                throw new GetException("$ID OF DRONE {dr.Id} DOESN'T EXIST\n");
            DroneToList dr = drones.Find(dr => dr.Id == id);
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList();
            List<DO.Parcel> parcelConnected = parcels.FindAll(pc => pc.DroneId == id);

            int index = parcelConnected.FindIndex(pc => pc.Scheduled < DateTime.Now && pc.PickedUp > DateTime.Now);
            //if any parcel has to be delivered
            if (index != -1)
            {
                DO.Parcel p = parcelConnected[index];
                DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                //if hte drone has enough battery to deliver the parcel
                //then make the wanted changes
                if (dr.BatteryStatus >= myDal.DroneElectricConsumations()[0] *
                    BO.LocationFuncs.Distance(dr.DroneLocation, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }))
                {
                    p.PickedUp = DateTime.Now;
                    dr.BatteryStatus -= myDal.DroneElectricConsumations()[0] *
                         BO.LocationFuncs.Distance(dr.DroneLocation, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude });
                    dr.DroneLocation.Latitude = cs.Latitude;
                    dr.DroneLocation.Longitude = cs.Longitude;
                    drones[drones.FindIndex(dr => dr.Id == id)] = dr;
                    myDal.UpdateParcel(p);
                }
                else throw new BatteryException($"DRONE { dr.Id } DOESN'T HAVE ENOUGH BATTERY\n");
            }
        }
        /// <summary>
        /// assign a parcel to a drone
        /// </summary>
        /// <param name="idC"></param>
        public void UpdateAssignParcelToDrone(int idC)
        {
            //if drone not found
            if (!drones.Any(dr => dr.Id == idC))
                throw new GetException ($"ID OF DRONE {idC} DOESN'T EXIST\n");
            DroneToList dr = drones.Find(dr => dr.Id == idC);
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList();
            int found = -1;
            //find the heaviest parcel the drone can take
            for (int i = (int)dr.MaxWeight; i >= 0; i--)
            {
                if (searchParcel(dr, parcels, i) == true)
                {
                    found = i;
                    break;
                }
            }
            //if found a parcel that the drone can carry, update wanted states
            if (found != -1)
            {
                DO.Parcel p = findParcel(dr, parcels, found);
                p.DroneId = dr.Id;
                dr.Status = Enums.DroneStatuses.InDelivery;
                p.Scheduled = DateTime.Now;
                myDal.UpdateParcel(p);
                drones[drones.FindIndex(dr => dr.Id == idC)] = dr;
            }

        }
        /// <summary>
        /// function to release a drone from charging
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="duration"></param>
        public void UpdateReleaseDroneFromCharge(int idC, TimeSpan duration)
        {
            if (!drones.Any(dr => dr.Id == idC))
                throw new GetException ($"ID OF DRONE {idC} DOESN'T EXIST\n");
            if (drones[drones.FindIndex(dr => dr.Id == idC)].Status != Enums.DroneStatuses.Maintenance)
                throw new GetException ($"THE DRONE {idC} ISN'T IN MAINTENANCE\n");
            if ((drones[drones.FindIndex(dr => dr.Id == idC)].BatteryStatus + ((duration.TotalSeconds * 1 / 3600) + (duration.TotalMinutes * 1 / 60) + (duration.TotalHours)) * myDal.DroneElectricConsumations()[4]) > 100)
            {
                drones[drones.FindIndex(dr => dr.Id == idC)].BatteryStatus = 100;
            }
            else
                drones[drones.FindIndex(dr => dr.Id == idC)].BatteryStatus += ((duration.TotalSeconds * 1 / 3600) + (duration.TotalMinutes * 1 / 60) + (duration.TotalHours)) * myDal.DroneElectricConsumations()[4];
            drones[drones.FindIndex(dr => dr.Id == idC)].Status = Enums.DroneStatuses.Vacant;
            List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            DO.BaseStation myBase = bs.Find(bas => bas.Latitude == drones[drones.FindIndex(dr => dr.Id == idC)].DroneLocation.Latitude&&
             bas.Latitude == drones[drones.FindIndex(dr => dr.Id == idC)].DroneLocation.Latitude);
            myBase.NumOfSlots++;


        }
        /// <summary>
        /// Send a drone to charge
        /// </summary>
        /// <param name="id"></param>
        public void UpdateDroneSentToCharge(int id)
        {//if drone not found
            if (!myDal.GetDroneList().Any(dr => dr.Id == id))
            {
                throw new GetException ($"id {id} doesn't exist ");
            }
            DroneToList drone = drones.Find(dr => dr.Id == id);
            // if the drown isn't vacant it can't be charged.
            if (drone.Status != Enums.DroneStatuses.Vacant)
            {
                return;
            }
            BO.BaseStation bc = getClosestBase(drone.DroneLocation);
            List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            DO.BaseStation myBase = bs.Find(bas => bas.Latitude == bc.BaseStationLocation.Latitude&&bas.Longitude==bc.BaseStationLocation.Longitude);
            if ((myDal.DroneElectricConsumations()[0]) *
                BO.LocationFuncs.Distance(drone.DroneLocation,new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude }) <= drone.BatteryStatus)
            {
                drone.BatteryStatus -= (myDal.DroneElectricConsumations()[0]) *
                BO.LocationFuncs.Distance(drone.DroneLocation, new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude });
                drone.DroneLocation = new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude };
                drone.Status = Enums.DroneStatuses.Maintenance;
                myBase.NumOfSlots--;
              
                drones[drones.FindIndex(dr => dr.Id == drone.Id)] = drone;
            }
            else throw new BatteryException($"The drone {drone.Id} doesn't have enough battery to get to the close station.");
        }
        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        public void UpdateCustomerInfo(int idC, string name, string phone)
        {//if customer not found
            if (!myDal.GetCustomerList().Any(cu => cu.Id == idC))
            {
                throw new GetException($"id {idC} doesn't exist ");
            }
            //do the wanted changes
            List<DO.Customer> custom = (List<DO.Customer>)myDal.GetCustomerList();
            DO.Customer bs = custom[custom.FindIndex(cus => cus.Id == idC)];
            if (name != " ")
                bs.Name = name;
            if (phone != " ")
                bs.Phone = phone;
            myDal.UpdateCustomerInfoFromBL(bs);
        }
        /// <summary>
        /// Update baseStatiom numof Slots or name
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="numOfSlots"></param>
        /// <param name="name"></param>
        public void UpdateBaseStation(int myId, int numOfSlots, string name)
        {//if BaseStation not found
            if (!myDal.GetAllBaseStations().Any(bs => bs.Id == myId))
            {
                throw new GetException ($"id {myId} doesn't exist ");
            }
            List<DO.BaseStation> BaseS = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            DO.BaseStation bs = BaseS[BaseS.FindIndex(bs => bs.Id == myId)];
            if (numOfSlots != null)
                bs.NumOfSlots = numOfSlots;
            if (name != "Base ")
                bs.Name = name;
            try
            {
                myDal.UpdateBaseStationFromBl(bs);
            }
            catch (BaseExeption p) {
                throw new GetException($"The Base station {bs.Id} doesn't exist" ,p);
            }

        }
        /// <summary>
        /// Updae name to drone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void UpdateNameDrone(int id, string name)
        {//if drone not found
            if (!myDal.GetDroneList().Any(dr => dr.Id == id))
            {
                throw new GetException($"id {id} doesn't exist ");
            }
            //Update wanted rows
            List<DO.Drone> drs = (List<DO.Drone>)myDal.GetDroneList();
            DO.Drone dr = drs[drs.FindIndex(dr => dr.Id == id)];
            dr.Model = name;
            myDal.UpdateDrone(dr);
            drones[drones.FindIndex(dr => dr.Id == id)].Model = name;
        }
    }
}
