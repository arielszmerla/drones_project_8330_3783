using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLAPI;
using DO;
using DS;


namespace DalObject
{
    /// <summary>
    /// 
    /// </summary>
     partial class DalObject : IDal
    {
        public void UpdateDrone(Drone dr)
        {
            int index = DataSource.Drones.FindIndex(drone => drone.Id == dr.Id);
            DataSource.Drones[index] = dr.Clone();
        }
        /// <summary>
        /// send a new drone to database
        /// </summary>
        /// <param name="drone"></param>
        public void AddDrone(Drone drone)
        {
            if (DataSource.Drones.Any(dr => dr.Id == drone.Id))
            {
                throw new DroneException($"id {drone.Id} allready exist");
            }
            DataSource.Drones.Add(drone.Clone());
        }
        /// <summary>
        /// gets drone from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></drone>
        public Drone GetDrone(int id)
        {
            Drone? myDrone = null;

            myDrone = DataSource.Drones.Find(dr => dr.Id == id);
            if (myDrone == null)
                throw new DroneException("id of drone not found");
            return (Drone)myDrone.Clone();
        }

        /// <summary>
        /// to set a time for when the drone pick's up a packet
        /// </summary>
        /// <param name="id"></param>
        public void UpdateDronePickUp(int id)
        {
    
            if (!DataSource.Drones.Any(dr => dr.Id == id))
                throw new DroneException($"invalid drone id {id}");
            int k = DataSource.Parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new DLAPI.ParcelExeption("invalid parcel id");
            Parcel tmp = DataSource.Parcels[k];
            tmp.PickedUp = DateTime.Now;
            DataSource.Parcels[k] = tmp.Clone();
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetDroneList(Func<Drone, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.Drones.ToList();
            else
                return (from item in DataSource.Drones
                        where predicate(item)
                        select item.Clone());
        }

        public double[] DroneElectricConsumations()
        {
            double[] returnedArray ={ DataSource.Config.powerUseFreeDrone, DataSource.Config.powerUseLightCarrying,
                DataSource.Config.powerUseMediumCarrying, DataSource.Config.powerUseHeavyCarrying,
             DataSource.Config.chargePerHour };
            return returnedArray;
        }

    }
}
