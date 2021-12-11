using BO;
using IBL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLAPI
{/// <summary>
/// part of interface
/// </summary>
    public interface IBL
    {
        #region add 
        /// <summary>
        /// method to add a base station 
        /// </summary>
        /// <param name="station"></param>
        void AddBaseStation(BaseStation station);
        /// <summary>
        /// method to add a drone
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="BaseStationNum"></param>
        void AddDrone(DO.Drone drone, int BaseStationNum);
        /// <summary>
        /// method to add a drone
        /// </summary>
        /// <param name="drone"></drone to be sent>
      void AddDrone(Drone drone);
        /// <summary>
        /// mathod to add a parcel
        /// </summary>
        /// <param name="parcel"></param>
        void AddParcel(Parcel parcel);
        /// <summary>
        /// method to add a customer
        /// </summary>
        /// <param name="customer"></param>
        void AddCustomer(Customer customer);
        #endregion
        #region gets
        /// <summary>
        ///  method that returns a parcel by id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        Parcel GetParcel(int idP);
        /// <summary>
        /// method that returns a base station by Id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        BaseStation GetBaseStation(int idP);
        /// <summary>
        /// method that returns a customer by Id
        /// </summary>
        /// <param name="idP"></param>
        /// <returns></returns>
        Customer GetCustomer(int idP);
        /// <summary>
        /// method that returns a drone by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Drone GetDrone(int id);
        #endregion
        #region get lists
        /// <summary>
        /// method to return a list of all the base stations in data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseStationToList> GetBaseStationList(Func<BaseStationToList, bool> predicat = null);
        /// <summary>
        /// method to return a list of all the parcels in data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ParcelToList> GetParcelList(Func<ParcelToList, bool> predicat = null);
        /// <summary>
        /// method to return a list of all the drones in the data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DroneToList> GetDroneList(Enums.DroneStatuses? statuses = null, Enums.WeightCategories? weight = null);
        /// <summary>
        /// method to return a list of all the customers in the data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomerToList> GetCustomerList(Func<CustomerToList, bool> predicat = null);
        /// <summary>
        /// method to return a list of all the parcels who are not assigned to a drone in the data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ParcelToList> GetParcelNotAssignedList();//(Func<ParcelToList,bool> predical = null);
        /// <summary>
        /// method to return a list of all the base stations that have free slots.
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseStationToList> GetListOfBaseStationsWithFreeSlots();
        #endregion
        #region updates
        /// <summary>
        /// method to update the name of a drone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        void UpdateNameDrone(int id, string name);
        /// <summary>
        /// method to update name and/or phone number
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        void UpdateCustomerInfo(int idC, string name, string phone);
        /// <summary>
        /// method to send a drone to charge 
        /// </summary>
        /// <param name="id"></param>
        void UpdateDroneSentToCharge(int id);
        /// <summary>
        /// method to update a base stations name or/and number of free slots.
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="numOfSlots"></param>
        /// <param name="name"></param>
        void UpdateBaseStation(int myId, int numOfSlots, string name);
        /// <summary>
        /// method to release a drone from charging.
        /// </summary>
        /// <param name="idC"></param>
        /// <param name="duration"></param>
        void UpdateReleaseDroneFromCharge(int idC, TimeSpan duration);
        /// <summary>
        /// method to send a drone to pick up a parcel
        /// </summary>
        /// <param name="id"></param>
        void UpdateDroneToPickUpAParcel(int id);
        /// <summary>
        /// method to assign a parcel to a drone
        /// </summary>
        /// <param name="idC"></param>
        void UpdateAssignParcelToDrone(int idC);
        /// <summary>
        /// method to deliver a parcel
        /// </summary>
        /// <param name="id"></param>
        void UpdateDeliverParcel(int id);
        #endregion
    }
}
