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
            /*if (p.Assignment != DateTime.MinValue)
             {
                 DroneToList d = drones.Find(dr => dr.Id == parcel.DroneId);
                 int ids= d.Id;
                 p.DIP.Id = ids;
                 p.DIP.DronePlace = d.DroneLocation;
                 p.DIP.BatteryStatus = d.BatteryStatus;
             }*/
            p.PickedUp = parcel.PickedUp;
            p.Priorities = (Enums.Priorities)parcel.Priority;
            CustomerInParcel send = new CustomerInParcel { Id = parcel.SenderId, Name = customers.Find(cs => cs.Id == parcel.SenderId).Name };
            CustomerInParcel targ = new CustomerInParcel { Id = parcel.TargetId, Name = customers.Find(cs => cs.Id == parcel.TargetId).Name };
            p.Sender= send;
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
            List<DO.BaseStation> bases = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            if (bases.Any(pc => pc.Id == idP))
            {
                myBase = bases.Find(pc => pc.Id == idP);
            }
            else
                throw new GetException("id of BaseStation not found");
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
            List<DO.Customer> customers = (List<DO.Customer>)myDal.GetCustomerList();
            if (customers.Any(pc => pc.Id == idP))
            {
                myCust = customers.Find(pc => pc.Id == idP);
            }
            else
                throw new GetException("id of Customer not found");
            Customer customer = new Customer();
            customer.Id = myCust.Id;
            customer.Location.Latitude = myCust.Latitude;
            customer.Location.Longitude = myCust.Longitude;
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
                tmp.CIP.Id = it.SenderId;
                tmp.CIP.Name = customers.Find(pc => pc.Id == it.SenderId).Name;
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
                tmp.CIP.Id = it.TargetId;
                tmp.CIP.Name = customers.Find(pc => pc.Id == it.TargetId).Name;
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
        #region

        public IEnumerable<BaseStationToList> GetBaseStationList(Func<BaseStationToList, bool> predicat = null)
        {
            List<DO.BaseStation> bases = (List<DO.BaseStation>)myDal.GetAllBaseStations();
            List<BaseStationToList> baseStationTos = new();
            foreach (var it in bases)
            {
                BaseStationToList baseStationTo = new();
                baseStationTo.Id = it.Id;
                baseStationTo.Name = it.Name;
                baseStationTo.NumOfSlotsInUse = dronCharges(GetBaseStation(it.Id)).Count;
                baseStationTo.NumOfFreeSlots = it.NumOfSlots - baseStationTo.NumOfSlotsInUse;
                baseStationTos.Add(baseStationTo);
            }
            if (predicat == null)
                return baseStationTos;
            else
                //return drones.Where(predicate);
                return (from item in baseStationTos
                        where predicat(item)
                        select item);
        }
        public IEnumerable<BaseStationToList> GetListOfBaseStationsWithFreeSlots()
        {
            List<BaseStationToList> tmp = (List<BaseStationToList>)GetBaseStationList();
            return tmp.FindAll(bs => bs.NumOfFreeSlots > 0);

        }
        public IEnumerable<ParcelToList>GetParcelList(Enums.WeightCategories? statuses = null)
        {
            List<DO.Parcel> parcels = (List<DO.Parcel>)myDal.GetParcelList(null);
            List<ParcelToList> parcelTos = new();
            foreach (var it in parcels)
            {
                ParcelToList parcelToList = new ParcelToList
                {
                    Id = it.Id,
                    Priorities = (Enums.Priorities)it.Priority,
                    WeightCategorie = (Enums.WeightCategories)it.Weight
                };

         
                parcelToList.SenderName =myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.SenderId).Name;
                parcelToList.TargetName = myDal.GetCustomerList().FirstOrDefault(s => s.Id == it.TargetId).Name;
                parcelTos.Add(parcelToList);
            }
            if (statuses == null)
                return parcelTos;
            else
                
                return (from item in parcelTos
                        where item.WeightCategorie==statuses
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
        public IEnumerable<DroneToList> GetDroneList(Enums.DroneStatuses? statuses = null, Enums.WeightCategories? weight = null)
        {
            if (statuses == null && weight == null)
                return drones.ToList();
            else if (statuses != null && weight == null)
            {
                return drones.Where(d=>d.Status==statuses);
            }
            else if (statuses != null && weight != null)
            {
                return drones.Where(d => d.Status == statuses && d.MaxWeight==weight);
            }
      
                return drones.Where(d=>d.MaxWeight == weight);
            
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
