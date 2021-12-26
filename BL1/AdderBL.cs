using BO;
using BLAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLAPI;

/// <summary>
/// partial Bl Class
///with Add functs
/// </summary>
namespace BL
{/// <summary>
/// partial Bl Class
///with Add functs
/// </summary>
    partial class BLImp : IBL
    {
        /// <summary>
        /// add a costumer to database
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            //checks if exists already
            if (myDal.GetCustomerList().Any(item => item.Id == customer.Id))
            {
                throw new AddException($"id {customer.Id} exist already");
            }
            if (customer.Id < 10000000 ||
           customer.Id >999999999)
                throw new AddException("Invalid id  ");
            if (customer.Location.Latitude < 31.740967 || customer.Location.Latitude > 31.815177)
                throw new AddException("Incorect Latitude, please enter correct Jerusalem coordinates");

            try
            {
                //calling mydal after mapping BO to DO
                myDal.AddCustomer(new DO.Customer
                {
                    Id = customer.Id,
                    Latitude = customer.Location.Latitude,
                    Longitude = customer.Location.Longitude,
                    Name = customer.Name,
                    Phone = customer.Phone
                });
            }
            catch (CostumerExeption d)
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
          if (myDal.GetBaseStationsList(null).Any(item =>item.Id == station.Id))
                    throw new AddException($"id {station.Id} exist already");

            if (station.NumOfFreeSlots < 3 || station.NumOfFreeSlots > 20)
                throw new AddException("Invalid amount of number of free slots ");
            if (station.Location.Latitude < 31.740967 || station.Location.Latitude > 31.815177)
                throw new AddException("Incorect Latitude, please enter correct Jerusalem coordinates");
            if (station.Location.Longitude < 35.171323 || station.Location.Longitude > 35.202050)
                throw new AddException("Incorect Longitude, please enter correct Jerusalem coordinates");
            try
            {
                //calling mydal after mapping BO to DO
                myDal.AddBaseStation(new DO.BaseStation
                {
                    Id = station.Id,
                    Latitude = station.Location.Latitude,
                    Longitude = station.Location.Longitude,
                    Name = station.Name,
                    NumOfSlots = station.NumOfFreeSlots
                });
            }
            catch (BaseExeption d)
            {
                throw new AddException($"The Base station {station.Id} already exists ", d);
            }
        }

        /// <summary>
        /// method to add a drone
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="BaseStationNum"></param>
        public void AddDrone(DO.Drone drone, int BaseStationNum)
        { //checks if exists already
            foreach (var item in myDal.GetDroneList())
            {
                if (item.Id == drone.Id)
                {
                    throw new AddException($"id {drone.Id} exist already");
                }
            } //checks if basestation to send drone exists 
            if (!myDal.GetBaseStationsList(null).Any(ps => ps.Id == BaseStationNum))
            {
                throw new GetException($"id {drone.Id} doesn't exist ");
            }
            //calling mydal after mapping BO to DO
            try
            {
                myDal.AddDrone(drone);
            }
            catch (DroneException d)
            {

                throw new AddException($"The drone {drone.Id} already exists ", d);
            }

            List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetBaseStationsList(null);
            //put it in the BL droneList
            DroneToList dr = new DroneToList
            {
                Id = drone.Id,
                MaxWeight = (Enums.WeightCategories)drone.MaxWeight,
                Model = (Enums.DroneNames)drone.Model,
                BatteryStatus = 20 + 20 * random.NextDouble(),
                Status = Enums.DroneStatuses.Maintenance,
                Location = new Location { Latitude = bs.Find(b => b.Id == BaseStationNum).Latitude, Longitude = bs.Find(b => b.Id == BaseStationNum).Longitude },
                NumOfDeliveredParcel = 0
            };
            drones.Add(dr);
        }

        public void AddDrone(Drone drone)
        {
            //checks if exists already
            foreach (var item in myDal.GetDroneList())
            {
                if (item.Id == drone.Id)
                {
                    throw new AddException($"id {drone.Id} exist already");
                }
            } //checks if basestation to send drone exists 
            DO.Drone drone1 = new DO.Drone
            {
                Id = drone.Id,
                MaxWeight = (DO.WeightCategories)drone.MaxWeight,
                Model = (DO.DroneNames)drone.Model
            };
            //drone1.Id = drone.Id;

            //calling mydal after mapping BO to DO
            try
            {
                myDal.AddDrone(drone1);
            }
            catch (DroneException d)
            {

                throw new AddException($"The drone {drone.Id} already exists ", d);
            }

          
            //put it in the BL droneList
            DroneToList dr = new DroneToList
            {
                Id = drone.Id,
                BatteryStatus = drone.BatteryStatus,
                Location = drone.DronePlace,
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
                myDal.AddParcel(new DO.Parcel
                {
                    Id = parcel.Id,
                    Delivered = parcel.Delivered,
                    DroneId = parcel.DIP.Id,
                    PickedUp = parcel.PickedUp,
                    Requested = parcel.Created,
                    Scheduled = parcel.Assignment,
                    SenderId = parcel.Sender.Id,
                    TargetId = parcel.Target.Id,
                    Weight = (DO.WeightCategories)parcel.WeightCategories
                });
            }
            catch (ParcelExeption pr)
            {
                throw new AddException($"The parcel {parcel.Id} alreaedy exists.", pr);
            }
        }

    }
}
