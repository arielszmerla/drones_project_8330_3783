using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DLAPI;
using DO;
using DS;

namespace DalXML
{

    sealed partial class DalXML : IDal
    {
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly DalXML instance = new DalXML();
        }

        private static object syncRoot = new object();
        public static DalXML Instance
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


        #region customers
        /// <summary>
        /// method to add a customer
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            List<Customer> cus = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (cus.Any(cos => cos.Id == customer.Id))
            {
                throw new CostumerExeption("id already exist");
            }
            cus.Add(customer);

            XMLTolls.SaveListToXMLSerializer(cus, @"customer.xml");

        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteCustomer(int id)
        {
            List<Customer> customers = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (!customers.Any(cos => cos.Id == id))
            {
                throw new DLAPI.DeleteException($"Customer with {id} as Id does not exist");
            }
            customers.RemoveAll(p => p.Id == id);
            XMLTolls.SaveListToXMLSerializer(customers, @"customer.xml");
        }

        /// <summary>
        /// gets customer from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the customer got>
        public Customer GetCustomer(int id)
        {
            List<Customer> customers = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (!(customers.Any(customer => customer.Id == id)))
            {
                throw new DLAPI.CostumerExeption($"Customer with {id} as Id does not exist");
            }
            return customers.Find(cus => cus.Id == id);
        }

        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
        {
            List<Customer> customers = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (predicate == null)
                return customers;
            else
                return (from item in customers
                        where predicate(item)
                        select item);
        }


        /// <summary>
        /// update a customer from bl to data saurce
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateCustomerInfoFromBL(Customer customer)
        {
            List<Customer> customers = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (!(customers.Any(cus => cus.Id == customer.Id)))
            {
                throw new DLAPI.CostumerExeption($"Customer with {customer.Id} as Id does not exist");
            }
            int index = customers.FindIndex(cus => cus.Id == customer.Id);
            customers[index] = customer;
            XMLTolls.SaveListToXMLSerializer(customers, @"customer.xml");

        }
        #endregion

        #region drones
        /// <summary>
        /// send a new drone to database
        /// </summary>
        /// <param name="drone"></param>
        public void AddDrone(Drone drone)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (drones.Any(dr => dr.Id == drone.Id))
            {
                throw new DroneException("id already exist");
            }
            drones.Add(drone);
            XMLTolls.SaveListToXMLSerializer(drones, @"drone.xml");
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteDrone(int id)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (!drones.Any(cos => cos.Id == id))
            {
                throw new DLAPI.DeleteException($"Drone with {id} as Id does not exist");
            }
            drones.RemoveAll(p => p.Id == id);
            XMLTolls.SaveListToXMLSerializer(drones, @"drone.xml");

        }

        public double[] DroneElectricConsumations()
        {
            double[] returnedArray ={ DataSource.Config.powerUseFreeDrone, DataSource.Config.powerUseLightCarrying,
                DataSource.Config.powerUseMediumCarrying, DataSource.Config.powerUseHeavyCarrying,
             DataSource.Config.chargePerHour };
            return returnedArray;
        }
        /// <summary>
        /// gets drone from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></drone>
        public Drone GetDrone(int id)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (!drones.Any(cos => cos.Id == id))
            {
                throw new DroneException($"Drone with {id} as Id does not exist");
            }
            return drones.Find(dr => dr.Id == id);

        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (predicate == null)
                return drones;
            else
                return (from item in drones
                        where predicate(item)
                        select item);
        }
        /// <summary>
        /// update drone frome bl to data source
        /// </summary>
        /// <param name="dr"></param>
        public void UpdateDrone(Drone dr)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (!drones.Any(drone => drone.Id == dr.Id))
            {
                throw new DLAPI.DeleteException($"Drone with {dr.Id} as Id does not exist");
            }
            int index = drones.FindIndex(drone => drone.Id == dr.Id);
            drones[index] = dr;
            XMLTolls.SaveListToXMLSerializer(drones, @"drone.xml");
        }
        /// <summary>
        /// to set a time for when the drone pick's up a packet
        /// </summary>
        /// <param name="id"></param>
        public void UpdateDronePickUp(int id)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            if (!drones.Any(cos => cos.Id == id))
            {
                throw new DLAPI.DeleteException($"Drone with {id} as Id does not exist");
            }

            int k = parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = parcels[k];
            tmp.PickedUp = DateTime.Now;
            parcels[k] = tmp;
            XMLTolls.SaveListToXMLSerializer(drones, @"drone.xml");
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
        }
        /// <summary>
        /// send a drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        public void UpdateDroneToCharge(int idD, string baseName)
        {
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            List<BaseStation> baseStations = XMLTolls.LoadListFromXMLSerializer<BaseStation>(@"stations.xml");
            Drone? dr;
            dr = (from dro in drones
                  where dro.Valid == true && dro.Id == idD
                  select dro).FirstOrDefault();
            BaseStation? bs;
            bs = (from bas in baseStations
                  where bas.Valid == true && bas.Name == baseName
                  select bas).FirstOrDefault();
            if (dr == null)
                throw new DroneException("id of drone not found");
            if (bs == null)
                throw new BaseExeption("id of base station not found");
            //dr.Value
            // need to finish this function
        }
        #endregion


        #region Parcel
        public int AddParcel(Parcel parcel)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            if (parcels.Any(pa => pa.Id == parcel.Id))
            {
                throw new ParcelExeption("id already exist");
            }
            parcels.Add(parcel);
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
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
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            if (!parcels.Any(pa => pa.Id == id))
            {
                throw new ParcelExeption($"Drone with {id} as Id does not exist");
            }
            return parcels.Find(dr => dr.Id == id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void UpdateParcelToDrone(int idP, int idD)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            List<Drone> drones = XMLTolls.LoadListFromXMLSerializer<Drone>(@"drone.xml");
            if (!parcels.Any(pa => pa.Id == idP))
            {
                throw new ParcelExeption($"Parcel with {idP} as Id does not exist");
            }
            if (DataSource.Drones.TrueForAll(dr => dr.Id != idD))
            {
                throw new DroneException($" Id {idD} of drone not found\n");
            }
            int i = parcels.FindIndex(ps => ps.Id == idP);
            Parcel myParcel = parcels.Find(ps => ps.Id == idP);
            myParcel.DroneId = idD;
            myParcel.Requested = DateTime.Now;
            parcels[i] = myParcel;
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
        }
        /// <summary>
        /// method that sets the delivery time 
        /// </summary>
        /// <param name="id"></param>
        public void UpdatesParcelDelivery(int id)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            int k = parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption($"invalid parcel id {id}");
            Parcel tmp = parcels[k];
            tmp.Delivered = DateTime.Now;
            tmp.DroneId = 0;
            parcels[k] = tmp;
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            if (predicate == null)
                return parcels;
            else
                return (from item in parcels
                        where predicate(item)
                        select item);
        }

        public void UpdateParcel(Parcel p)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            int index = parcels.FindIndex(pc => pc.Id == p.Id);
            if (index == -1)
                throw new ParcelExeption($"the parcel {p.Id} doesn't exists");
            parcels[index] = p;
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteParcel(int id)
        {
            List<Parcel> parcels = XMLTolls.LoadListFromXMLSerializer<Parcel>(@"parcel.xml");
            if (!parcels.Any(p => p.Id == id))
                throw new DLAPI.DeleteException($"parcel with {id}as Id does not exist");
            parcels.RemoveAll(p => p.Id == id);
            XMLTolls.SaveListToXMLSerializer(parcels, @"parcel.xml");
        }
        #endregion


        #region BaseStation


        /// <summary>
        /// gets basestation from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns the basestation got>
        public BaseStation GetBaseStation(int id)
        {
            XElement baseRoot = XMLTolls.LoadListFromXMLElement(@"BaseStations");
            BaseStation? bs;
            try
            {
                bs = (from basestation in baseRoot.Elements()
                      where bool.Parse(basestation.Element("Valid").Value) == true
                      where int.Parse(basestation.Element("Id").Value) == id
                      select new BaseStation()
                      {
                          Id = int.Parse(basestation.Element("Id").Value),
                          Latitude = double.Parse(basestation.Element("Latitude").Value),
                          Longitude = double.Parse(basestation.Element("Longitude").Value),
                          Name = basestation.Element("Name").Value,
                          NumOfSlots = int.Parse(basestation.Element("NumOfSlots").Value),
                          Valid = bool.Parse(basestation.Element("Valid").Value)
                      }).FirstOrDefault();
            }
            catch
            {
                bs = null;
            }
            if (bs == null)
                throw new BaseExeption("id of base not found");

            return (BaseStation)bs;
        }

        /// <summary>
        /// send a new base to database by XElement
        /// </summary>
        /// <param name="baseStation"></param>
        public void AddBaseStation(BaseStation baseStation)
        {
            XElement baseRoot = XMLTolls.LoadListFromXMLElement(@"BaseStations");
            BaseStation? bs;
            try
            {
                bs = (from basestation in baseRoot.Elements()
                      where int.Parse(basestation.Element("Id").Value) == baseStation.Id
                      select new BaseStation()
                      {
                          Id = int.Parse(basestation.Element("Id").Value),
                          Latitude = double.Parse(basestation.Element("Latitude").Value),
                          Longitude = double.Parse(basestation.Element("Longitude").Value),
                          Name = basestation.Element("Name").Value,
                          NumOfSlots = int.Parse(basestation.Element("NumOfSlots").Value),
                          Valid = bool.Parse(basestation.Element("Valid").Value)
                      }).FirstOrDefault();
            }
            catch
            {
                bs = null;
            }
            if (bs == null)
            {
                XElement Id = new XElement("Id", baseStation.Id);
                XElement Latitude = new XElement("Latitude", baseStation.Latitude);
                XElement Longitude = new XElement("Longitude", baseStation.Longitude);
                XElement Name = new XElement("Name", baseStation.Name);
                XElement NumOfSlots = new XElement("NumOfSlots", baseStation.NumOfSlots);
                XElement Valid = new XElement("Valid", baseStation.Valid);
                baseRoot.Add(new XElement("baseStation", Id, Name, NumOfSlots, Latitude, Longitude, Valid));
                XMLTolls.SaveListToXMLElement(baseRoot, @"BaseStations");
            }
            throw new BaseExeption("id already exists");

        }
        /// <summary>
        /// get list of base stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicat)
        {
            XElement baseRoot = XMLTolls.LoadListFromXMLElement(@"BaseStations");
         
            if (predicat == null)
                return (from bas in baseRoot.Elements()
                        where bool.Parse(bas.Element("Valid").Value) == true
                        select new BaseStation()
                        {
                            Id = int.Parse(bas.Element("Id").Value),
                            Latitude = double.Parse(bas.Element("Latitude").Value),
                            Longitude = double.Parse(bas.Element("Longitude").Value),
                            Name = bas.Element("Name").Value,
                            NumOfSlots = int.Parse(bas.Element("NumOfSlots").Value),
                            Valid = bool.Parse(bas.Element("Valid").Value)
                        }).ToList();

           else
                return (from bas in baseRoot.Elements()
                        where bool.Parse(bas.Element("Valid").Value) == true
                        let baseStation = new BaseStation()
                        {
                            Id = int.Parse(bas.Element("Id").Value),
                            Latitude = double.Parse(bas.Element("Latitude").Value),
                            Longitude = double.Parse(bas.Element("Longitude").Value),
                            Name = bas.Element("Name").Value,
                            NumOfSlots = int.Parse(bas.Element("NumOfSlots").Value),
                            Valid = bool.Parse(bas.Element("Valid").Value)
                        }
                        where predicat(baseStation)
                        select baseStation);
        }
        /// <summary>
        /// update in dal a basestation
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateBaseStationFromBl(BaseStation bs)
        {
            XElement baseRoot = XMLTolls.LoadListFromXMLElement(@"BaseStations");
            BaseStation? bas;
            // first we search for old base station in the list
            try
            {
                bas = (from basestation in baseRoot.Elements()
                       where bool.Parse(basestation.Element("Valid").Value) == true
                       where int.Parse(basestation.Element("Id").Value) == bs.Id
                       select new BaseStation()
                       {
                           Id = int.Parse(basestation.Element("Id").Value),
                           Latitude = double.Parse(basestation.Element("Latitude").Value),
                           Longitude = double.Parse(basestation.Element("Longitude").Value),
                           Name = basestation.Element("Name").Value,
                           NumOfSlots = int.Parse(basestation.Element("NumOfSlots").Value),
                           Valid = bool.Parse(basestation.Element("Valid").Value)
                       }).FirstOrDefault();
            }
            catch
            {
                bas = null;
            }
            // if the base station doesn't exist we throw exception
            if (bas == null)
                throw new BaseExeption("id of base not found");
            // we delete old base station with same id from the list
            XElement xElement = (from basestation in baseRoot.Elements()
                                 where bool.Parse(basestation.Element("Valid").Value) == true
                                 where int.Parse(basestation.Element("Id").Value) == bs.Id
                                 select basestation).FirstOrDefault();
            xElement.Remove();
      
            // then we add updated base station to the list and update the file
            XElement Id = new XElement("Id", bs.Id);
            XElement Latitude = new XElement("Latitude", bs.Latitude);
            XElement Longitude = new XElement("Longitude", bs.Longitude);
            XElement Name = new XElement("Name", bs.Name);
            XElement NumOfSlots = new XElement("NumOfSlots", bs.NumOfSlots);
            XElement Valid = new XElement("Valid", bs.Valid);
            baseRoot.Add(new XElement("baseStation", Id, Name, NumOfSlots, Latitude, Longitude, Valid));
            XMLTolls.SaveListToXMLElement(baseRoot, @"BaseStations");
          
        }




        /// <summary>
        /// delete base station XElement
        /// </summary>
        /// <param name="id"></param>
        public void DeleteBasestation(int id)
        {
            XElement baseRoot = XMLTolls.LoadListFromXMLElement(@"BaseStations");
            BaseStation? bas;
            // first we search for old base station in the list
            try
            {
                bas = (from basestation in baseRoot.Elements()
                       where bool.Parse(basestation.Element("Valid").Value) == true
                       where int.Parse(basestation.Element("Id").Value) == id
                       select new BaseStation()
                       {
                           Id = int.Parse(basestation.Element("Id").Value),
                           Latitude = double.Parse(basestation.Element("Latitude").Value),
                           Longitude = double.Parse(basestation.Element("Longitude").Value),
                           Name = basestation.Element("Name").Value,
                           NumOfSlots = int.Parse(basestation.Element("NumOfSlots").Value),
                           Valid = bool.Parse(basestation.Element("Valid").Value)
                       }).FirstOrDefault();
            }
            catch
            {
                bas = null;
            }
            // if the base station doesn't exist we throw exception
            if (bas == null)
                throw new BaseExeption("id of base not found");
            // we delete old base station with same id from the list
            XElement xElement = (from basestation in baseRoot.Elements()
                                 where bool.Parse(basestation.Element("Valid").Value) == true
                                 where int.Parse(basestation.Element("Id").Value) == id
                                 select basestation).FirstOrDefault();
           xElement.Element("Valid").Value = false.ToString();
           XMLTolls.SaveListToXMLElement(baseRoot, @"BaseStations");
        }





        #endregion
    }

}
