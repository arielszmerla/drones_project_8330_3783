using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using DLAPI;
using DO;
using BO;

namespace BL
{/// <summary>
/// part of BL class containing private funcs and ctor
/// 
/// </summary>
    partial class BLImp : IBL
    {
       private List<DroneToList> drones = new();
        static Random random = new Random();
      
        
        IDal myDal;
        /// <summary>
        /// constructor BL
        /// </summary>
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly BLImp instance =new BLImp() ;
        }
        private static object syncRoot = new object();
        public static BLImp Instance
        {
            get
            {
                if (Nested.instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Nested.instance == null)
                            return Nested.instance;
                    }
                }

                return Nested.instance;
            }
        }
       
    
      
        #endregion

        private BLImp()
        {

            try
            {
                myDal = DLFactory.GetDL("1");
            }
            catch ( DLConfigException  ex) {
                throw new BLConfigException("", ex);

            }



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
                    List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
                    List<DO.Customer> customers = (List<DO.Customer>)myDal.GetCustomerList();
                    DO.Customer cs = customers.Find(c => c.Id == parcel.SenderId);
                    //caculate the nearest station to customer

                    droneToList.DroneLocation = getClosestBase(new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }).BaseStationLocation;

                    List<DO.Customer> custom = (List<DO.Customer>)myDal.GetCustomerList();
                    DO.Customer myCs = custom.Find(cs => cs.Id == parcel.SenderId);
                    droneToList.BatteryStatus = random.Next((int)(BO.LocationFuncs.Distance(closestBase, new Location { Latitude = myCs.Latitude, Longitude = myCs.Longitude })
                        * consumationFreeDrone + BatteryCons(new Location { Latitude = myCs.Latitude, Longitude = myCs.Longitude }, parcel)), 99) +
                        random.NextDouble();
                    int index = drones.FindIndex(dr => dr.Id == droneToList.Id);
                    drones[index] = droneToList;
                }
                if (drones.Any(dr => dr.Id == parcel.DroneId) && parcel.Delivered >= DateTime.Now && parcel.PickedUp < DateTime.Now)
                {
                    DroneToList droneToList = drones.Find(dr => dr.Id == parcel.DroneId);
                    List<DO.Customer> customers = (List<DO.Customer>)myDal.GetCustomerList();
              
                     BO.Location loci = new BO.Location { Latitude = customers.Find(cs => cs.Id == parcel.SenderId).Latitude, Longitude = customers.Find(cs => cs.Id == parcel.SenderId).Longitude };
                    droneToList.DroneLocation = loci;
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
        private bool searchParcel(DroneToList dr, List<DO.Parcel> parcels, int weight)
        {
            //find "to be sent" parcels
            List<DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
            if (maxWeightParcels.Count > 0)
            {//checks if the drone can reach them
                DO.Parcel p = finfClosestParcelToDrone(dr.DroneLocation, maxWeightParcels);
                DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                if (dr.BatteryStatus > BatteryCons(dr.DroneLocation, p)
                    + BO.LocationFuncs.Distance(dr.DroneLocation, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }) * myDal.DroneElectricConsumations()[0])
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
        private ParcelInDelivery? findParcelOnDrone(BO.Drone dr)
        {
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList();
            DO.Parcel p = new();
            //find all the parcels on this drone
            List<DO.Parcel> onDrone = parcels.FindAll(pcs => (int)pcs.DroneId == dr.Id && pcs.PickedUp < DateTime.Now && pcs.Delivered > DateTime.Now);

            if (onDrone.Count > 0)//if found, create the object relevant and return it
            {
                ParcelInDelivery pid = new ParcelInDelivery();
                pid.Id = onDrone[0].Id;
                pid.WeightCategorie = (Enums.WeightCategories)onDrone[0].Weight;
                DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == onDrone[0].SenderId);
                pid.Sender.Id = cs.Id;
                pid.Sender.Name = cs.Name;
                pid.PickUpLocation.Latitude = cs.Latitude;
                pid.PickUpLocation.Longitude = cs.Longitude;
                if (onDrone[0].Scheduled != DateTime.MinValue)
                {
                    pid.Status = true;
                }
                else pid.Status = false;
                cs = myDal.GetCustomerList().First(cs => cs.Id == onDrone[0].TargetId);
                pid.Target.Id = cs.Id;
                pid.Target.Name = cs.Name;
                pid.TargetLocation.Latitude = cs.Latitude;
                pid.TargetLocation.Longitude = cs.Longitude;
                pid.Prioritie = (Enums.Priorities)onDrone[0].Priority; 
                pid.Distance = BO.LocationFuncs.Distance(pid.PickUpLocation, pid.TargetLocation);
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
        private DO.Parcel findParcel(DroneToList dr, List<DO.Parcel> parcels, int weight)
        {
            DO.Parcel p = new();
            List<DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
            p = finfClosestParcelToDrone(dr.DroneLocation, maxWeightParcels);
            return p;
        }


        /// <summary>
        /// find closest parcel to the drone that the drone can reach
        /// </summary>
        /// <param name="dronePlace"></param>
        /// <param name="parcels"></param>
        /// <returns></returns>
        private DO.Parcel finfClosestParcelToDrone(Location dronePlace, List<DO.Parcel> parcels)
        {
            double tmpLength = 40000000000000;
           DO.Parcel tmpParcel = new();
            //check all the parcels
            for (int i = 0; i < parcels.Count; i++)
            {
               
                Location lc = new();
                lc.Latitude = myDal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Latitude;
                lc.Longitude = myDal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Longitude;
                if (BO.LocationFuncs.Distance(dronePlace, lc) < tmpLength)
                {
                    tmpLength = BO.LocationFuncs.Distance(dronePlace, lc);
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
        private double BatteryCons(Location lc, DO.Parcel parcel)
        {
            double total = 0;
            double temp = 99999999999; ;
            DO.Customer cs = myDal.GetCustomerList().First(cs => cs.Id == parcel.TargetId);
            total += BO.LocationFuncs.Distance(lc, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude });
            List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            Location closestBase = new();
            //look after closest baseStation
            for (int i = 0; i < bs.Count(); i++)
            {
                if (BO.LocationFuncs.Distance(new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude }) < temp)
                {
                    temp = BO.LocationFuncs.Distance(new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude });
                    closestBase = new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude };
                }
            }//return amount of battery needed to the trip
            return (total * myDal.DroneElectricConsumations()[(int)(parcel.Weight + 1)] + temp * myDal.DroneElectricConsumations()[0]);
        }

 
       /// <summary>
       /// Create the list of drone charging on a baseStation
       /// </summary>
       /// <param name="bs"></param>
       /// <returns></returns>
        private List<BO.DroneCharge> dronCharges(BO.BaseStation bs)
        {//find the relevant drones
            List<DroneToList> drs = drones.FindAll(dr => dr.Status == Enums.DroneStatuses.Maintenance&& dr.DroneLocation == bs.BaseStationLocation);
            List<BO.DroneCharge> droneCharges = new();
            foreach (DroneToList ds in drs)
            {
                    droneCharges.Add(new BO.DroneCharge
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
        public DO.Customer GetCust(int id)
        {
            try
            {
                return myDal.GetCustomer(id);
            }
            catch (CostumerExeption p) {
                throw new GetException($"This Customer {id} does not exist",p);
            }
        }
        /// <summary>
        /// Gets the closest baseStation to a certain location
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private BO.BaseStation getClosestBase(Location dr)
        {
            List <DO.BaseStation > bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
           DO. BaseStation closestBase = new();
            double shortest = 99999999999999990;
            //find closest
            int i = 0;
            for (; i < bs.Count(); i++)
            {
                if (bs[i].NumOfSlots > 0 && BO.LocationFuncs.Distance(dr,new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude }) < shortest)
                {
                    shortest =BO. LocationFuncs.Distance(dr, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude });
                    closestBase = bs[i];
                }
            }

           BO. BaseStation b = new BO.BaseStation { BaseStationLocation = new Location{ Latitude=closestBase.Latitude, Longitude=closestBase.Longitude  },
                Id = closestBase.Id,
                Name = closestBase.Name,
                NumOfFreeSlots = closestBase.NumOfSlots };
            List<BO.DroneCharge> dc = new();
            foreach (var drone in drones)
            {
                if (drone.DroneLocation == new Location { Latitude = closestBase.Latitude, Longitude = closestBase.Longitude })
                {
                   BO. DroneCharge d = new BO.DroneCharge { Id = drone.Id, BatteryStatus = drone.BatteryStatus };
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

                List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
                int rand = random.Next(bs.Count());
                Location loc = new Location { Latitude = bs[rand].Latitude, Longitude = bs[rand].Longitude };
              ;
                dronetolis.DroneLocation = loc;
                dronetolis.BatteryStatus = 20 * random.NextDouble();
            }
            if (dronetolis.Status == Enums.DroneStatuses.Vacant)
            {
                List<DO.Customer> c = (List<DO.Customer>)myDal.GetCustomerList();
                List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList();
                List<DO.Parcel> tmp = parcels.FindAll(pc => pc.Delivered < DateTime.Now && pc.Delivered != DateTime.MinValue);
                List<DO.Customer> customersTmp = new();
                foreach (var it in tmp)
                {
                    if (c.Any(cs => cs.Id == it.TargetId))
                        customersTmp.Add(c.Find(cs => cs.Id == it.TargetId));
                }
                List<DO.BaseStation> bs = (List<DO.BaseStation>)myDal.GetAllBaseStations();
                Location lc = new Location { Latitude = bs[0].Latitude, Longitude = bs[0].Longitude };

                dronetolis.DroneLocation = lc;
                dronetolis.BatteryStatus = random.Next((int)(BO.LocationFuncs.Distance(dronetolis.DroneLocation, getClosestBase(dronetolis.DroneLocation).BaseStationLocation) * myDal.DroneElectricConsumations()[0]), 99) + random.NextDouble();
            }
            return dronetolis;
        }
    }
}



