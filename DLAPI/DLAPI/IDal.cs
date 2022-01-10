using DO;
using System;
using System.Collections.Generic;

namespace DLAPI
{
    public interface IDal
    { 
        #region add options
        /// <summary>
        ///  method to add a base station  
        /// </summary>
        /// <param name="baseStation"></param>
        void AddBaseStation(BaseStation baseStation);
        /// <summary>
        /// method to add a customer
        /// </summary>
        /// <param name="customer"></param>
        void AddCustomer(Customer customer);
        /// <summary>
        /// method to add a drone 
        /// </summary>
        /// <param name="drone"></param>
        void AddDrone(Drone drone);
        /// <summary>
        /// method to add a parcel.
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        int AddParcel(Parcel parcel);
        /// <summary>
        /// method to add a dronecharge unit.
        /// </summary>
        /// <param "id drone, id parcel"></param>
        void AddDroneCharge(int idDrone, int idBase);

        #endregion
        #region get list options
        /// <summary>
        /// method that returns all base stations in data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicate);
        /// <summary>
        /// method that returns all customers in data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicat = null);
        /// <summary>
        /// method that returns all parcels in data base.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate);
        int GetDroneChargeBaseStationId(int droneId);
        /// <summary>
        /// holds array with electric cunsumption by weight of parcel
        /// </summary>
        /// <returns></returns>
        double[] DroneElectricConsumations();
        /// <summary>
        /// method to get list of drones
        /// </summary>
        /// <returns></returns>
        IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate=null);
         IEnumerable<DroneCharge> GetDroneCharges(int idBase);
        #endregion
        #region get a single element 
        /// <summary>
        /// method to get a specific base station
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BaseStation GetBaseStation(int id);
        /// <summary>
        /// method to get a specific customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Customer GetCustomer(int id);
        /// <summary>
        /// method to get a specific drone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Drone GetDrone(int id);
        /// <summary>
        ///  method to get a specific parcel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Parcel GetParcel(int id);
        #endregion
        #region updates
        /// <summary>
        /// method to update a drone
        /// </summary>
        /// <param name="dr"></param>
        void UpdateDrone(Drone dr);
        /// <summary>
        /// method to update the time that a drone picks up a parcel
        /// </summary>
        /// <param name="id"></param>
        void UpdateDronePickUp(int id);
        /// <summary>
        /// to set a parcel to pickup
        /// </summary>
        /// <param name="id"></param>
         void ParcelDelivery(int parcelId);
        /// <summary>
        /// to set a parcel to pickup
        /// </summary>
        /// <param name="id"></param>
         void ParcelPickup(int parcelId);
        /// <summary>
        /// to set a parcel to pickup
        /// </summary>
        /// <param name="id"></param>
         void ParcelSchedule(int parcelId,int droneId);
        /// <summary>
        /// mehod to send drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        void UpdateDroneToCharge(int idD, string baseName);
        /// <summary>
        /// method to update the time of the delivery in the drone to now.
        /// </summary>
        /// <param name="id"></param>
        void UpdatesParcelDelivery(int id);
        /// <summary>
        /// method to connnect a parcel to a drone.
        /// </summary>
        /// <param name="idP"></param>
        /// <param name="idD"></param>
        void UpdateParcelToDrone(int idP, int idD);
        /// <summary>
        /// update to release a drone from charging
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        void UpdateReleasDroneCharge(int idD, string baseName);
//double Distance(double d, double d1, double d2, double d3);
/// <summary>
/// method to update a base station with a base station sent from bl layer
/// </summary>
/// <param name="bs"></param>
        void UpdateBaseStationFromBl(BaseStation bs);
        /// <summary>
        /// method to update a customer from bl layer
        /// </summary>
        /// <param name="bs"></param>
        void UpdateCustomerInfoFromBL(Customer bs);
        /// <summary>
        /// method to update a parcel
        /// </summary>
        /// <param name="p"></param>
        void UpdateParcel(Parcel p);

        #endregion
        #region delete
        /// <summary>
        /// delete an parcel
        /// </summary>
        /// <param name="id"></id of parcel to erase>
        void DeleteParcel(int id);
        /// <summary>
        /// delete an customer
        /// </summary>
        /// <param name="id"></id of customer to erase>
        void DeleteCustomer(int id);
        /// <summary>
        /// delete an drone
        /// </summary>
        /// <param name="id"></id of drone to erase>
        void DeleteDrone(int id);
        /// <summary>
        /// delete an drone
        /// </summary>
        /// <param name="id"></id of drone to erase>
        void DeleteBasestation(int id);
        /// <summary>
        /// method to delete a dronecharge unit.
        /// </summary>
        /// <param "id drone, id parcel"></param>
        void DeleteDroneCharge(int idDrone);
        #endregion
    }
}