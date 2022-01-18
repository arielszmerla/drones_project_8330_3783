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

        #region adapters
        /// <summary>
        /// get customer from DO converts it to customertolist
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        private CustomerToList DOcustomerToListBO(DO.Customer it)
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
        /// adapter, drone from DO to Be
        /// </summary>
        /// <param name="d"></param>
        /// <returns> BO drone </returns>
        Drone droneBODOadpater(DO.Drone d) => dODrone(d);
        /// <summary>
        /// get customer from DO converts it to customertolist
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        private Customer DOcustomerB(DO.Customer it)
        {
            BO.Customer ct = new();
            ct.Id = it.Id;
            ct.Name = it.Name;
            ct.Phone = it.Phone;
            List<DO.Parcel> parcels = (List<DO.Parcel>)Dal.GetParcelList(null);
            ct.Location = new Location { Latitude = it.Latitude, Longitude = it.Longitude };

            IEnumerable<DO.Parcel> parce = (from item in parcels
                                            where item.TargetId == ct.Id
                                            select item);

            ct.From = (IEnumerable<ParcelByCustomer>)(from item in parce
                                                      let parc = dOparcelTObyCustomerBO(item)
                                                      select parc);
            IEnumerable<DO.Parcel> p = (from item in parcels
                                        where item.SenderId == ct.Id
                                        select item);
            ct.To = (IEnumerable<ParcelByCustomer>)(from item in p
                                                    let parc = dOparcelFROMbyCustomerBO(item)
                                                    select parc);
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
        private BaseStation dOBaseStation(DO.BaseStation myBase)
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

            DroneToList d = drones.Find(d => d.Id == dr.Id);
            Drone bs = new Drone
            {
                Location = d.Location,
                MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                Id = dr.Id,
                Model = (Enums.DroneNames)dr.Model,
                Battery = d.Battery,
                PID = null,
                Status = d.Status,
                DeliveryId = 0,

            };
            //create parcel in delivery if any
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
                bs.DeliveryId = parce.Id;
                if (parce.PickedUp == null)
                {
                    bs.Distance = bs.Distances(GetCustomer(parce.SenderId));
                }
                if (parce.Delivered == null && parce.PickedUp != null)
                {
                    bs.Distance = bs.Distances(GetCustomer(parce.TargetId));
                }
            }

            return bs;
        }
        /// <summary>
        /// converts drone from do to DroneTolist in BO
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns> adapted DroneToList in Bo
        [Obsolete("not in use but can be usable")]
        private DroneToList dODroneToList(DO.Drone dr)
        {
            lock (Dal)
                return new DroneToList
                {
                    Location = drones.Find(dr => dr.Id == dr.Id).Location,
                    MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                    Id = dr.Id,
                    Model = (Enums.DroneNames)dr.Model,
                    Battery = drones.Find(dr => dr.Id == dr.Id).Battery,
                    Status = drones.Find(dr => dr.Id == dr.Id).Status,
                    DeliveryId = Dal.GetParcelList(p => p.Delivered == null && p.PickedUp != null && p.DroneId == dr.Id).Count(),
                    Valid = dr.Valid,
                    NumOfDeliveredParcel = Dal.GetParcelList(p => p.Delivered != null && p.DroneId == dr.Id).Count()

                };
        }
        /// <summary>
        /// convert basestation DO to basestationtolist BO 
        /// </summary>
        /// <param name="baseStation"></pacel from DO>
        /// <returns></returns> new adapted parcel
        private BaseStationToList adaptBaseStationToList(DO.BaseStation baseStation)
        {

            BaseStationToList baseStationTo = new();
            baseStationTo.Id = baseStation.Id;
            baseStationTo.Name = baseStation.Name;
            Location loc = new Location { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };
            baseStationTo.Location = loc;
            baseStationTo.ChargingDrones = dronCharges(GetBaseStation(baseStation.Id));
            baseStationTo.NumOfSlotsInUse = baseStationTo.ChargingDrones.Count;
            baseStationTo.NumOfFreeSlots = baseStation.NumOfSlots;
            baseStationTo.Valid = baseStation.Valid;
            return baseStationTo;
        }
        /// <summary>
        /// do parcel in customer converter to BO parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns> adapted parcel in customer
        private ParcelByCustomer dOparcelTObyCustomerBO(DO.Parcel parcel)
        {
            lock (Dal)
            {   //set logical status from data knowlegde
                ParcelByCustomer tmp = new();
                tmp.Id = parcel.Id;
                tmp.Priorities = (Enums.Priorities)parcel.Priority;
                if (parcel.Delivered != null)
                    tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
                else if (parcel.PickedUp != null)
                    tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
                else if (parcel.Requested != null)
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
        }
        /// <summary>
        /// DO parcel from customer converter to BO parcel from customer
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns>parcel from customer in BO  </returns> 
        private ParcelByCustomer dOparcelFROMbyCustomerBO(DO.Parcel parcel)
        {
            lock (Dal)
            {
                ParcelByCustomer tmp = new();
                tmp.Id = parcel.Id;
                tmp.Priorities = (Enums.Priorities)parcel.Priority;
                if (parcel.Delivered != null)
                    tmp.ParcelStatus = Enums.ParcelStatus.Delivered;
                else if (parcel.PickedUp != null)
                    tmp.ParcelStatus = Enums.ParcelStatus.PickedUp;
                else if (parcel.Requested != null)
                    tmp.ParcelStatus = Enums.ParcelStatus.Assigned;
                else
                    tmp.ParcelStatus = Enums.ParcelStatus.Created;
                tmp.CIP = new CustomerInParcel { Id = parcel.SenderId, Name = Dal.GetCustomer(parcel.SenderId).Name };
                tmp.WeightCategorie = (Enums.WeightCategories)parcel.Weight;
                return tmp;
            }
        }
        #endregion
    }
}



