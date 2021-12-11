﻿using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;

/// <summary>
/// partial Bl Class
///with Add functs
/// </summary>
namespace BL
{/// <summary>
/// partial Bl Class
///with Add functs
/// </summary>
    public partial class BLImp : IBL
    {
        /// <summary>
        /// add a costumer to database
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            //checks if exists already
            foreach (var item in myDal.GetParcelList())
            {
                if (item.Id == customer.Id)
                {
                    throw new AddException ($"id {customer.Id} exist already");
                }
            }
            try
            {
                //calling mydal after mapping BO to DO
                myDal.AddCustomer(new DO.Customer
                {
                    Id = customer.Id,
                    Latitude = customer.Location.latitude,
                    Longitude = customer.Location.longitude,
                    Name = customer.Name,
                    Phone = customer.Phone
                });
            }
            catch (DalObject.CostumerExeption d)
            {
                throw new AddException($"The Customer {customer.Id} already exists ", d);
            }
        }
        /// <summary>
        /// add BaseStation to database
        /// </summary>
        /// <param name="station"></param>
        public void AddBaseStation(BaseStation station)
        { //checks if exists already
            foreach (var item in myDal.GetAllBaseStations())
            {
                if (item.Id == station.Id)
                {
                    throw new AddException($"id {station.Id} exist already");
                }
            }
            try
            {
                //calling mydal after mapping BO to DO
                myDal.AddBaseStation(new IDAL.DO.BaseStation
                {
                    Id = station.Id,
                    Latitude = station.BaseStationLocation.latitude,
                    Longitude = station.BaseStationLocation.longitude,
                    Name = station.Name,
                    NumOfSlots = station.NumOfFreeSlots
                });
            }
            catch (DalObject.BaseExeption d)
            {

                throw new AddException($"The Base station {station.Id} already exists " , d);
            }
        }

        /// <summary>
        /// method to add a drone
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="BaseStationNum"></param>
        public void AddDrone(IDAL.DO.Drone drone, int BaseStationNum)
        { //checks if exists already
            foreach (var item in myDal.GetDroneList())
            {
                if (item.Id == drone.Id)
                {
                    throw new AddException($"id {drone.Id} exist already");
                }
            } //checks if basestation to send drone exists 
            if (!myDal.GetAllBaseStations().Any(ps => ps.Id == BaseStationNum))
            {
                throw new GetException($"id {drone.Id} doesn't exist ");
            }
            //calling mydal after mapping BO to DO
            try
            {
                myDal.AddDrone(drone);
            }
            catch (DalObject.DroneException d)
            {

                throw new AddException($"The drone {drone.Id} already exists " , d);
            }

            List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
            //put it in the BL droneList
            DroneToList dr = new DroneToList
            {
                Id = drone.Id,
                MaxWeight = (Enums.WeightCategories)drone.MaxWeight,
                Model = drone.Model,
                BatteryStatus = 20 + 20 * random.NextDouble(),
                Status = Enums.DroneStatuses.Maintenance,
                DroneLocation = new Location { latitude = bs.Find(b => b.Id == BaseStationNum).Latitude, longitude = bs.Find(b => b.Id == BaseStationNum).Longitude },
                NumOfDeliveredParcel = 0
            };
            drones.Add(dr);
        }

       public void AddDrone(Drone drone) {
            //checks if exists already
                foreach (var item in myDal.GetDroneList())
                {
                    if (item.Id == drone.Id)
                    {
                        throw new AddException($"id {drone.Id} exist already");
                    }
                } //checks if basestation to send drone exists 
                IDAL.DO.Drone drone1 = new IDAL.DO.Drone { Id= drone.Id,
                 MaxWeight = (IDAL.DO.WeightCategories)drone.MaxWeight, Model = drone.Model};
                //drone1.Id = drone.Id;
               
                //calling mydal after mapping BO to DO
                try
                {
                    myDal.AddDrone(drone1);
                }
                catch (DalObject.DroneException d)
                {

                    throw new AddException($"The drone {drone.Id} already exists ", d);
                }

                List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
            //put it in the BL droneList
            DroneToList dr = new DroneToList
            {
                Id = drone.Id,
                BatteryStatus = drone.BatteryStatus,
                DroneLocation = drone.DronePlace,
                MaxWeight = drone.MaxWeight,
                Model = drone.Model,
                NumOfDeliveredParcel = 0,
                Status = drone.Status
            };
                drones.Add(dr);

            }
        /// <summary>
        /// adds new  parcel to datasource
        /// </summary>
        /// <param name="parcel"></param>
        public void AddParcel(Parcel parcel)
        {//check if exits already
            if (!myDal.GetCustomerList().Any(c => c.Id == parcel.Sender.Id))
                throw new GetException($"id sender {parcel.Sender.Id} doesn't exist ");
            if (!myDal.GetCustomerList().Any(c => c.Id == parcel.Target.Id))
                throw new GetException($"id target {parcel.Target.Id} doesn't exist ");
            //calling mydal after mapping BO to DO
            try
            {
                myDal.AddParcel(new IDAL.DO.Parcel
                {
                    Id = parcel.Id,
                    Delivered = parcel.Delivered,
                    DroneId = parcel.DIP.Id,
                    PickedUp = parcel.PickedUp,
                    Requested = parcel.Created,
                    Scheduled = parcel.Assignment,
                    SenderId = parcel.Sender.Id,
                    TargetId = parcel.Target.Id,
                    Weight = (IDAL.DO.WeightCategories)parcel.WeightCategories
                });
            }
            catch (DalObject.ParcelExeption pr)
            {
                throw new AddException($"The parcel {parcel.Id} alreaedy exists." , pr);
            }
        }

    }
}
