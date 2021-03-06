using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DLAPI;
using DO;
using DS;

namespace DalXML
{
    /// <summary>
    /// data base implemention via xml
    /// </summary>
    sealed public partial class DalXML : IDal
    {
        #region  paths
        /// <summary>
        /// consts as paths
        /// </summary>
        const string DRONEPATH = @"drones.xml";
        const string PARCELPATH = @"parcels.xml";
        const string BASESTATIONPATH = @"stations.xml";
        const string CUSTOMERPATH = @"customer.xml";
        const string DRONECHARGEPATH = @"droneCharge.xml";
        #endregion
        #region singelton
        /// <summary>
        /// makes sure only one user can approch the code.
        /// </summary>
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
        public DalXML()
        {
            //empty charging drone list
            releaseDroneFromCharge();
        }
        #endregion

        #region customers
        /// <summary>
        /// method to add a customer
        /// </summary>
        /// <param name="customer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(Customer customer)
        {
            var cus = XMLTools.LoadListFromXMLSerializer<Customer>(CUSTOMERPATH);
            if (cus.Any(cos => cos.Id == customer.Id))
            {
                throw new CostumerExeption("id already exist");
            }
            cus.Add(customer);
            XMLTools.SaveListToXMLSerializer(cus, CUSTOMERPATH);
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            var customers = XMLTools.LoadListFromXMLSerializer<Customer>(CUSTOMERPATH);
            if (!customers.Any(cos => cos.Id == id))
            {
                throw new DO.CostumerExeption($"Customer with {id} as Id does not exist");
            }
            customers.RemoveAll(p => p.Id == id);
            XMLTools.SaveListToXMLSerializer(customers, CUSTOMERPATH);
        }

        /// <summary>
        /// gets customer from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the customer got>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int id)
        {
            var customers = XMLTools.LoadListFromXMLSerializer<Customer>(CUSTOMERPATH);
            if (!customers.Any(customer => customer.Id == id))
            {
                throw new CostumerExeption($"Customer with {id} as Id does not exist");
            }
            return customers.Find(cus => cus.Id == id);
        }

        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
        {
            var customers = XMLTools.LoadListFromXMLSerializer<Customer>(CUSTOMERPATH);

            IEnumerable<Customer> d = (from item in customers
                                       where predicate == null ? true : predicate(item)
                                       select item);
            if (d == null)
                throw new DroneException("empty list");
            return d.ToList();
        }


        /// <summary>
        /// update a customer from bl to data saurce
        /// </summary>
        /// <param name="customer"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerInfoFromBL(Customer customer)
        {
            List<Customer> customers = XMLTools.LoadListFromXMLSerializer<Customer>(CUSTOMERPATH);
            if (!(customers.Any(cus => cus.Id == customer.Id)))
            {
                throw new CostumerExeption($"Customer with {customer.Id} as Id does not exist");
            }
            int index = customers.FindIndex(cus => cus.Id == customer.Id);
            customers[index] = customer;
            XMLTools.SaveListToXMLSerializer(customers, CUSTOMERPATH);

        }
        #endregion

        #region electricity
        ///    /// <summary>
        /// return list of consumation data
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] DroneElectricConsumations()
        {
            var returnedArrays = XMLTools.LoadListFromXMLSerializer<double>(@"configs.xml");
            return returnedArrays.ToArray();
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

            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            Drone? myDrone = null;
            myDrone = drones.Where(dr => dr.Id == idBase).FirstOrDefault();
            if (myDrone == null)
                throw new DroneException("id of drone not found");

            var drones1 = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
            if (!drones1.Any(c => c.DroneId == idDrone))
                drones1.Add(new DroneCharge { DroneId = idDrone, StationId = idBase });
            XMLTools.SaveListToXMLSerializer(drones1, DRONECHARGEPATH);
            baseStationDroneIn(idBase);
        }
        /// <summary>
        /// release drone from charge
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void releaseDroneFromCharge()
        {
            var drones1 = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
            if (drones1.Count() > 0)
            {
                foreach (var unit in drones1)//
                    baseStationDroneOut(unit.StationId);
            }
            drones1.Clear();
            XMLTools.SaveListToXMLSerializer(drones1, DRONECHARGEPATH);
        }
        /// <summary>
        /// func that update basestation free slots when drone comes in
        /// </summary>
        /// <param name="baseStationId"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void baseStationDroneIn(int baseStationId)
        {
            XElement baseStations = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            XElement baseStation = (from bs in baseStations.Elements()
                                    where bs.Element("id").Value == $"{baseStationId}"
                                    select bs).FirstOrDefault();
            int availableChargingPorts = Convert.ToInt32(baseStation.Element("numOfSlots").Value);
            --availableChargingPorts;
            baseStation.Element("numOfSlots").Value = availableChargingPorts.ToString();
            XMLTools.SaveListToXMLElement(baseStations, BASESTATIONPATH);
        }
        /// <summary>
        /// func that update basestation free slots when drone goes out
        /// </summary>
        /// <param name="baseStationId"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void baseStationDroneOut(int baseStationId)
        {
            XElement baseStations = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            XElement baseStation = (from bs in baseStations.Elements()
                                    where bs.Element("id").Value == $"{baseStationId}"
                                    select bs).FirstOrDefault();
            int availableChargingPorts = Convert.ToInt32(baseStation.Element("numOfSlots").Value);
            ++availableChargingPorts;
            baseStation.Element("numOfSlots").Value = availableChargingPorts.ToString();

            XMLTools.SaveListToXMLElement(baseStations, BASESTATIONPATH);
        }
        /// <summary>
        /// return the drone in charge
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetDroneChargeBaseStationId(int droneId)
        {
            XElement DroneCharges = XMLTools.LoadListFromXMLElement(DRONECHARGEPATH);
            return (from dc in DroneCharges.Elements()
                    where dc.Element("DroneId").Value == $"{droneId}"
                    select Convert.ToInt32(dc.Element("StationId").Value))
                .FirstOrDefault();
        }
        /// <summary>
        /// method to delete a dronecharge unit.
        /// </summary>
        /// <param "id drone></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDroneCharge(int idDrone)
        {
            //check if drone and charge unit exists
            DroneCharge? myDrone = null;
            List<DroneCharge> dronesCharge = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
            myDrone = dronesCharge.Where(dr => dr.DroneId == idDrone).FirstOrDefault();
            if (myDrone == null)
                throw new DroneChargeException("id of drone not found");
            //take oout from data
            baseStationDroneOut(myDrone.Value.StationId);
            dronesCharge.RemoveAll(d => d.DroneId == idDrone);
            XMLTools.SaveListToXMLSerializer(dronesCharge, DRONECHARGEPATH);
        }
        #endregion

        #region drones
        /// <summary>
        /// send a new drone to database
        /// </summary>
        /// <param name="drone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone drone)
        {
            List<Drone> drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            if (drones.Any(dr => dr.Id == drone.Id))
                throw new DroneException("id already exist");
            drones.Add(drone);
            XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            if (!drones.Any(cos => cos.Id == id))
                throw new DroneException($"Drone with {id} as Id does not exist");
            if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {id} as Id is alredy deleted");
            drones.RemoveAll(p => p.Id == id);
            XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
        }
        /// <summary>
        /// gets drone from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></drone>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int id)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            if (!drones.Any(cos => cos.Id == id))
                throw new DroneException($"Drone with {id} as Id does not exist");
            if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {id} as Id is already deleted");
            return drones.Find(dr => dr.Id == id);
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);

           var d = (from item in drones
                                    where predicate == null ? true : predicate(item) && item.Valid == true
                                    select item);
            if (d == null)
                throw new DroneException("empty list");
            return d.ToList();
        }
        /// <summary>
        /// update drone frome bl to data source
        /// </summary>
        /// <param name="dr"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone dr)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            if (!drones.Any(drone => drone.Id == dr.Id))
            {
                throw new DroneException($"Drone with {dr.Id} as Id does not exist");
            }
            if (drones.Where(d => d.Id == dr.Id).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {dr.Id} as Id is already deleted");
            drones.RemoveAll(d => d.Id == dr.Id);
            drones.Add(dr);
            XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
        }
        /// <summary>
        /// to set a time for when the drone pick's up a packet
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDronePickUp(int id)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
            if (!drones.Any(cos => cos.Id == id))
            {
                throw new DroneException($"Drone with {id} as Id does not exist");
            }
            if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {id} as Id is already deleted");
            int k = parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = parcels[k];
            tmp.PickedUp = DateTime.Now;
            parcels[k] = tmp;
            XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
            XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
        }
        /// <summary>
        /// to set a parcel to pickup
        /// </summary>
        /// <param name="id"></param>
        public void ParcelPickup(int parcelId)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
            var parcel = (from p in parcels
                          where p.Id == parcelId
                          select p).FirstOrDefault();


            int k = parcels.FindIndex(ps => ps.Id == parcelId);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = parcels[k];
            tmp.PickedUp = DateTime.Now;
            parcels[k] = tmp;

            XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
        }
        /// <summary>
        /// schedule a parcel to a drone
        /// </summary>
        /// <param name="parcelId"> id of the drone</param>
        /// <param name="droneId">id of the parcel</param>
        public void ParcelSchedule(int parcelId, int droneId)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
            var parcel = (from p in parcels
                          where p.Id == parcelId
                          select p).FirstOrDefault();
            int k = parcels.FindIndex(ps => ps.Id == parcelId);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = parcels[k];
            tmp.DroneId = droneId;
            tmp.Scheduled = DateTime.Now;
            parcels[k] = tmp;

            XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
        }
        /// <summary>
        /// to set a parcel to pickup
        /// </summary>
        /// <param name="id"> parcel id</param>
        public void ParcelDelivery(int parcelId)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
            var parcel = (from p in parcels
                          where p.Id == parcelId
                          select p).FirstOrDefault();


            int k = parcels.FindIndex(ps => ps.Id == parcelId);
            if (k == -1)
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = parcels[k];
            tmp.Delivered = DateTime.Now;

            parcels[k] = tmp;

            XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
        }
        /// <summary>
        /// send a drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneToCharge(int idD, string baseName)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(PARCELPATH);
            var baseStations = XMLTools.LoadListFromXMLSerializer<BaseStation>(BASESTATIONPATH);
            var droneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
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
            if (bs.Value.NumOfSlots == 0)
                throw new BaseExeption("base station already full");
            BaseStation b = (BaseStation)bs;
            b.NumOfSlots--;
            baseStations.RemoveAll(b => b.Id == bs.Value.Id);
            baseStations.Add(b);
            droneCharges.Add(new DroneCharge { DroneId = dr.Value.Id, StationId = b.Id });
            XMLTools.SaveListToXMLSerializer(droneCharges, DRONECHARGEPATH);
            XMLTools.SaveListToXMLSerializer(baseStations, BASESTATIONPATH);
        }
        /// <summary>
        /// method to release a charging drone from a base station
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateReleasDroneCharge(int idD, string baseName)
        {
            var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
            var baseStations = XMLTools.LoadListFromXMLSerializer<BaseStation>(BASESTATIONPATH);
            var droneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
            Drone? dr = (from dro in drones
                         where dro.Valid == true && dro.Id == idD
                         select dro).FirstOrDefault();
            BaseStation? bs = (from bas in baseStations
                               where bas.Valid == true && bas.Name == baseName
                               select bas).FirstOrDefault();
            DroneCharge? charge = (from item in droneCharges
                                   where item.DroneId == idD && item.StationId == bs.Value.Id
                                   select item
                                  ).FirstOrDefault();
            if (dr == null)
                throw new DroneException("id of drone not found");
            if (bs == null)
                throw new BaseExeption("id of base station not found");
            if (charge == null)
                throw new DroneChargeException("no such drone charging actually");
            BaseStation basest = (BaseStation)bs;
            basest.NumOfSlots++;
            baseStations.RemoveAll(b => b.Name == baseName);
            baseStations.Add(basest);
            droneCharges.RemoveAll(b => b.DroneId == idD && b.StationId == basest.Id);
            XMLTools.SaveListToXMLSerializer(droneCharges, DRONECHARGEPATH);
            XMLTools.SaveListToXMLSerializer(baseStations, BASESTATIONPATH);
        }

        #endregion

        #region Parcel
        /// <summary>
        /// send a new parcel to database
        /// </summary>
        /// <param name="parcel"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddParcel(Parcel parcel)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            if (parcels.Any(pa => pa.Id == parcel.Id))
            {
                throw new ParcelExeption("id already exist");
            }
            parcels.Add(parcel);
            XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
            //need to be implemented on configs.xml
            var vs = XMLTools.LoadListFromXMLSerializer<double>(@"configs.xml");
            vs[5]++;
            XMLTools.SaveListToXMLSerializer(vs, @"configs.xml");
            return 1;
        }

        /// <summary>
        /// gets parcel from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the parcel got >
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int id)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            if (!parcels.Any(pa => pa.Id == id))
            {
                throw new ParcelExeption($"Parcel with {id} as Id does not exist");
            }
            return parcels.Find(dr => dr.Id == id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcelToDrone(int idP, int idD)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            List<Drone> drones = XMLTools.LoadListFromXMLSerializer<Drone>(@"drones.xml");
            if (!parcels.Any(pa => pa.Id == idP))
            {
                throw new ParcelExeption($"Parcel with {idP} as Id does not exist");
            }
            if (!drones.Any(d => d.Id == idD))
            {
                throw new DroneException($" Id {idD} of drone not found\n");
            }
            if (drones.Where(d => d.Id == idD).FirstOrDefault().Valid == false)
                throw new DroneException($"Drone with {idD} as Id is already deleted");
            int i = parcels.FindIndex(ps => ps.Id == idP);
            Parcel myParcel = parcels.Find(ps => ps.Id == idP);
            myParcel.DroneId = idD;
            myParcel.Requested = DateTime.Now;

            parcels[i] = myParcel;
            XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
        }
        /// <summary>
        /// method that sets the delivery time 
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatesParcelDelivery(int id)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            int k = parcels.FindIndex(ps => ps.DroneId == id);
            if (k == -1)
                throw new ParcelExeption($"invalid parcel id {id}");
            Parcel tmp = parcels[k];
            tmp.Delivered = DateTime.Now;
            tmp.DroneId = 0;

            parcels[k] = tmp;
            XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");

            IEnumerable<Parcel> p = (from item in parcels
                                     where predicate == null ? true : predicate(item)
                                     select item);
            if (p == null)
                throw new ParcelExeption("empty list");
            return p.ToList();
        }


        /// <summary>
        /// update a parcel
        /// </summary>
        /// <param name="p"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(Parcel p)
        {
            List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            int index = parcels.FindIndex(pc => pc.Id == p.Id);
            if (index == -1)
                throw new ParcelExeption($"the parcel {p.Id} doesn't exists");
            parcels[index] = p;

            XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
            if (!parcels.Any(p => p.Id == id))
                throw new ParcelExeption($"parcel with {id}as Id does not exist");
            parcels.RemoveAll(p => p.Id == id);
            XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
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
            XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            BaseStation? bs;
            try
            {
                bs = (from basestation in baseRoot.Elements()
                      where (basestation.Element("valid").Value) == "true"
                      where int.Parse(basestation.Element("id").Value) == id
                      select new BaseStation()
                      {
                          Id = int.Parse(basestation.Element("id").Value),
                          Latitude = double.Parse(basestation.Element("latitude").Value),
                          Longitude = double.Parse(basestation.Element("longitude").Value),
                          Name = basestation.Element("name").Value,
                          NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                          Valid = bool.Parse(basestation.Element("valid").Value)
                      }).FirstOrDefault();
            }
            catch
            {
                bs = null;
            }
            if (bs == null)
                throw new BaseExeption("id of base not found or deleted");
            return (BaseStation)bs;
        }

        /// <summary>
        /// send a new base to database by XElement
        /// </summary>
        /// <param name="baseStation"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddBaseStation(BaseStation baseStation)
        {
            XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            BaseStation? bs;
            try
            {
                bs = (from basestation in baseRoot.Elements()
                      where int.Parse(basestation.Element("id").Value) == baseStation.Id
                      select new BaseStation()
                      {
                          Id = int.Parse(basestation.Element("id").Value),
                          Latitude = double.Parse(basestation.Element("latitude").Value),
                          Longitude = double.Parse(basestation.Element("longitude").Value),
                          Name = basestation.Element("name").Value,
                          NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                          Valid = bool.Parse(basestation.Element("valid").Value)
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
                XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
            }
            throw new BaseExeption("id already exists");

        }
        /// <summary>
        /// get list of base stations
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicat)
        {
            XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            IEnumerable<BaseStation> b = from bas in baseRoot.Elements()

                                         let da = new BaseStation
                                         {
                                             Id = int.Parse(bas.Element("id").Value),
                                             Latitude = double.Parse(bas.Element("latitude").Value),
                                             Longitude = double.Parse(bas.Element("longitude").Value),
                                             Name = bas.Element("name").Value,
                                             NumOfSlots = int.Parse(bas.Element("numOfSlots").Value),
                                             Valid = bool.Parse(bas.Element("valid").Value)
                                         }
                                         where predicat == null ? true : predicat(da)
                                     && da.Valid == true
                                         select da;
            if (b == null)
            {
                throw new CostumerExeption("empty list");
            }
            return b.ToList();

        }
        /// <summary>
        /// update in dal a basestation
        /// </summary>
        /// <param name="bs"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateBaseStationFromBl(BaseStation bs)
        {
            XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            BaseStation? bas;
            // first we search for old base station in the list
            try
            {
                bas = (from basestation in baseRoot.Elements()
                       where (basestation.Element("valid").Value) == "true"
                       where int.Parse(basestation.Element("id").Value) == bs.Id
                       select new BaseStation()
                       {
                           Id = int.Parse(basestation.Element("id").Value),
                           Latitude = double.Parse(basestation.Element("latitude").Value),
                           Longitude = double.Parse(basestation.Element("longitude").Value),
                           Name = basestation.Element("name").Value,
                           NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                           Valid = bool.Parse(basestation.Element("valid").Value)
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
                                 where (basestation.Element("valid").Value) == "true"
                                 where int.Parse(basestation.Element("id").Value) == bs.Id
                                 select basestation).FirstOrDefault();
            xElement.Remove();

            // then we add updated base station to the list and update the file
            XElement Id = new XElement("id", bs.Id);
            XElement Latitude = new XElement("latitude", bs.Latitude);
            XElement Longitude = new XElement("longitude", bs.Longitude);
            XElement Name = new XElement("name", bs.Name);
            XElement NumOfSlots = new XElement("numOfSlots", bs.NumOfSlots);
            XElement Valid = new XElement("valid", bs.Valid);
            baseRoot.Add(new XElement("baseStation", Id, Name, NumOfSlots, Latitude, Longitude, Valid));
            XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
        }
        /// <summary>
        /// return the drones charging on a base
        /// </summary>
        /// <param name="idBase"></param>
        /// <returns>list of drones</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> GetDroneCharges(int idBase = 0)
        {
            if (idBase == 0)
                return XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
            else
                return XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH).Where(b => b.StationId == idBase);
        }
        /// <summary>
        /// delete base station XElement
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteBasestation(int id)
        {
            XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
            BaseStation? bas;
            // first we search for old base station in the list
            try
            {
                bas = (from basestation in baseRoot.Elements()
                       where (basestation.Element("valid").Value) == "true"
                       where int.Parse(basestation.Element("id").Value) == id
                       select new BaseStation()
                       {
                           Id = int.Parse(basestation.Element("id").Value),
                           Latitude = double.Parse(basestation.Element("latitude").Value),
                           Longitude = double.Parse(basestation.Element("longitude").Value),
                           Name = basestation.Element("name").Value,
                           NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                           Valid = bool.Parse(basestation.Element("valid").Value)
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
                                 where (basestation.Element("valid").Value) == "true"
                                 where int.Parse(basestation.Element("id").Value) == id
                                 select basestation).FirstOrDefault();
            xElement.Element("valid").Value = "false";
            XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
        }
        #endregion
    }

}

