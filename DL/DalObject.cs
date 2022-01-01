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
        #region BaseStation


        /// <summary>
        /// gets basestation from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns the basestation got>
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
        public IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicat)
        {
            if (predicat == null)
                return DataSource.BaseStations.Select(item => item).ToList();
            else
                return (from item in DataSource.BaseStations
                        where predicat(item)
                        select item);

        }
        /// <summary>
        /// update in dal a basestation
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateBaseStationFromBl(BaseStation bs)
        {
            
          BaseStation? b = DataSource.BaseStations.FirstOrDefault(ba => ba.Id == bs.Id);//.Clone();
            if ( b== null)
            {
                throw new BaseExeption($"base station {bs.Id} not found\n");
            }
            DataSource.BaseStations.Remove((BaseStation) b);
            DataSource.BaseStations.Add(bs);
        }
        public void DeleteBasestation(int id)
        {
            if (!DataSource.BaseStations.Any(p => p.Id == id))
                throw new DLAPI.DeleteException($"BaseStation with {id}as Id does not exist");
            BaseStation b = DataSource.BaseStations.FirstOrDefault(p => p.Id == id);
            b.Valid = false;
            DataSource.BaseStations.RemoveAll(p => p.Id == id);
            try
            {
                DataSource.BaseStations.Add(b);
            }
            catch (DO.BaseExeption ex) { throw new BaseExeption("id allready exist"); }
        }





        #endregion

        #region Parcel
        public int AddParcel(Parcel parcel)
        {
            if (DataSource.Parcels.Any(pr => pr.Id == parcel.Id))
            {
                throw new DLAPI.ParcelExeption($"id { parcel.Id} already exist");
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
        public IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate)
        {
            if (predicate == null)
                return DataSource.Parcels.Select(item=>item).ToList();
            else
                return (from item in DataSource.Parcels
                        where predicate(item)
                        select item);

        }

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
        public void DeleteParcel(int id)
        {
            if (!DataSource.Parcels.Any(p => p.Id == id))
                throw new DLAPI.DeleteException($"parcel with {id}as Id does not exist");
            DataSource.Parcels.RemoveAll(p => p.Id == id);
        }

     
        #endregion

        #region Drone
        /// <summary>
        /// update drone frome bl to data source
        /// </summary>
        /// <param name="dr"></param>
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
        public IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate)
        {
            if (predicate == null)
                return DataSource.Drones.Select(item => item).ToList();
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
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteDrone(int id)
        {
            if (!DataSource.Drones.Any(p => p.Id == id))
                throw new DLAPI.DeleteException($"drone with {id}as Id does not exist");
            DataSource.Drones.RemoveAll(p => p.Id == id);

        }
        #endregion

        #region Customer
        /// <summary>
        /// send a new customer to database
        /// </summary>
        /// <param name="customer"></param>
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
        public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
        {

            if (predicate == null)
                return DataSource.Customers.Select(item => item).ToList();
            else
                return (from item in DataSource.Customers
                        where predicate(item)
                        select item);

        }



        /// <summary>
        /// update a customer from bl to data saurce
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateCustomerInfoFromBL(Customer customer)
        {
            int index = DataSource.Customers.FindIndex(cs => cs.Id == customer.Id);
            DataSource.Customers[index] = customer;
        }
        #endregion
        /// <summary>
        /// method to release a charging drone from a base station
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        public void UpdateReleasDroneCharge(int idD, string baseName)
        {
            // first we search for the drone requested
            for (int k = 0; k < DataSource.Drones.Count; k++)
            {
                if (DataSource.Drones[k].Id == idD)
                {
                    // next 'for' iterarion to find base station
                    for (int i = 0; i < DataSource.BaseStations.Count; i++)
                    {
                        if (DataSource.BaseStations[i].Name == baseName)
                        {
                            // after finding both base station and id we do the folowing
                            // 1) make room for a new drone to charge in the base station
                            // 2) change the drone status tp vacant
                            // 3) remove the drone charge from the droneCharge list
                            BaseStation Base = DataSource.BaseStations[i];
                            Base.NumOfSlots++;
                            DataSource.BaseStations[i] = Base;
                            /*Drone temp = DataSource.Drones[k];
                            temp.Status = DroneStatuses.Vacant;
                            DataSource.Drones[k] = temp;*/
                            for (int h = 0; h < DataSource.DroneCharges.Count; h++)
                            {
                                if (DataSource.DroneCharges[i].DroneId == idD)
                                {
                                    DataSource.DroneCharges.RemoveAt(i);
                                }
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// send a drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        public void UpdateDroneToCharge(int idD, string baseName)
        {

            for (int k = 0; k < DataSource.Drones.Count(); k++)
            {
                if (DataSource.Drones[k].Id == idD)
                {
                    for (int i = 0; i < DataSource.BaseStations.Count; i++)//look for the right base
                    {
                        BaseStation Base = DataSource.BaseStations[i];
                        if (Base.Name == baseName)//make the asked changes and create anobject of Drone to charge
                        {
                            // after finding base and drone we do the following
                            // set the drone status to be maintenance
                            // set a slot in dronecharge with the drone and base stations ID;
                           
                            /* Drone temp = DataSource.Drones[k];
                             temp.Status = DroneStatuses.Maintenance;
                             DataSource.Drones[k] = temp;*/
                            DroneCharge dc = new DroneCharge();
                            dc.DroneId = idD;
                            dc.StationId = DataSource.BaseStations[i].Id; DataSource.DroneCharges.Add(dc);
                        }
                    }
                }
            }
        }
    
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteCustomer(int id)
        {
            if (!DataSource.Customers.Any(p => p.Id == id))
                throw new DLAPI.DeleteException($"Customer with {id}as Id does not exist");
            DataSource.Customers.RemoveAll(p => p.Id == id);
        }



    }
}
