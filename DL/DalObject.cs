using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DLAPI;
using DO;
using DS;


namespace DalObject
{
    sealed partial class DalObject : IDal
    {
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly DalObject instance = new DalObject();
        }

        private static object syncRoot = new object();
        public static DalObject Instance
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

        private DalObject()
        {
            DataSource.Config.Initialize();
        }

        #region electricity
        /// <summary>
        /// return list of consumation data
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] DroneElectricConsumations()
        {
            double[] returnedArray ={ DataSource.Config.powerUseFreeDrone, DataSource.Config.powerUseLightCarrying,
                DataSource.Config.powerUseMediumCarrying, DataSource.Config.powerUseHeavyCarrying,
             DataSource.Config.chargePerHour };
            return returnedArray;
        }
        #endregion

        #region dronecharge
        /// <summary>
        /// method to add a dronecharge unit.
        /// </summary>
        /// <param "id drone, id parcel"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDroneCharge(int idDrone, int idBase)
        {
            BaseStation? myBase = null;
            myBase = DataSource.BaseStations.Find(bs => bs.Id == idBase);
            if (myBase == null)
                throw new BaseExeption("id of base not found");
            Drone? myDrone = null;
            myDrone = DataSource.Drones.Find(dr => dr.Id == idBase);
            if (myDrone == null)
                throw new DroneException("id of drone not found");
            if (!DataSource.DroneCharges.Any(d => d.DroneId == idDrone))
                DataSource.DroneCharges.Add(new DroneCharge { DroneId = idDrone, StationId = idBase });

        }
        /// <summary>
        /// method to delete a dronecharge unit.
        /// </summary>
        /// <param "id drone></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDroneCharge(int idDrone)
        {
            DroneCharge? myDrone = null;
            myDrone = DataSource.DroneCharges.Find(dr => dr.DroneId == idDrone);
            if (myDrone == null)
                throw new DroneChargeException("id of drone not found");
            DataSource.DroneCharges.RemoveAll(d => d.DroneId == idDrone);
            BaseStation b = DataSource.BaseStations.Find(bs => bs.Id == myDrone.Value.StationId);
            DataSource.BaseStations.Remove(b);
            b.NumOfSlots++;
            DataSource.BaseStations.Add(b);
        }
        /// <summary>
        /// method to release a charging drone from a base station
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateReleasDroneCharge(int idD, string baseName)
        {
            // first we search for the drone requested
            if (!DataSource.Drones.Any(dr => dr.Id == idD))
                throw new DroneException("id of drone not found");
            if (!DataSource.BaseStations.Any(b => b.Name == baseName))
                throw new BaseExeption("id of base station not found");

            BaseStation basest = DataSource.BaseStations.Where(b => b.Name == baseName).FirstOrDefault();
            basest.NumOfSlots++;
            DataSource.BaseStations.RemoveAll(b => b.Name == baseName);
            DataSource.BaseStations.Add(basest);

            for (int i = 0; i < DataSource.DroneCharges.Count; i++)
            {
                if (DataSource.DroneCharges[i].DroneId == idD)
                {
                    DataSource.DroneCharges.RemoveAt(i);
                }
            }
        }

        #endregion

        #region BaseStation
        /// <summary>
        /// gets basestation from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns the basestation got>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BaseStation GetBaseStation(int id)
        {
            BaseStation? myBase = null;
            myBase = DataSource.BaseStations.Find(bs => bs.Id == id);
            if (myBase == null)
                throw new BaseExeption("id of base not found");
            return (BaseStation)myBase;
        }
        /// <summary>
        /// send a new base to database
        /// </summary>
        /// <param name="baseStation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddBaseStation(BaseStation baseStation)
        {
            baseStation.Valid = true;
            if (DataSource.BaseStations.Any(bs => bs.Id == baseStation.Id))
            {
                throw new BaseExeption("id allready exist");
            }
            else
            {
                baseStation.Valid = true;
                DataSource.BaseStations.Add(baseStation);
            }
        }
        /// <summary>
        /// get list of base stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicat)
        {
            if (predicat == null)
                if (DataSource.BaseStations.Select(item => item).ToList().Count == 0)
                    throw new BaseExeption("empty list");
                else return DataSource.BaseStations.Select(item => item).ToList();
            else
            {
                IEnumerable<BaseStation> b = (from item in DataSource.BaseStations
                                              where predicat(item)
                                              select item);
                if (b == null)
                    throw new BaseExeption("empty list");
                return b.ToList();
            }

        }
        /// <summary>
        /// update in dal a basestation
        /// </summary>
        /// <param name="bs"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateBaseStationFromBl(BaseStation bs)
        {

            BaseStation? b = DataSource.BaseStations.FirstOrDefault(ba => ba.Id == bs.Id);//.Clone();
            if (b == null)
            {
                throw new BaseExeption($"base station {bs.Id} not found\n");
            }
            DataSource.BaseStations.Remove((BaseStation)b);
            DataSource.BaseStations.Add(bs);
        }

        /// <summary>
        /// method to delete a basestation
        /// </summary>
        /// <param name="id"></param>
       [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteBasestation(int id)
        {
            if (!DataSource.BaseStations.Any(p => p.Id == id))
                throw new DeleteException($"BaseStation with {id}as Id does not exist");
            BaseStation b = DataSource.BaseStations.FirstOrDefault(p => p.Id == id);
            b.Valid = false;
            DataSource.BaseStations.RemoveAll(p => p.Id == id);
            try
            {
                DataSource.BaseStations.Add(b);
            }
            catch (DO.BaseExeption ex) { throw new BaseExeption("id allready exist"); }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> GetDroneCharges(int idBase)
        {
          var b =  DataSource.DroneCharges.Where(b => b.StationId == idBase);
            if (b != null)
                return b;
            throw new DroneChargeException("empty list");
        }
        #endregion

        #region Parcel

        /// <summary>
        /// add a parcel and return running number
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddParcel(Parcel parcel)
        {
            if (DataSource.Parcels.Any(pr => pr.Id == parcel.Id))
            {
                throw new ParcelExeption($"id { parcel.Id} already exist");
            }
            DataSource.Parcels.Add(parcel);
            DataSource.Config.totalNumOfParcels++;
            return DataSource.Config.totalNumOfParcels;
        }
        /// <summary>
        /// gets parcel from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the parcel got >

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int id)
        {
            Parcel? myParcel = DataSource.Parcels.Find(p => p.Id == id);
            if (myParcel == null)
                throw new ParcelExeption("id of parcel not found");
            return (Parcel)myParcel.Clone();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcelToDrone(int idP, int idD)
        {
            if (!DataSource.Parcels.Any(ps => ps.Id == idP))
            {
                throw new ParcelExeption($" Id {idP} of parcel not found\n");
            }
            if (DataSource.Drones.TrueForAll(dr => dr.Id != idD))
            {
                throw new DroneException($" Id {idD} of drone not found\n");
            }
            int i = DataSource.Parcels.FindIndex(ps => ps.Id == idP);
            Parcel myParcel = DataSource.Parcels.Find(ps => ps.Id == idP);
            myParcel.DroneId = idD;
            myParcel.Requested = DateTime.Now;
            DataSource.Parcels[i] = myParcel;

        }
        /// <summary>
        /// method that sets the delivery time 
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatesParcelDelivery(int id)
        {
            int k = DataSource.Parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption($"invalid parcel id {id}");
            Parcel tmp = DataSource.Parcels[k];
            tmp.Delivered = DateTime.Now;
            tmp.DroneId = 0;
            DataSource.Parcels[k] = tmp;
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate)
        {
            if (predicate == null)
                return DataSource.Parcels.Select(item => item).ToList();
            else
                return (from item in DataSource.Parcels
                        where predicate(item)
                        select item).ToList();

        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetDroneChargeBaseStationId(int droneId) {
       DroneCharge? d=DataSource.DroneCharges.Find(dc => dc.DroneId == droneId);
            if(d==null)
          throw new DroneChargeException($"Drone is not being charged {droneId}");
            return d.Value.StationId;

        }
        /// <summary>
        /// method to update a parcel
        /// </summary>
        /// <param name="p"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(Parcel p)
        {

            int index = DataSource.Parcels.FindIndex(pc => pc.Id == p.Id);
            if (index == -1)
                throw new ParcelExeption($"the parcel {p.Id} doesn't exists");
            DataSource.Parcels[index] = p;
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            if (!DataSource.Parcels.Any(p => p.Id == id))
                throw new ParcelExeption($"parcel with {id}as Id does not exist");
            DataSource.Parcels.RemoveAll(p => p.Id == id);
        }
        #endregion

        #region Drone
        /// <summary>
        /// update drone frome bl to data source
        /// </summary>
        /// <param name="dr"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone dr)
        {
            int index = DataSource.Drones.FindIndex(drone => drone.Id == dr.Id);
            DataSource.Drones[index] = dr;
        }
        /// <summary>
        /// send a new drone to database
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone drone)
        {
            if (DataSource.Drones.Any(dr => dr.Id == drone.Id))
            {
                throw new DroneException($"id {drone.Id} allready exist");
            }
            DataSource.Drones.Add(drone);
        }
        /// <summary>
        /// gets drone from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></drone>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int id)
        {
            Drone? myDrone = null;

            myDrone = DataSource.Drones.Find(dr => dr.Id == id);
            if (myDrone == null)
                throw new DroneException("id of drone not found");
            if (DataSource.Drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {id} as Id is already deleted");
            return (Drone)myDrone;
        }

        /// <summary>
        /// to set a time for when the drone pick's up a packet
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDronePickUp(int id)
        {

            if (!DataSource.Drones.Any(dr => dr.Id == id))
                throw new DroneException($"invalid drone id {id}");
            int k = DataSource.Parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = DataSource.Parcels[k];
            tmp.PickedUp = DateTime.Now;
            DataSource.Parcels[k] = tmp;
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate)
        {

            IEnumerable<Drone> d = (from item in DataSource.Drones
                                    where predicate == null ? true : predicate(item) && item.Valid == true
                                    select item);
            if (d == null)
                throw new DroneException("empty list");
            else return d.ToList();
        }
      
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            if (!DataSource.Drones.Any(p => p.Id == id))
                throw new DeleteException($"drone with {id}as Id does not exist");
            if (DataSource.Drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                throw new DeleteException($"Drone with {id} as Id is alredy deleted");
            DataSource.Drones.RemoveAll(p => p.Id == id);

        }
        /// <summary>
        /// send a drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneToCharge(int idD, string baseName)
        {
            if (!DataSource.Drones.Any(dr => dr.Id == idD))
                throw new DroneException("id of drone not found");
            if (!DataSource.BaseStations.Any(b => b.Name == baseName))
                throw new BaseExeption("id of base station not found");
            if (DataSource.BaseStations.Where(b => b.Name == baseName).FirstOrDefault().NumOfSlots == 0)
                throw new BaseExeption("base station already full");
            DroneCharge dc = new DroneCharge();
            dc.DroneId = idD;
            dc.StationId = DataSource.BaseStations.Where(b => b.Name == baseName).FirstOrDefault().Id;
            DataSource.DroneCharges.Add(dc);
        }
        #endregion

        #region Customer
        /// <summary>
        /// send a new customer to database
        /// </summary>
        /// <param name="customer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer customer)
        {
            if (DataSource.Customers.Any(cos => cos.Id == customer.Id))
            {
                throw new CostumerExeption("id allready exist");
            }
            DataSource.Customers.Add(customer);
        }
        /// <summary>
        /// gets customer from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the customer got>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int id)
        {
            Customer? costumer = null;

            costumer = DataSource.Customers.Find(cs => cs.Id == id);
            if (costumer == null)
                throw new CostumerExeption($"id {id}of customer not found");
            return (Customer)costumer;
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
        {

            IEnumerable<Customer> c = (from item in DataSource.Customers
                                       where predicate == null ? true : predicate(item)
                                       select item);
            if (c == null)
            {
                throw new CostumerExeption("empty list");
            }
            return c.ToList();
        }


        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            if (!DataSource.Customers.Any(p => p.Id == id))
                throw new DeleteException($"Customer with {id}as Id does not exist");
            DataSource.Customers.RemoveAll(p => p.Id == id);
        }


        /// <summary>
        /// update a customer from bl to data saurce
        /// </summary>
        /// <param name="customer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerInfoFromBL(Customer customer)
        {
            int index = DataSource.Customers.FindIndex(cs => cs.Id == customer.Id);
            if (index == -1)
            {
                throw new CostumerExeption("customer do not exist");
            }
            DataSource.Customers[index] = customer;
        }
        #endregion

    }
}
