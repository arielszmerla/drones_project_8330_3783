using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using DLAPI;
using DO;
using BO;
using System.Runtime.CompilerServices;

namespace BL
{/// <summary>
/// part of BL class containing private funcs and ctor
/// 
/// </summary>
    partial class BLImp : IBL
    {
        private List<DroneToList> drones = new();
        static Random random = new Random();

        internal readonly double[] BatteryUsages;
        internal const int DRONE_FREE = 0;
        internal const int DRONE_CHARGE = 4;

        internal readonly IDal Dal;
        /// <summary>
        /// constructor BL
        /// </summary>
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly BLImp instance = new BLImp();
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
                Dal = DLFactory.GetDL("2");
            }
            catch (DLConfigException ex)
            {
                throw new BLConfigException("", ex);

            }
            BatteryUsages = Dal.DroneElectricConsumations().Select(n => n / 100.0).ToArray();

            IntializeDrones();


        }
        /// <summary>
        /// iniatialises the drone to list property
        /// </summary>
        /*  void IntializeDrone()
          {
              drones = (from d in Dal.GetDroneList()
                        let drone = (DO.Drone)d
                        select new DroneToList
                        {
                            Id = drone.Id,
                            Model = (Enums.DroneNames)drone.Model,
                            MaxWeight = (Enums.WeightCategories)drone.MaxWeight,
                            Valid = true
                        }).ToList();

              foreach (var drone in drones)
              {
                  try
                  {
                      if (random.NextDouble() < 0.5)
                          throw new Exception(); // to go to catch
                      drone.Location = Dal.GetBaseStation(Dal.GetDroneChargeBaseStationId(drone.Id)).Location();
                      drone.Status = Enums.DroneStatuses.Maintenance;
                      drone.Battery = 0.05 + 0.15 * random.NextDouble();
                  }
                  catch (Exception)
                  {
                      int? parcelId = Dal.GetParcelList(null).FirstOrDefault(p => p.DroneId == drone.Id
                                                                    && p.Scheduled != null
                                                                    && p.Delivered == null).Id;
                      if (parcelId != null)
                      {
                          drone.DeliveryId = parcelId;
                          drone.Status = Enums.DroneStatuses.InDelivery;
                          drone.Location = findDroneLocation(drone);
                          double minBattery = drone.RequiredBattery(this, (int)parcelId);
                          drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);
                      }
                      else
                      {
                          drone.Status = Enums.DroneStatuses.Vacant;
                          drone.Location = findDroneLocation(drone);
                          double minBattery = BatteryUsages[(int)Enums.BatteryUsage.Available] * drone.Distance(FindClosestBaseStation(drone));
                          drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);
                      }
                  }
              }
          }*/


        void IntializeDrones()
        {
            double consumationFreeDrone = Dal.DroneElectricConsumations()[0];
            double consumationLightCarrier = Dal.DroneElectricConsumations()[1];
            double consumationMediumCarrier = Dal.DroneElectricConsumations()[2];
            double consumationHeavyCarrier = Dal.DroneElectricConsumations()[3];
            double chargePerHour = Dal.DroneElectricConsumations()[4];


            drones = (from drone in Dal.GetDroneList(d => d.Valid == true)
                      let dr = (DO.Drone)drone
                      select new DroneToList
                      {
                          Id = dr.Id,
                          Model = (Enums.DroneNames)dr.Model,
                          MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                          Valid = true,
                          DeliveryId = null
                      }).ToList();
            foreach (var drone in drones)
            {
                if (random.NextDouble() > 0.5)
                {

                  List<DO.BaseStation> bases = (List<DO.BaseStation>)Dal.GetBaseStationsList(b=>b.NumOfSlots>0);
                   DO. BaseStation b = Dal.GetBaseStation(bases[random.Next(bases.Count() - 1)].Id);
                    drone.Location =dOBaseStation( b).Location;
                    Dal.AddDroneCharge(drone.Id, b.Id);
                    drone.Status = Enums.DroneStatuses.Maintenance;
                    drone.Battery = 0.05 + 0.15 * random.NextDouble();
                }
                else
                {

                    int? parcelId = Dal.GetParcelList(p => p.DroneId == drone.Id && p.Scheduled !=null  && p.Delivered == null).FirstOrDefault().Id;
                    if (parcelId != 0)
                    {

                        drone.DeliveryId = parcelId;
                        drone.Status = Enums.DroneStatuses.InDelivery;
                        drone.Location = findDroneLocation(drone);
                        double minBattery = drone.RequiredBattery(this, (int)parcelId);
                        drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);

                    }
                    else
                    {
                        drone.Status = Enums.DroneStatuses.Vacant;
                        drone.Location = findDroneLocation(drone);
                        double minBattery = BatteryUsages[(int)Enums.BatteryUsage.Available] * drone.Distances(FindClosestBaseStation(drone));
                        drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);
                    }
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
                DO.Parcel p = finfClosestParcelToDrone(dr.Location, maxWeightParcels);
                DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                if (dr.Battery > BatteryCons(dr.Location, p)
                    + BO.LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }) * Dal.DroneElectricConsumations()[0])
                {
                    return true;

                }
            }
            return false;

        }
        private Location findDroneLocation(DroneToList drone)
        {
            int? parcelId = drone.DeliveryId ;

            switch (drone.Status)
            {
                case Enums.DroneStatuses.Maintenance:
                    return Dal.GetBaseStation(Dal.GetDroneChargeBaseStationId(drone.Id)).Location();

                case Enums.DroneStatuses.InDelivery:
                    
                        DO.Parcel parcel = Dal.GetParcel((int)parcelId);

                        if (parcel.PickedUp > DateTime.Now)
                        {
                            BO.Customer customer = GetCustomer(parcel.SenderId);
                            return FindClosestBaseStation(customer).Location;
                        }
                        if (parcel.Delivered > DateTime.Now)
                        {
                            return GetCustomer(parcel.SenderId).Location;
                        }
                        return Dal.GetCustomer(parcel.TargetId).Location();
                    
                case Enums.DroneStatuses.Vacant:
                    if (parcelId!=null)
                        return GetCustomer(Dal.GetParcel((int)parcelId).TargetId).Location;

                    IEnumerable<DO.Parcel> targets = Dal.GetParcelList(parcel => parcel.DroneId == drone.Id && parcel.TargetId != 0);


                    if (random.NextDouble() < 0.5 && targets.Any())
                    {

                        int[] idss = (from item in targets
                                      let num = item.TargetId
                                      select num).ToArray();
                        return GetCustomer(idss[random.Next(idss.Length)]).Location;
                    }

                    var stations = Dal.GetBaseStationsList(b => b.Valid == true).ToArray();
                    if (stations.Any())
                        return stations[random.Next(1, stations.Length)].Location();
                    return new Location
                    {
                        Latitude = (double)random.Next(31740967, 31815177) / (double)1000000,
                        Longitude = (double)random.Next(35171323, 35202050) / (double)1000000
                    };
                default:
                    return new Location
                    {
                        Latitude = (double)random.Next(31740967, 31815177) / (double)1000000,
                        Longitude = (double)random.Next(35171323, 35202050) / (double)1000000
                    };
            }
        }

        /// <summary>
        /// look for and return is any parcel is actually on the drone
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private ParcelInDelivery? findParcelOnDrone(BO.Drone dr)
        {

            //List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);

            //find all the parcels on this drone
            //  .FindAll(pcs => (int)pcs.DroneId == dr.Id && pcs.PickedUp < DateTime.Now && pcs.Delivered > DateTime.Now);
            List<DO.Parcel> onDrone = (List<DO.Parcel>)(from parcel in Dal.GetParcelList(null)
                                                        where parcel.DroneId == dr.Id && parcel.PickedUp < DateTime.Now && parcel.Delivered > DateTime.Now
                                                        select parcel).ToList();


            if (onDrone.Count() > 0)//if found, create the object relevant and return it
            {
                ParcelInDelivery pid = new ParcelInDelivery();
                pid.Id = onDrone[0].Id;
                pid.WeightCategorie = (Enums.WeightCategories)onDrone[0].Weight;
                DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == onDrone[0].SenderId);
                pid.Sender.Id = cs.Id;
                pid.Sender.Name = cs.Name;
                pid.Location.Latitude = cs.Latitude;
                pid.Location.Longitude = cs.Longitude;
                if (onDrone[0].Scheduled != DateTime.MinValue)
                {
                    pid.Status = true;
                }
                else pid.Status = false;
                cs = Dal.GetCustomerList().First(cs => cs.Id == onDrone[0].TargetId);
                pid.Target.Id = cs.Id;
                pid.Target.Name = cs.Name;
                pid.TargetLocation.Latitude = cs.Latitude;
                pid.TargetLocation.Longitude = cs.Longitude;
                pid.Prioritie = (Enums.Priorities)onDrone[0].Priority;
                pid.Distance = BO.LocationFuncs.Distance(pid.Location, pid.TargetLocation);
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
            List<DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
            return finfClosestParcelToDrone(dr.Location, maxWeightParcels);
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
                lc.Latitude = Dal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Latitude;
                lc.Longitude = Dal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Longitude;
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
            DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == parcel.TargetId);
            total += BO.LocationFuncs.Distance(lc, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude });
            List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(null);
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
            return (total * Dal.DroneElectricConsumations()[(int)(parcel.Weight + 1)] + temp * Dal.DroneElectricConsumations()[0]);
        }


        /// <summary>
        /// Create the list of drone charging on a baseStation
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private List<BO.DroneCharge> dronCharges(BO.BaseStation bs)
        {//find the relevant drones

            List<BO.DroneCharge> droneCharges = new();
            IEnumerable<BO.DroneToList> dr = drones.FindAll(dr => dr.Status == Enums.DroneStatuses.Maintenance && dr.Location.Latitude == bs.Location.Latitude);
            foreach (var item in dr)
            {
                BO.DroneCharge d = new BO.DroneCharge { Id = item.Id, BatteryStatus = item.Battery };
                droneCharges.Add(d);
            }
            return droneCharges;

        }
        /// <summary>
        /// calls func from dalObject to get customer with and Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
     /*   public DO.Customer GetCust(int id)
        {
            try
            {
                return Dal.GetCustomer(id);
            }
            catch (CostumerExeption p)
            {
                throw new GetException($"This Customer {id} does not exist", p);
            }
        }*/
        /// <summary>
        /// Gets the closest baseStation to a certain location
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private BO.BaseStation getClosestBase(Location dr)
        {
            List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(null);
            DO.BaseStation closestBase = new();
            double shortest = 99999999999999990;
            //find closest
            int i = 0;
            for (; i < bs.Count(); i++)
            {
                if (bs[i].NumOfSlots > 0 && BO.LocationFuncs.Distance(dr, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude }) < shortest)
                {
                    shortest = BO.LocationFuncs.Distance(dr, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude });
                    closestBase = bs[i];
                }
            }

            BO.BaseStation b = new BO.BaseStation
            {
                Location = new Location { Latitude = closestBase.Latitude, Longitude = closestBase.Longitude },
                Id = closestBase.Id,
                Name = closestBase.Name,
                NumOfFreeSlots = closestBase.NumOfSlots
            };
            List<BO.DroneCharge> dc = new();
            BO.DroneCharge droneCharge = new();
            foreach (var drone in drones)
            {
                if (drone.Location == new Location { Latitude = closestBase.Latitude, Longitude = closestBase.Longitude } && drone.Status == Enums.DroneStatuses.Maintenance)
                    droneCharge = new BO.DroneCharge { Id = drone.Id, BatteryStatus = drone.Battery };
                dc.Add(droneCharge);

            }
            b.ChargingDrones = dc;



            return b;
        }


        private DroneToList updateDroneToList(DroneToList dronetolis)
        {

            if (dronetolis.Status != Enums.DroneStatuses.InDelivery)
            {
                //puts the drone or vacant or maintenance
                dronetolis.Status = (Enums.DroneStatuses)random.Next(2);
            }
            if (dronetolis.Status == Enums.DroneStatuses.Maintenance)
            {
                List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(null);
                int rand = random.Next(bs.Count() - 1);
                Location loc = new Location { Latitude = bs[rand].Latitude, Longitude = bs[rand].Longitude };
                dronetolis.Location = loc;
                dronetolis.Battery = 20 * random.NextDouble();
                Dal.AddDroneCharge(dronetolis.Id, bs[rand].Id);

            }
            if (dronetolis.Status == Enums.DroneStatuses.Vacant)
            {
                List<DO.Customer> c = (List<DO.Customer>)Dal.GetCustomerList();
                List<DO.Parcel> parcels = (List<DO.Parcel>)Dal.GetParcelList(null);
                List<DO.Parcel> tmp = parcels.FindAll(pc => pc.Delivered < DateTime.Now && pc.Delivered != DateTime.MinValue);
                List<DO.Customer> customersTmp = new();
                foreach (var it in tmp)
                {
                    if (c.Any(cs => cs.Id == it.TargetId))
                        customersTmp.Add(c.Find(cs => cs.Id == it.TargetId));
                }
                List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(null);
                Location lc = new Location { Latitude = bs[0].Latitude, Longitude = bs[0].Longitude };
                dronetolis.Location = lc;
                dronetolis.Battery = random.Next((int)(BO.LocationFuncs.Distance(dronetolis.Location, getClosestBase(dronetolis.Location).Location) * Dal.DroneElectricConsumations()[0]), 99) + random.NextDouble();
            }
            return dronetolis;
        }

        internal BO.BaseStation FindClosestBaseStation(ILocatable loc)
        {
            var bases = from item in Dal.GetBaseStationsList(b => b.Valid == true && b.NumOfSlots > 0)
                        let boBase = dOBaseStation(item)
                        select boBase;
            double closest = 99999999;
            BO.BaseStation b = new BO.BaseStation();
            foreach (BO.BaseStation item in bases)
            {
                if (item.Distances(loc) < closest)
                {
                    closest = item.Distances(loc);
                    b = item;
                }
            }
            return b;
        }

    }
}



