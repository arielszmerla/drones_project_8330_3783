using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using BLAPI;
namespace BL
{
    /// <summary>
    /// implementation of private funcs 
    /// </summary>
    partial class BLImp : IBL
    {
        /// <summary>
        /// func that search if any parcel with a certain weigth category is set to be sent by a certain drone 
        /// and checks if drone has enough battery to get to it
        /// </summary>
        /// <param name="dr">drone</param>
        /// <param name="parcels">parcel</param>
        /// <param name="weight">weightmax</param>
        /// <returns>flag if there is a parcel left</returns>
        private bool searchParcel(DroneToList dr, List<DO.Parcel> parcels, int weight)
        {
            //find "to be sent" parcels
            lock (Dal)
            {
                List<DO.Parcel> maxWeightParcels = parcels.FindAll(pcs => (int)pcs.Weight == weight && pcs.Scheduled == DateTime.MinValue);
                if (maxWeightParcels.Count > 0)
                {//checks if the drone can reach them
                    DO.Parcel p = finfClosestParcelToDrone(dr.Location, maxWeightParcels);

                    DO.Customer cs = Dal.GetCustomerList().First(cs => cs.Id == p.TargetId);
                    if (dr.Battery > BatteryCons(dr.Location, p)
                        + LocationFuncs.Distance(dr.Location, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }) * Dal.DroneElectricConsumations()[0])
                    {
                        return true;

                    }
                }
                return false;
            }
        }
        /// <summary>
        /// return drone Location
        /// </summary>
        /// <param name="drone"> drone sent</param>
        /// <returns>drones location</returns>
        private Location findDroneLocation(DroneToList drone)
        {
            lock (Dal)
            {
                int? parcelId = drone.DeliveryId;

                switch (drone.Status)
                {
                    // if the drone is charging we return the location if the base station at which he is charging
                    case Enums.DroneStatuses.Maintenance:
                        return Dal.GetBaseStation(Dal.GetDroneChargeBaseStationId(drone.Id)).Location();
                    // if the drone is in a delivery we check a few conditions
                    case Enums.DroneStatuses.InDelivery:

                        DO.Parcel parcel = Dal.GetParcel((int)parcelId);
                        // if the parcel was picked up 
                        if (parcel.PickedUp > DateTime.Now)
                        {
                            Customer customer = GetCustomer(parcel.SenderId);
                            return FindClosestBaseStation(customer).Location;
                        }
                        if (parcel.Delivered > DateTime.Now)
                        {
                            return GetCustomer(parcel.SenderId).Location;
                        }
                        return Dal.GetCustomer(parcel.TargetId).Location();
                        //if drone is vacant 
                    case Enums.DroneStatuses.Vacant:
                        //if a parcel remain so go to sender of one of those or basestarion randomly
                        if (parcelId != null)
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
        }

        /// <summary>
        /// look for and return is any parcel is actually on the drone
        /// </summary>
        /// <param name="dr">drone</param>
        /// <returns>parcel in delivery</returns>
        private ParcelInDelivery? findParcelOnDrone(Drone dr)
        {
            lock (Dal)
            {
                //find all the parcels on this drone
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
                    pid.Distance = LocationFuncs.Distance(pid.Location, pid.TargetLocation);
                    return (ParcelInDelivery)pid;
                }
                //no parcel found
                else return null;
            }
        }
        /// <summary>
        /// find a parcel that can be reache by drone (assuming it exits...)
        /// </summary>
        /// <param name="dr">drone</param>
        /// <param name="parcels">parcel</param>
        /// <param name="weight">max weihjt</param>
        /// <returns></returns>
        [Obsolete("not in use, replaced by findparcelondrone")]
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
        /// <returns>parcel</returns>
        private DO.Parcel finfClosestParcelToDrone(Location dronePlace, List<DO.Parcel> parcels)
        {
            lock (Dal)
            {
                double tmpLength = 40000000000000;
                DO.Parcel tmpParcel = new();
                //check all the parcels 
                for (int i = 0; i < parcels.Count; i++)
                {
                    Location lc = new();
                    lc.Latitude = Dal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Latitude;
                    lc.Longitude = Dal.GetCustomerList().First(cs => cs.Id == parcels[i].SenderId).Longitude;
                    //check closest
                    if (LocationFuncs.Distance(dronePlace, lc) < tmpLength)
                    {
                        tmpLength = LocationFuncs.Distance(dronePlace, lc);
                        tmpParcel = parcels[i];
                    }
                }
                return tmpParcel;
            }
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
            total += LocationFuncs.Distance(lc, new Location { Latitude = cs.Latitude, Longitude = cs.Longitude });
            List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(null);
            Location closestBase = new();
            //look after closest baseStation
            for (int i = 0; i < bs.Count(); i++)
            {
                if (LocationFuncs.Distance(new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude }) < temp)
                {
                    temp = LocationFuncs.Distance(new Location { Latitude = cs.Latitude, Longitude = cs.Longitude }, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude });
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
        private List<DroneCharge> dronCharges(BaseStation bs)
        {//find the relevant drones

            List<DroneCharge> droneCharges = new();
            IEnumerable<DroneToList> dr = drones.FindAll(dr => dr.Status == Enums.DroneStatuses.Maintenance && dr.Location.Latitude == bs.Location.Latitude);
            foreach (var item in dr)
            {
                DroneCharge d = new DroneCharge { Id = item.Id, BatteryStatus = item.Battery };
                droneCharges.Add(d);
            }
            return droneCharges;

        }

        /// <summary>
        /// Gets the closest baseStation to a certain location
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private BaseStation getClosestBase(Location dr)
        {
            //get closest base with frre slots
            List<DO.BaseStation> bs = (List<DO.BaseStation>)Dal.GetBaseStationsList(bs => bs.NumOfSlots > 0);
            if (bs.Count() == 0)
                throw new GetException("No place for charge");
            DO.BaseStation closestBase = new();
            double shortest = 99999999999999990;
            //find closest
            int i = 0;
            for (; i < bs.Count(); i++)
            {
                if (bs[i].NumOfSlots > 0 && LocationFuncs.Distance(dr, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude }) < shortest)
                {
                    shortest = LocationFuncs.Distance(dr, new Location { Latitude = bs[i].Latitude, Longitude = bs[i].Longitude });
                    closestBase = bs[i];
                }
            }
            //create the base with the BO elements
            BaseStation b = new BaseStation
            {
                Location = new Location { Latitude = closestBase.Latitude, Longitude = closestBase.Longitude },
                Id = closestBase.Id,
                Name = closestBase.Name,
                NumOfFreeSlots = closestBase.NumOfSlots
            };
            b.ChargingDrones = (from drone in drones
                                where drone.Location == new Location { Latitude = closestBase.Latitude, Longitude = closestBase.Longitude } && drone.Status == Enums.DroneStatuses.Maintenance
                                select new DroneCharge { Id = drone.Id, BatteryStatus = drone.Battery }).ToList();
            return b;
        }

        /// <summary>
        /// update drone to list place and status
        /// </summary>
        /// <param name="dronetolis"> droneto list to update</param>
        /// <returns > updated one</returns>
        [Obsolete("not in use")]
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
                try
                {
                    dronetolis.Battery = random.Next((int)(LocationFuncs.Distance(dronetolis.Location, getClosestBase(dronetolis.Location).Location) * Dal.DroneElectricConsumations()[0]), 99) + random.NextDouble();
                }
                catch (GetException e) { throw e; }
            }
            return dronetolis;
        }
        /// <summary>
        /// find closest base with free slots
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        private BaseStation FindClosestBaseStation(ILocatable loc)
        {
            lock (Dal)
            {  //get list of those
                var bases = from item in Dal.GetBaseStationsList(b => b.Valid == true && b.NumOfSlots > 0)
                            let boBase = dOBaseStation(item)
                            select boBase;
                double closest = 99999999;
                BaseStation b = new BaseStation();
                //find closest
                foreach (BaseStation item in bases)
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
}
