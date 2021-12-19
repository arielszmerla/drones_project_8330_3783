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
    internal partial class DalObject : IDal
    {
        /// <summary>
        /// send new parcel to database
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></int as value of new index>
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
            Parcel? myParcel = DataSource.Parcels.Find(p=>p.Id==id);
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
                throw new ParcelExeption("invalid parcel id");
            Parcel tmp = DataSource.Parcels[k];
            tmp.Delivered = DateTime.Now;
            tmp.DroneId = 0;
            DataSource.Parcels[k] = tmp;
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> GetParcelList(Func<Parcel, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.Parcels.ToList();
            else
                return (from item in DataSource.Parcels
                        where predicate(item)
                        select item);
       
        }

      public  void UpdateParcel(Parcel p) {

            int index = DataSource.Parcels.FindIndex(pc => pc.Id == p.Id);
            if (index == -1)
                throw new ParcelExeption($"the parcel {p.Id} doesn't exists");
            DataSource.Parcels[index] = p;
        }
    }
}
