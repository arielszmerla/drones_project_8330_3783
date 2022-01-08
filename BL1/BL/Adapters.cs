using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using DLAPI;

namespace BL
{
    /// <summary>
    /// converts similar types of elements from differeets layers
    /// </summary>
    partial class BLImp
    {
        /// <summary>
        /// get customer from DO converts it to customertolist
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        private BO.CustomerToList DOcustomerBO(DO.Customer it)
        {
            BO.CustomerToList ct = new();
            ct.Id = it.Id;
            ct.Name = it.Name;
            ct.Phone = it.Phone;
            List<DO.Parcel> parcels = (List<DO.Parcel>)Dal.GetParcelList(null);
            ct.NumberOfParcelsReceived = parcels.FindAll(pc => pc.TargetId == ct.Id && pc.Delivered <= DateTime.Now && pc.Delivered != DateTime.MinValue).Count;
            ct.NumberOfParcelsonTheWay = parcels.FindAll(pc => pc.TargetId == ct.Id && pc.Delivered > DateTime.Now).Count;
            ct.NumberOfParcelsSentAndDelivered = parcels.FindAll(pc => pc.SenderId == ct.Id && pc.Delivered <= DateTime.Now && pc.Delivered != DateTime.MinValue).Count;
            ct.NumberOfParcelsSentButNotDelivered = parcels.FindAll(pc => pc.SenderId == ct.Id && pc.Delivered > DateTime.Now).Count;

            return ct;
        }
        /// <summary>
        /// convert parcel DO to parceltolist BO 
        /// </summary>
        /// <param name="it"></pacel from DO>
        /// <returns></returns> new adapted parcel
        private BO.ParcelToList DOparcelBO(DO.Parcel it)
        {
            ParcelToList parcelToList = new ParcelToList
            {
                Id = it.Id,
                Priority = (Enums.Priorities)it.Priority,
                WeightCategorie = (Enums.WeightCategories)it.Weight
            };

            parcelToList.SenderName = Dal.GetCustomerList().FirstOrDefault(s => s.Id == it.SenderId).Name;
            parcelToList.TargetName = Dal.GetCustomerList().FirstOrDefault(s => s.Id == it.TargetId).Name;
            return parcelToList;
        }
        /// <summary>
        /// convert basestation DO to basestationtolist BO 
        /// </summary>
        /// <param name="myBase"></pacel from DO>
        /// <returns></returns> new adapted parcel    
        private BO.BaseStation dOBaseStation(DO.BaseStation myBase)
        {
            BaseStation bs = new BaseStation
            {
                Location = new Location { Latitude = myBase.Latitude, Longitude = myBase.Longitude },

                Id = myBase.Id,
                Name = myBase.Name

            };
            bs.ChargingDrones = dronCharges(bs);
            bs.NumOfFreeSlots = myBase.NumOfSlots - bs.ChargingDrones.Count;

            return bs;
        }
        /// <summary>
        /// convert basestation DO to basestationtolist BO 
        /// </summary>
        /// <param name="myBase"></pacel from DO>
        /// <returns></returns> new adapted parcel    
        private Drone dODrone(DO.Drone dr)
        {
            Drone bs = new Drone
            {
                Location = drones.Find(dr => dr.Id == dr.Id).Location,
                MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                Id = dr.Id,
                Model = (Enums.DroneNames)dr.Model,
                Battery = drones.Find(dr => dr.Id == dr.Id).Battery,
                PID = null,
                Status = drones.Find(dr => dr.Id == dr.Id).Status
            };

            DO.Parcel parce = new();
            if (Dal.GetParcelList(p => p.DroneId == dr.Id && p.Delivered == null && p.Scheduled != null).Any())
            {
                parce = Dal.GetParcelList(p => p.DroneId == dr.Id && p.Delivered == null && p.Scheduled != null).FirstOrDefault();
                bs.PID = new ParcelInDelivery();
                bs.PID.Id = parce.Id;
                bs.PID.Prioritie = (Enums.Priorities)parce.Priority;
                bs.PID.Target = new CustomerInParcel { Id = parce.SenderId, Name = Dal.GetCustomer(parce.SenderId).Name };
                bs.PID.Sender = new CustomerInParcel { Id = parce.TargetId, Name = Dal.GetCustomer(parce.TargetId).Name };
                bs.PID.TargetLocation = GetCustomer(parce.TargetId).Location;
                bs.PID.Location = GetCustomer(parce.SenderId).Location;
                bs.PID.WeightCategorie = (Enums.WeightCategories)parce.Weight;
                bs.PID.Distance = bs.PID.Distances(GetCustomer(parce.TargetId));

            }
         
            return bs;
        }
        /// <summary>
        /// convert basestation DO to basestationtolist BO 
        /// </summary>
        /// <param name="baseStation"></pacel from DO>
        /// 
        /// <returns></returns> new adapted parcel
        private BO.BaseStationToList adaptBaseStationToList(DO.BaseStation baseStation)
        {

            BaseStationToList baseStationTo = new();
            baseStationTo.Id = baseStation.Id;
            baseStationTo.Name = baseStation.Name;
            Location loc = new Location { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };
            baseStationTo.Location = loc;
            baseStationTo.ChargingDrones = dronCharges(GetBaseStation(baseStation.Id));
            baseStationTo.NumOfSlotsInUse = baseStationTo.ChargingDrones.Count;
            baseStationTo.NumOfFreeSlots = baseStation.NumOfSlots - baseStationTo.NumOfSlotsInUse;
            baseStationTo.Valid = baseStation.Valid;
            return baseStationTo;
        }
        /// <summary>
        /// do parcel in customer converter
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        private BO.ParcelByCustomer dOparcelTObyCustomerBO(DO.Parcel parcel)
        {

            ParcelByCustomer tmp = new();
            tmp.Id = parcel.Id;
            tmp.Priorities = (Enums.Priorities)parcel.Priority;
            if (parcel.Delivered != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
            else if (parcel.PickedUp != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
            else if (parcel.Requested != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.Assigned;
            else
                tmp.ParcelStatus = Enums.ParcelStatus.Created;

            tmp.CIP = new CustomerInParcel
            {
                Id = parcel.TargetId,
                Name = Dal.GetCustomer(parcel.TargetId).Name
            };
            tmp.WeightCategorie = (Enums.WeightCategories)parcel.Weight;
            return tmp;
        }
        /// <summary>
        /// do parcel in customer converter
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        private BO.ParcelByCustomer dOparcelFROMbyCustomerBO(DO.Parcel parcel)
        {
            ParcelByCustomer tmp = new();
            tmp.Id = parcel.Id;
            tmp.Priorities = (Enums.Priorities)parcel.Priority;
            if (parcel.Delivered != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
            else if (parcel.PickedUp != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
            else if (parcel.Requested != DateTime.MinValue)
                tmp.ParcelStatus = Enums.ParcelStatus.Assigned;
            else
                tmp.ParcelStatus = Enums.ParcelStatus.Created;
            tmp.CIP = new CustomerInParcel { Id = parcel.SenderId, Name = Dal.GetCustomer(parcel.SenderId).Name };
            tmp.WeightCategorie = (Enums.WeightCategories)parcel.Weight;
            return tmp;
        }
    }
}

