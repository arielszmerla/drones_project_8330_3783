using IBL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBL
{/// <summary>
/// part of BL class containing private funcs and ctor
/// 
/// </summary>
    public partial class BL : IBL
    {
        List<DroneToList> drones = new();
        static Random random = new Random();
        IDAL.IDal myDal;
        /// <summary>
        /// constructor BL
        /// </summary>
        public BL()
        {

            myDal = new DalObject.DalObject();
            double consumationFreeDrone = myDal.DroneElectricConsumations()[0];
            double consumationLightCarrier = myDal.DroneElectricConsumations()[1];
            double consumationMediumCarrier = myDal.DroneElectricConsumations()[2];
            double consumationHeavyCarrier = myDal.DroneElectricConsumations()[3];
            double chargePerHour = myDal.DroneElectricConsumations()[4];
            double shortest = 0;

            foreach (var dr in myDal.GetDroneList())
            {
                DroneToList droneToList = new DroneToList
                {
                    Id = dr.Id,
                    Model = dr.Model,
                    MaxWeight = (Enums.WeightCategories)dr.MaxWeight
                };
                drones.Add(droneToList);
            }
            foreach (var parcel in myDal.GetParcelList())
            {

                if (drones.Any(dr => dr.Id == parcel.DroneId) && parcel.PickedUp >= DateTime.Now)
                {
                    Location closestBase = new();
                    DroneToList droneToList = drones.Find(dr => dr.Id == parcel.DroneId);
                    droneToList.Status = Enums.DroneStatuses.InDelivery;
                    List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
                    List<IDAL.DO.Customer> customers = (List<IDAL.DO.Customer>)myDal.GetCustomerList();
                    IDAL.DO.Customer cs = customers.Find(c => c.Id == parcel.SenderId);
                    //caculate the nearest station to customer

                    droneToList.DroneLocation = getClosestBase(new Location { latitude = cs.Latitude, longitude = cs.Longitude }).BaseStationLocation;

                    List<IDAL.DO.Customer> custom = (List<IDAL.DO.Customer>)myDal.GetCustomerList();
                    IDAL.DO.Customer myCs = custom.Find(cs => cs.Id == parcel.SenderId);
                    droneToList.BatteryStatus = random.Next((int)(LocationFuncs.Distance(closestBase, new Location { latitude = myCs.Latitude, longitude = myCs.Longitude })
                        * consumationFreeDrone + BatteryCons(new Location { latitude = myCs.Latitude, longitude = myCs.Longitude }, parcel)), 99) +
                        random.NextDouble();
                    int index = drones.FindIndex(dr => dr.Id == droneToList.Id);
                    drones[index] = droneToList;
                }
                if (drones.Any(dr => dr.Id == parcel.DroneId) && parcel.Delivered >= DateTime.Now && parcel.PickedUp < DateTime.Now)
                {
                    DroneToList droneToList = drones.Find(dr => dr.Id == parcel.DroneId);
                    List<Customer> customers = (List<Customer>)myDal.GetCustomerList();
                    droneToList.DroneLocation = customers.Find(cs => cs.Id == parcel.SenderId).Location;
                    droneToList.BatteryStatus = random.Next((int)BatteryCons(droneToList.DroneLocation, parcel), 99) + random.NextDouble();
                    int index = drones.FindIndex(dr => dr.Id == droneToList.Id);
                    drones[index] = droneToList;
                }
                List<DroneToList> temp = new();
                for (int i = 0; i < drones.Count; i++)
                {
                    drones[i] = updateDroneToList(drones[i]);
                }

            }
        }
        /// <summary>
        /// func that search if any parcel with a certain weigth category is set to be sent py a certain drone 
        /// abd checks if drone has enough battery to get to it
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="parcels"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private bool searchParcel(DroneToList dr, List<IDAL.DO.Parcel> parcels, int weight)
        {
            //find "to be sent" parcels
            List<IDAL.DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
            if (maxWeightParcels.Count > 0)
            {//checks if the drone can reach them
                IDAL.DO.Parcel p = finfClosestParcelToDrone(dr.DroneLocation, maxWeightParcels);
                IDAL.DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                if (dr.BatteryStatus > BatteryCons(dr.DroneLocation, p)
                    + LocationFuncs.Distance(dr.DroneLocation, new Location { latitude = cs.Latitude, longitude = cs.Longitude }) * myDal.DroneElectricConsumations()[0])
                {
                    return true;

                }
            }
            return false;

        }

        /// <summary>
        /// look for and return is any parcel is actually on the drone
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private ParcelInDelivery? findParcelOnDrone(Drone dr)
        {
            List<IDAL.DO.Parcel> parcels = (List<IDAL.DO.Parcel>)myDal.GetParcelList();
            IDAL.DO.Parcel p = new();
            //find all the parcels on this drone
            List<IDAL.DO.Parcel> onDrone = parcels.FindAll(pcs => (int)pcs.DroneId == dr.Id && pcs.PickedUp < DateTime.Now && pcs.Delivered > DateTime.Now);

            if (onDrone.Count > 0)//if found, create the object relevant and return it
            {
                ParcelInDelivery pid = new ParcelInDelivery();
                pid.Id = onDrone[0].Id;
                pid.WeightCategorie = (Enums.WeightCategories)onDrone[0].Weight;
                IDAL.DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == onDrone[0].SenderId);
                pid.Sender.Id = cs.Id;
                pid.Sender.Name = cs.Name;
                pid.PickUpLocation.latitude = cs.Latitude;
                pid.PickUpLocation.longitude = cs.Longitude;
                if (onDrone[0].Scheduled != DateTime.MinValue)
                {
                    pid.Status = true;
                }
                else pid.Status = false;
                cs = myDal.GetCustomerList().First(cs => cs.Id == onDrone[0].TargetId);
                pid.Target.Id = cs.Id;
                pid.Target.Name = cs.Name;
                pid.TargetLocation.latitude = cs.Latitude;
                pid.TargetLocation.longitude = cs.Longitude;
                pid.Prioritie = (Enums.Priorities)onDrone[0].Priority; 
                pid.Distance = LocationFuncs.Distance(pid.PickUpLocation, pid.TargetLocation);
                return (ParcelInDelivery)pid;
            }
            else return null;
        }
        /// <summary>
        /// find a parcel that can be reache by drone (assuming it exits...)
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="parcels"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private IDAL.DO.Parcel findParcel(DroneToList dr, List<IDAL.DO.Parcel> parcels, int weight)
        {
            IDAL.DO.Parcel p = new();
            List<IDAL.DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
            p = finfClosestParcelToDrone(dr.DroneLocation, maxWeightParcels);
            return p;
        }


        /// <summary>
        /// find closest parcel to the drone that the drone can reach
        /// </summary>
        /// <param name="dronePlace"></param>
        /// <param name="parcels"></param>
        /// <returns></returns>
        private IDAL.DO.Parcel finfClosestParcelToDrone(Location dronePlace, List<IDAL.DO.Parcel> parcels)
        {
            double tmpLength = 40000000000000;
            IDAL.DO.Parcel tmpParcel = new();
            //check all the parcels
            for (int i = 0; i < parcels.Count; i++)
            {
               
                Location lc = new();
                lc.latitude = myDal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Latitude;
                lc.longitude = myDal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Longitude;
                if (LocationFuncs.Distance(dronePlace, lc) < tmpLength)
                {
                    tmpLength = LocationFuncs.Distance(dronePlace, lc);
                    tmpParcel = parcels[i];
                }
            }
            return tmpParcel;
        }
        /// <summary>
        /// return Battery consumation for going to reach parcel by customer and
        /// sent it to the target customer and goto closest base
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="parcel"></param>
        /// <returns></returns>
        private double BatteryCons(Location lc, IDAL.DO.Parcel parcel)
        {
            double total = 0;
            double temp = 99999999999; ;
            IDAL.DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == parcel.TargetId);
            total += LocationFuncs.Distance(lc, new Location { latitude = cs.Latitude, longitude = cs.Longitude });
            List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
            Location closestBase = new();
            //look after closest baseStation
            for (int i = 0; i < bs.Count(); i++)
            {
                if (LocationFuncs.Distance(new Location { latitude = cs.Latitude, longitude = cs.Longitude }, new Location { latitude = bs[i].Latitude, longitude = bs[i].Longitude }) < temp)
                {
                    temp = LocationFuncs.Distance(new Location { latitude = cs.Latitude, longitude = cs.Longitude }, new Location { latitude = bs[i].Latitude, longitude = bs[i].Longitude });
                    closestBase = new Location { latitude = bs[i].Latitude, longitude = bs[i].Longitude };
                }
            }//return amount of battery needed to the trip
            return (total * myDal.DroneElectricConsumations()[(int)(parcel.Weight + 1)] + temp * myDal.DroneElectricConsumations()[0]);
        }

 
       /// <summary>
       /// Create the list of drone charging on a baseStation
       /// </summary>
       /// <param name="bs"></param>
       /// <returns></returns>
        private List<DroneCharge> dronCharges(BaseStation bs)
        {//find the relevant drones
            List<DroneToList> drs = drones.FindAll(dr => dr.Status == Enums.DroneStatuses.Maintenance&& dr.DroneLocation == bs.BaseStationLocation);
            List<DroneCharge> droneCharges = new();
            foreach (DroneToList ds in drs)
            {
                    droneCharges.Add(new DroneCharge
                    {
                        Id = ds.Id,
                        BatteryStatus = ds.BatteryStatus
                    });
            }
            return droneCharges;
        }
     /// <summary>
     /// calls func from dalObject to get customer with and Id 
     /// </summary>
     /// <param name="id"></param>
     /// <returns></returns>
        public IDAL.DO.Customer GetCust(int id)
        {
            try
            {
                return myDal.GetCustomer(id);
            }
            catch (DalObject.CostumerExeption p) {
                throw new GetException($"This Customer {id} does not exist",p);
            }
        }
        /// <summary>
        /// Gets the closest baseStation to a certain location
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private BaseStation getClosestBase(Location dr)
        {
            List < IDAL.DO.BaseStation > bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
           IDAL.DO. BaseStation closestBase = new();
            double shortest = 99999999999999990;
            //find closest
            int i = 0;
            for (; i < bs.Count(); i++)
            {
                if (bs[i].NumOfSlots > 0 && LocationFuncs.Distance(dr,new Location { latitude = bs[i].Latitude, longitude = bs[i].Longitude }) < shortest)
                {
                    shortest = LocationFuncs.Distance(dr, new Location { latitude = bs[i].Latitude, longitude = bs[i].Longitude });
                    closestBase = bs[i];
                }
            }

            BaseStation b = new BaseStation { BaseStationLocation = new Location{ latitude=closestBase.Latitude, longitude=closestBase.Longitude  },
                Id = closestBase.Id,
                Name = closestBase.Name,
                NumOfFreeSlots = closestBase.NumOfSlots };
            List<DroneCharge> dc = new();
            foreach (var drone in drones)
            {
                if (drone.DroneLocation == new Location { latitude = closestBase.Latitude, longitude = closestBase.Longitude })
                {
                    DroneCharge d = new DroneCharge { Id = drone.Id, BatteryStatus = drone.BatteryStatus };
                    dc.Add(d);
                }
            }
            return b;
        }


        private DroneToList updateDroneToList(DroneToList dronetolis) {

            if (dronetolis.Status != Enums.DroneStatuses.InDelivery)
            {
                //puts the drone or vacant or maintenance
                dronetolis.Status = (Enums.DroneStatuses)random.Next(2);
            }
            if (dronetolis.Status == Enums.DroneStatuses.Maintenance)
            {

                List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
                int rand = random.Next(bs.Count());
                Location loc = new Location { latitude = bs[rand].Latitude, longitude = bs[rand].Longitude };
              ;
                dronetolis.DroneLocation = loc;
                dronetolis.BatteryStatus = 20 * random.NextDouble();
            }
            if (dronetolis.Status == Enums.DroneStatuses.Vacant)
            {
                List<IDAL.DO.Customer> c = (List<IDAL.DO.Customer>)myDal.GetCustomerList();
                List<IDAL.DO.Parcel> parcels = (List<IDAL.DO.Parcel>)myDal.GetParcelList();
                List<IDAL.DO.Parcel> tmp = parcels.FindAll(pc => pc.Delivered < DateTime.Now && pc.Delivered != DateTime.MinValue);
                List<IDAL.DO.Customer> customersTmp = new();
                foreach (var it in tmp)
                {
                    if (c.Any(cs => cs.Id == it.TargetId))
                        customersTmp.Add(c.Find(cs => cs.Id == it.TargetId));
                }
                List<IDAL.DO.BaseStation> bs = (List<IDAL.DO.BaseStation>)myDal.GetAllBaseStations();
                Location lc = new Location { latitude = bs[0].Latitude, longitude = bs[0].Longitude };

                dronetolis.DroneLocation = lc;
                dronetolis.BatteryStatus = random.Next((int)(LocationFuncs.Distance(dronetolis.DroneLocation, getClosestBase(dronetolis.DroneLocation).BaseStationLocation) * myDal.DroneElectricConsumations()[0]), 99) + random.NextDouble();
            }
            return dronetolis;
        }
    }
}



