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
    sealed partial class DalObject : IDal
    {
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly DalObject instance = new DalObject();
        }
        
        private static object syncRoot = new object();
        public static DalObject Instance
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

        private DalObject() {
            DataSource.Config.Initialize();
        }

        /// <summary>
        /// method to release a charging drone from a base station
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        public void UpdateReleasDroneCharge(int idD, string baseName)
        {
            // first we search for the drone requested
            for (int k = 0; k < DataSource.Drones.Count; k++)
            {
                if (DataSource.Drones[k].Id == idD)
                {
                    // next 'for' iterarion to find base station
                    for (int i = 0; i < DataSource.BaseStations.Count; i++)
                    {
                        if (DataSource.BaseStations[i].Name == baseName)
                        {
                            // after finding both base station and id we do the folowing
                            // 1) make room for a new drone to charge in the base station
                            // 2) change the drone status tp vacant
                            // 3) remove the drone charge from the droneCharge list
                            BaseStation Base = DataSource.BaseStations[i];
                            Base.NumOfSlots++;
                            DataSource.BaseStations[i] = Base;
                            /*Drone temp = DataSource.Drones[k];
                            temp.Status = DroneStatuses.Vacant;
                            DataSource.Drones[k] = temp;*/
                            for (int h = 0; h < DataSource.DroneCharges.Count; h++)
                            {
                                if (DataSource.DroneCharges[i].DroneId == idD)
                                {
                                    DataSource.DroneCharges.RemoveAt(i);
                                }
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// send a drone to charge
        /// </summary>
        /// <param name="idD"></param>
        /// <param name="baseName"></param>
        public void UpdateDroneToCharge(int idD, string baseName)
        {

            for (int k = 0; k < DataSource.Drones.Count(); k++)
            {
                if (DataSource.Drones[k].Id == idD)
                {
                    for (int i = 0; i < DataSource.BaseStations.Count; i++)//look for the right base
                    {
                        BaseStation Base = DataSource.BaseStations[i];
                        if (Base.Name == baseName)//make the asked changes and create anobject of Drone to charge
                        {
                            // after finding base and drone we do the following
                            // take a space off of the base station
                            // set the drone status to be maintenance
                            // set a slot in dronecharge with the drone and base stations ID;
                            Base.NumOfSlots--;
                            /* Drone temp = DataSource.Drones[k];
                             temp.Status = DroneStatuses.Maintenance;
                             DataSource.Drones[k] = temp;*/
                            DroneCharge dc = new DroneCharge();
                            dc.DroneId = idD;
                            dc.StationId = DataSource.BaseStations[i].Id; DataSource.DroneCharges.Add(dc);
                        }
                    }
                }
            }
        }




    }
}
