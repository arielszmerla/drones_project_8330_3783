using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Runtime.CompilerServices;
using BLAPI;


namespace BL
{/// <summary>
/// part of interface BL
/// </summary>
    partial class BLImp : IBL
    {
        BO.Drone droneBODOadpater(DO.Drone busDO)
        {
            BO.Drone busBO = dODrone(busDO);
            //busDO.CopyPropertiesTo(busBO);
            return busBO;
        }
        #region gets
        /// <summary>
        /// method to get a parcel
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int idP)
        {
            DO.Parcel parcel = new();
            List<DO.Parcel> parcels = (List<DO.Parcel>)Dal.GetParcelList(null);
            if (parcels.Any(pc => pc.Id == idP))
            {
                parcel = parcels.Find(pc => pc.Id == idP);
            }
            else
                throw new GetException("id of parcel not found");
            Parcel p = new Parcel();
            List<DO.Customer> customers = (List<DO.Customer>)Dal.GetCustomerList();
            p.Assignment = parcel.Scheduled;
            p.Created = parcel.Requested;
            p.Delivered = parcel.Delivered;

            p.PickedUp = parcel.PickedUp;
            p.Priority = (Enums.Priorities)parcel.Priority;
            CustomerInParcel send = new CustomerInParcel { Id = parcel.SenderId, Name = customers.Find(cs => cs.Id == parcel.SenderId).Name };
            CustomerInParcel targ = new CustomerInParcel { Id = parcel.TargetId, Name = customers.Find(cs => cs.Id == parcel.TargetId).Name };
            p.Sender = send;
            p.Target = targ;
            p.Id = parcel.Id;
            p.WeightCategories = (Enums.WeightCategories)parcel.Weight;
            return p;
        }

        /// <summary>
        /// method to get a parcel on drone
        /// </summary>
        /// <param name="idP"></id drone>
        /// <returns></returns parcel on delivery>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDroneOnParcel(int idP)
        {
         
            DroneToList? p = drones.Find(de=>de.DeliveryId == idP);
            if (p == null)
                throw new GetException($"the drone with id: {idP} is not on delivery");
           
            return dODrone(Dal.GetDrone(p.Id));
        }
        /// <summary>
        /// method to get a parcel on drone
        /// </summary>
        /// <param name="idD"></id drone>
        /// <returns></returns parcel on delivery>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ParcelToList GetParcelToListonDrone(int idD)
        {

            DO.Parcel? p = Dal.GetParcelList(d => d.DroneId == idD && d.Requested!=null && d.Delivered == null).FirstOrDefault();
            if (p == null)
                throw new GetException($"the drone with id: {idD} is not on delivery");
            else { return DOparcelBO((DO.Parcel)p); }
        }
        /// <summary>
        /// method that returns a customer by Id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int idP) 
        {
            if (!Dal.GetCustomerList(pc => pc.Id == idP ).Any())
                throw new GetException("id of BaseStation not found");
            return DOcustomerB(Dal.GetCustomer(idP));

        }
        /// <summary>
        /// method that returns a drone by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int id) {
            if (!Dal.GetDroneList(pc => pc.Id == id && pc.Valid == true).Any())
                throw new GetException("id of BaseStation not found");
            return dODrone(Dal.GetDrone(id));
        }
        /// <summary>
        /// method that return a certain base station by id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.BaseStation GetBaseStation(int idP)
        {
            if (!Dal.GetBaseStationsList(pc => pc.Id == idP && pc.Valid == true).Any())
                throw new GetException("id of BaseStation not found");
            return dOBaseStation(Dal.GetBaseStation(idP));
        }
    

        #endregion


        #region delete
        /// <summary>
        /// parcel to delete
        /// </summary>
        /// <param name="id"></delete>
        public void DeleteParcel(int id)
        {
            if (!Dal.GetParcelList(null).Any(c => c.Id == id))
            {
                throw new DeleteException($"parcel with {id}as Id does not exist");
            }
            else if (Dal.GetParcelList(null).FirstOrDefault(c => c.Id == id).Delivered <= DateTime.Now)
            {
                throw new DeleteException($"parcel with {id}as Id has not yet been delivered");
            }
            else
                try
                {
                    Dal.DeleteParcel(id);
                }
                catch (DO.DeleteException d)
                {
                    throw new DeleteException($"parcel with {id}as Id does not exist", d);
                }
        }
        /// <summary>
        /// delete an customer
        /// </summary>
        /// <param name="id"></id of customer to erase>
        public void DeleteCustomer(int id)
        {
            if (!Dal.GetCustomerList(c => c.Id == id).Any())
            {
                throw new DeleteException($"Customer with {id}as Id does not exist");
            }
            else if (Dal.GetParcelList(c => c.TargetId == id && c.Scheduled <= DateTime.Now && c.Delivered == DateTime.MinValue).Any())
            {
                throw new DeleteException($"Customer with {id}as Id has yet more commands on way");
            }
            else
                try
                {
                    Dal.DeleteCustomer(id);
                }

                catch (DO.DeleteException d)
                {
                    throw new DeleteException($"Customer with {id}as Id does not exist", d);
                }
        }
        /// <summary>
        /// delete an Drone
        /// </summary>
        /// <param name="id"></id of Drone to erase>
        public void DeleteDrone(int id)
        {
            if (!Dal.GetDroneList(c => c.Id == id).Any())
            {
                throw new DeleteException($"Drone with {id}as Id does not exist");
            }
            else if (Dal.GetParcelList(c => c.DroneId == id && c.Scheduled <= DateTime.Now && c.Delivered == DateTime.MinValue).Any())
            {
                throw new DeleteException($"Drone with {id}is on  delivery");
            }
            try
            {
                Dal.DeleteDrone(id);
                drones.RemoveAll(pc => pc.Id == id);
            }
            catch (DO.DeleteException d)
            {
                throw new DeleteException($"Drone with {id}as Id does not exist", d);
            }
        }
        /// <summary>
        /// delete an BaseStation
        /// </summary>
        /// <param name="id"></id of BaseStation to erase>
        public void DeleteBasestation(int id)
        {
            if (!Dal.GetBaseStationsList(null).Any(c => c.Id == id))
            {
                throw new DeleteException($"BaseStation with {id}as Id does not exist");
            }
            else
                if (drones.Any(dr => dr.Location == new Location { Latitude = Dal.GetBaseStation(id).Latitude, Longitude = Dal.GetBaseStation(id).Longitude } && dr.Status == Enums.DroneStatuses.Maintenance))
                throw new DeleteException($"BaseStation with {id}as Id is in use");
            else
                try
                {
                    Dal.DeleteBasestation(id);
                    drones.RemoveAll(pc => pc.Id == id);
                }
                catch (DO.DeleteException d)
                {
                    throw new DeleteException($"BaseStation with {id}as Id does not exist", d);
                }
        }
        #endregion
        #region getlists
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicat"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStationToList> GetBaseStationList(Func<BaseStationToList, bool> predicat = null)
        {
            try
            {
                if (predicat == null)
                {
                    IEnumerable<BaseStationToList> b = (from item in Dal.GetBaseStationsList(null)
                                                        where item.Valid == true
                                                        let DObaseStationBO = adaptBaseStationToList(item)
                                                        select DObaseStationBO);

                    if (b == null)
                        throw new GetException("empty list");
                    return b.ToList();
                }
                else
                {
                    IEnumerable<BaseStationToList> b = (from item in Dal.GetBaseStationsList(null)
                                                        where item.Valid == true
                                                        let DObaseStationBO = adaptBaseStationToList(item)
                                                        where predicat(DObaseStationBO)
                                                        select DObaseStationBO);

                    if (b == null)
                        throw new GetException("empty list");
                    return b.ToList();
                }
            }
            catch (DO.BaseExeption ex)
            {
                throw new GetException("empty list", ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicat"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStationToList> GetBaseStationListGroup()
        {
            IEnumerable<BaseStationToList> b = (from item in Dal.GetBaseStationsList(null)
                                                where item.Valid == true
                                                let DObaseStationB = adaptBaseStationToList(item)
                                                select DObaseStationB);

            IEnumerable<IGrouping<bool, BaseStationToList>> e = from item in b
                                                                where item.Valid == true
                                                                group item by item.NumOfSlotsInUse == 0
                        into j
                                                                select j;
            List<BaseStationToList> ToReturn = new();
            foreach (IGrouping<bool, BaseStationToList> mashehu in e)
                switch (mashehu.Key)
                {
                    case true:
                        foreach (BaseStationToList bs in mashehu)
                            ToReturn.Add(bs);
                        break;
                    case false:
                        foreach (BaseStationToList bs in mashehu)
                            ToReturn.Add(bs);
                        break;
                }
            return ToReturn;
        }
        /// <summary>
        /// returns two groups one with base stations with free slots and one without
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<IGrouping<bool, BaseStationToList>> GetWithWithoutFreeSlotsBaseStationgroup()
        {
            return (from item in Dal.GetBaseStationsList(null)
                    where item.Valid == true
                    let DObaseStationBo = adaptBaseStationToList(item)
                    group DObaseStationBo by DObaseStationBo.NumOfFreeSlots == 0 into gs
                    select gs);

        }

        /// <summary>
        /// / return list of Base mapped by free slots
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStationToList> GetListOfBaseStationsWithFreeSlots()
        {
            try
            {
                List<BaseStationToList> tmp = (List<BaseStationToList>)GetBaseStationList();
                if (tmp.FindAll(bs => bs.NumOfFreeSlots > 0 && bs.Valid == true).Count == 0) ;
                throw new GetException("empty list");
                return tmp.FindAll(bs => bs.NumOfFreeSlots > 0 && bs.Valid == true);
            }
            catch (DO.BaseExeption ex)
            {
                throw new GetException("empty list", ex);
            }
        }
        /// <summary>
        /// / return list of parcels mapped by status
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelList(Enums.WeightCategories? statuses = null)
        {
            try
            {
                if (statuses == null)
                {
                    IEnumerable<ParcelToList> p = (from item in Dal.GetParcelList(null)
                                                   let parcelBO = DOparcelBO(item)
                                                   select parcelBO);
                    if (p == null)
                        throw new GetException("empty list");
                    return p.ToList();
                }
                Enums.WeightCategories weight = (Enums.WeightCategories)statuses;
                IEnumerable<ParcelToList> b = (from item in Dal.GetParcelList(null)
                                               let parcelBO = DOparcelBO(item)
                                               where parcelBO.WeightCategorie == weight
                                               select parcelBO);
                if (b == null)
                    throw new GetException("empty list");
                return b.ToList();
            }
            catch (DO.ParcelExeption ex)
            {
                throw new GetException("empty list", ex);
            }

        }
        /// <summary>
        /// return list of parcels
        /// </summary>
        /// <param name="name"></name of sender/ target>
        /// <returns></list of parcel>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelList(string name)
        {
            try
            {
                if (name == "")
                {
                    IEnumerable<ParcelToList> b = (from item in Dal.GetParcelList(null)
                                                   let parcelBO = DOparcelBO(item)
                                                   select parcelBO);
                    if (b == null)
                        throw new GetException("empty list");
                    return b.ToList();
                }

                IEnumerable<ParcelToList> p = (from item in Dal.GetParcelList(null)
                                               let parcelBO = DOparcelBO(item)
                                               where parcelBO.SenderName == name || parcelBO.TargetName == name
                                               select parcelBO);
                if (p == null)
                    throw new GetException("empty list");
                return p.ToList();
            }

            catch (DO.ParcelExeption ex)
            {
                throw new GetException("empty list", ex);
            }
        }
        /// <summary>
        /// returns list on non assigned parcels
        /// </summary>
        /// <returns></list of non assigned parcel>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelNotAssignedList()
        {
            try
            {
                IEnumerable<ParcelToList> p = (from item in Dal.GetParcelList(it => it.DroneId == 0)
                                               let parcelBO = DOparcelBO(item)
                                               select parcelBO
                        );

                if (p == null)
                    throw new GetException("empty list");
                return p.ToList();
            }
            catch (DO.ParcelExeption ex)
            {
                throw new GetException("empty list", ex);
            }
        }

        public IEnumerable<DroneToList> GetDronesInBaseStationList(int Id)
        {
            Location BaseLoc = new Location
            {
                Latitude = Dal.GetBaseStation(Id).Latitude,
                Longitude = Dal.GetBaseStation(Id).Longitude
            };
            return (from item in drones
                    where item.Location.Latitude == BaseLoc.Latitude &&
                    item.Location.Longitude == BaseLoc.Longitude &&
                    item.Status == Enums.DroneStatuses.Maintenance
                    && item.Valid == true
                    select item);
        }

        /// <summary>
        /// get dronelist filtered by status or weight categ
        /// </summary>
        /// <param name="statuses"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> GetDroneList(Enums.DroneStatuses? statuses = null, Enums.WeightCategories? weight = null)
        {
            if (statuses == null && weight == null)
                return drones.ToList();
            else if (statuses != null && weight == null)
            {
                return drones.Where(d => d.Status == statuses && d.Valid == true);
            }
            else if (statuses != null && weight != null)
            {
                return drones.Where(d => d.Status == statuses && d.MaxWeight == weight && d.Valid == true);
            }
            return drones.Where(d => d.MaxWeight == weight && d.Valid == true);

        }
        /// <summary>
        /// RETURN ienumerable of customerToList members
        /// </summary>
        /// <param name="predicat"></condition>
        /// <returns></list mapped>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerToList> GetCustomerList(Func<CustomerToList, bool> predicat = null)
        {
            try
            {
                IEnumerable<CustomerToList> c = from item in Dal.GetCustomerList()
                                                let customerBO = DOcustomerToListBO(item)
                                                where predicat == null ? true : predicat(customerBO)
                                                select customerBO;

                if (c.Count() == 0)
                    throw new GetException("empty list");
                return c;
            }
            catch (DO.CostumerExeption ex)
            {
                throw new GetException("empty list", ex);
            }

        }
        #endregion

    }
}
