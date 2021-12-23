using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using BLAPI;
namespace BL
{/// <summary>
/// part of interface
/// </summary>
    partial class BLImp : IBL
    {

        #region
        /// <summary>
        /// method to get a parcel
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        public Parcel GetParcel(int idP)
        {
            DO.Parcel parcel = new();
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            if (parcels.Any(pc => pc.Id == idP))
            {
                parcel = parcels.Find(pc => pc.Id == idP);
            }
            else
                throw new GetException("id of parcel not found");
            Parcel p = new Parcel();
            List<DO.Customer> customers = (List<DO.Customer>)myDal.GetCustomerList();
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
        /// method that return a certain base station by id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        public BaseStation GetBaseStation(int idP)
        {
            DO.BaseStation myBase = new();

            if (!myDal.GetBaseStationsList(null).Any(pc => pc.Id == idP))
            {
                throw new GetException("id of BaseStation not found");
            }
            BaseStation bs = new();
            bs.BaseStationLocation = new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude };

            bs.Id = myBase.Id;
            bs.Name = myBase.Name;
            bs.ChargingDrones = dronCharges(bs);
            bs.NumOfFreeSlots = myBase.NumOfSlots - bs.ChargingDrones.Count;
            return bs;
        }
        public Customer GetCustomer(int idP)
        {

            DO.Customer myCust = new();
            IEnumerable<DO.Customer> customers =myDal.GetCustomerList(c=>c.Id==idP);
            if (customers.Any(pc => pc.Id == idP))
            {
                myCust = customers.FirstOrDefault(pc => pc.Id == idP);
            }
            else
                throw new GetException("id of Customer not found");
            Customer customer = new Customer();
            customer.Id = myCust.Id;

            customer.Location = new Location { Latitude = myCust.Latitude, Longitude = myCust.Longitude };
            customer.Name = myCust.Name;
            customer.Phone = myCust.Phone;
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            List<DO.Parcel> parcelTo = parcels.FindAll(ps => ps.TargetId == customer.Id);
            List<DO.Parcel> parcelFrom = parcels.FindAll(ps => ps.SenderId == customer.Id);
            List<ParcelByCustomer> customerTmp = new();
            foreach (var it in parcelFrom)
            {
                ParcelByCustomer tmp = new();
                tmp.Id = it.Id;
                tmp.Priorities = (Enums.Priorities)it.Priority;
                if (it.Delivered != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
                else if (it.PickedUp != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
                else if (it.Requested != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.Assigned;
                else
                    tmp.ParcelStatus = Enums.ParcelStatus.Created;
                tmp.CIP = new CustomerInParcel { Id = it.SenderId, Name = myDal.GetCustomer(it.SenderId).Name };
              
                tmp.WeightCategorie = (Enums.WeightCategories)it.Weight;
                customerTmp.Add(tmp);
            }
            customer.To = customerTmp;
            customerTmp = new();
            foreach (var it in parcelTo)
            {
                ParcelByCustomer tmp = new();
                tmp.Id = it.Id;
                tmp.Priorities = (Enums.Priorities)it.Priority;
                if (it.Delivered != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
                else if (it.PickedUp != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
                else if (it.Requested != DateTime.MinValue)
                    tmp.ParcelStatus = Enums.ParcelStatus.Assigned;
                else
                    tmp.ParcelStatus = Enums.ParcelStatus.Created;
          
                tmp.CIP=new CustomerInParcel { Id= it.TargetId , Name = myDal.GetCustomer(it.TargetId).Name
            };
                tmp.CIP.Id = it.TargetId;
                tmp.CIP.Name = customers.FirstOrDefault(pc => pc.Id == it.TargetId).Name;
                tmp.WeightCategorie = (Enums.WeightCategories)it.Weight;
                customerTmp.Add(tmp);
            }
            customer.From = customerTmp;
            return customer;
        }
        public Drone GetDrone(int id)
        {
            DroneToList? myDrone = null;
            if (drones.Any(pc => pc.Id == id))
            {
                myDrone = drones.Find(pc => pc.Id == id);
            }
            if (myDrone == null)
                throw new GetException("id of drone not found");
            Drone dr = new Drone
            {
                Id = myDrone.Id,
                BatteryStatus = myDrone.BatteryStatus,
                DronePlace = myDrone.DroneLocation,
                MaxWeight = myDrone.MaxWeight,
                Model = myDrone.Model,
                PID = null,
                Status = myDrone.Status
            };
            if (findParcelOnDrone(dr) != null && dr.Status == Enums.DroneStatuses.InDelivery)

                dr.PID = findParcelOnDrone(dr);
            return dr;
        }
        #endregion


        #region delete
        /// <summary>
        /// parcel to delete
        /// </summary>
        /// <param name="id"></delete>
        public void DeleteParcel(int id)
        {
            if (!myDal.GetParcelList(null).Any(c => c.Id == id))
            {
                throw new DeleteException($"parcel with {id}as Id does not exist");
            }
            else if (myDal.GetParcelList(null).FirstOrDefault(c => c.Id == id).Delivered <= DateTime.Now)
            {
                throw new DeleteException($"parcel with {id}as Id has not yet been delivered");
            }
            else
                try
                {
                    myDal.DeleteParcel(id);
                }
                catch (DLAPI.DeleteException d)
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
            if (!myDal.GetCustomerList(c => c.Id == id).Any())
            {
                throw new DeleteException($"Customer with {id}as Id does not exist");
            }
            else if (myDal.GetParcelList(c => c.TargetId == id && c.Scheduled <= DateTime.Now && c.Delivered == DateTime.MinValue).Any())
            {
                throw new DeleteException($"Customer with {id}as Id has yet more commands on way");
            }
            else
                try
                {
                    myDal.DeleteCustomer(id);
                }


                catch (DLAPI.DeleteException d)
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
            if (!myDal.GetDroneList(c => c.Id == id).Any())
            {
                throw new DeleteException($"Drone with {id}as Id does not exist");
            }
            else if (myDal.GetParcelList(c => c.DroneId == id && c.Scheduled <= DateTime.Now && c.Delivered == DateTime.MinValue).Any())
            {
                throw new DeleteException($"Drone with {id}is on  delivery");
            }
            try
            {
                myDal.DeleteDrone(id);
                drones.RemoveAll(pc => pc.Id == id);
            }
            catch (DLAPI.DeleteException d)
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
            if (!myDal.GetBaseStationsList(null).Any(c => c.Id == id))
            {
                throw new DeleteException($"BaseStation with {id}as Id does not exist");
            }
            else
                if (drones.Any(dr => dr.DroneLocation == new Location { Latitude = myDal.GetBaseStation(id).Latitude, Longitude = myDal.GetBaseStation(id).Longitude } && dr.Status == Enums.DroneStatuses.Maintenance))
                throw new DeleteException($"BaseStation with {id}as Id is in use");
            else
                try
                {
                    myDal.DeleteBasestation(id);
                    drones.RemoveAll(pc => pc.Id == id);
                }
                catch (DLAPI.DeleteException d)
                {
                    throw new DeleteException($"BaseStation with {id}as Id does not exist", d);
                }
        }
        #endregion
        #region

        public IEnumerable<BaseStationToList> GetBaseStationList(Func<BaseStationToList, bool> predicat = null)
        {
            List<DO.BaseStation> bases = (List<DO.BaseStation>)myDal.GetBaseStationsList(null);
            List<BaseStationToList> baseStationTos = new();
            foreach (var it in bases)
            {
                if (it.Valid == true)//return only valid bases
                {
                    BaseStationToList baseStationTo = new();
                    baseStationTo.Id = it.Id;
                    baseStationTo.Name = it.Name;
                    Location loc = new Location { Latitude = it.Latitude, Longitude = it.Longitude };
                    baseStationTo.BaseStationLocation = loc;
                    baseStationTo.NumOfSlotsInUse = dronCharges(GetBaseStation(it.Id)).Count;
                    baseStationTo.NumOfFreeSlots = it.NumOfSlots - baseStationTo.NumOfSlotsInUse;
                    foreach (var dr in drones)
                    {
                        if (dr.Status == Enums.DroneStatuses.Maintenance && dr.DroneLocation == baseStationTo.BaseStationLocation)
                            baseStationTo.ChargingDrones.Add(new DroneCharge { BatteryStatus = dr.BatteryStatus, Id = dr.Id });
                    }
                    baseStationTos.Add(baseStationTo);
                  
                }

            }

            if (predicat == null)
                return baseStationTos;
            else
                return (from item in baseStationTos
                        where predicat(item)
                        select item);
        }
        public IEnumerable<BaseStationToList> GetListOfBaseStationsWithFreeSlots()
        {
            List<BaseStationToList> tmp = (List<BaseStationToList>)GetBaseStationList();
            return tmp.FindAll(bs => bs.NumOfFreeSlots > 0);

        }
        public IEnumerable<ParcelToList> GetParcelList(Enums.WeightCategories? statuses = null)
        {
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            List<ParcelToList> parcelTos = new();
            foreach (var it in parcels)
            {

                ParcelToList parcelToList = new ParcelToList
                {
                    Id = it.Id,
                    Priority = (Enums.Priorities)it.Priority,
                    WeightCategorie = (Enums.WeightCategories)it.Weight
                };


                parcelToList.SenderName = myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.SenderId).Name;
                parcelToList.TargetName = myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.TargetId).Name;
                parcelTos.Add(parcelToList);
            }
            if (statuses == null)
                return parcelTos;
            else

                return (from item in parcelTos
                        where item.WeightCategorie == statuses
                        select item);

        }
        public IEnumerable<ParcelToList> GetParcelList(string name)
        {
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            List<ParcelToList> parcelTos = new();
            foreach (var it in parcels)
            {

                ParcelToList parcelToList = new ParcelToList
                {
                    Id = it.Id,
                    Priority = (Enums.Priorities)it.Priority,
                    WeightCategorie = (Enums.WeightCategories)it.Weight
                };


                parcelToList.SenderName = myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.SenderId).Name;
                parcelToList.TargetName = myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.TargetId).Name;
                parcelTos.Add(parcelToList);
            }
            if (name=="")
                return parcelTos;
            else

                return (from item in parcelTos
                        where item.SenderName== name || item.TargetName==name
                        select item);

        }

        public IEnumerable<ParcelToList> GetParcelNotAssignedList()
        {
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            List<ParcelToList> tmp = (List<ParcelToList>)GetParcelList();
            List<ParcelToList> toReturn = new();
            foreach (var t in parcels)
            {
                if (t.DroneId == 0)
                    toReturn.Add(tmp.Find(p => p.Id == t.Id));
            }
            return toReturn.ToList();


        }

        public IEnumerable<DroneToList> GetDronesInBaseStationList(int Id)
        {
            Location BaseLoc = new Location {Latitude = myDal.GetBaseStation(Id).Latitude ,
                Longitude = myDal.GetBaseStation(Id).Longitude };
            return (from item in drones
                    where item.DroneLocation == BaseLoc &&
                    item.Status == Enums.DroneStatuses.Maintenance
                    select item);                        
        }

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

        public IEnumerable<CustomerToList> GetCustomerList(Func<CustomerToList, bool> predicat = null)
        {
            List<DO.Customer> customers = (List<DO.Customer>)myDal.GetCustomerList();
            List<CustomerToList> customerTos = new();
            foreach (var it in customers)
            {
                CustomerToList ct = new();
                ct.Id = it.Id;
                ct.Name = it.Name;
                ct.Phone = it.Phone;

                List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
                ct.NumberOfParcelsReceived = parcels.FindAll(pc => pc.TargetId == ct.Id && pc.Delivered <= DateTime.Now && pc.Delivered != DateTime.MinValue).Count;
                ct.NumberOfParcelsonTheWay = parcels.FindAll(pc => pc.TargetId == ct.Id && pc.Delivered > DateTime.Now).Count;
                ct.NumberOfParcelsSentAndDelivered = parcels.FindAll(pc => pc.SenderId == ct.Id && pc.Delivered <= DateTime.Now && pc.Delivered != DateTime.MinValue).Count;
                ct.NumberOfParcelsSentButNotDelivered = parcels.FindAll(pc => pc.SenderId == ct.Id && pc.Delivered > DateTime.Now).Count;
                customerTos.Add(ct);

            }
            if (predicat == null)
                return customerTos.ToList();

            return (from item in customerTos
                    where predicat(item)
                    select item);
        }
        #endregion
    }
}
