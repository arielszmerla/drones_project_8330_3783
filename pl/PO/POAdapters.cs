﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PO
{/// <summary>
/// converters
/// </summary>
    class POAdapters
    {

        /// <summary>
        /// converts drone from BO to PO
        /// </summary>
        /// <param name="drone"></param>
        /// <returns></returns>
        public PO.Drone BODroneToPo(BO.Drone drone)
        {
            PO.Drone dr = new();
            dr.Id = drone.Id;
            dr.Location = new Location { Latitude = drone.Location.Latitude, Longitude = drone.Location.Longitude };
            dr.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;
            dr.Model = (Enums.DroneNames)drone.Model;
            dr.Status = (Enums.DroneStatuses)drone.Status;
            dr.Distance = drone.Distance;
            return dr;

        }
        /// <summary>
        ///  converts droneToList from BO to PO
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public PO.DroneToList PODronetolist(BO.DroneToList dr)
        {

            return new PO.DroneToList
            {
                Id = dr.Id,
                Location = new Location { Latitude = dr.Location.Latitude, Longitude = dr.Location.Longitude },
                MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                DeliveryId = dr.DeliveryId,
                NumOfDeliveredParcel = dr.NumOfDeliveredParcel,
                Status = (Enums.DroneStatuses)dr.Status,
                Battery = dr.Battery,
                Model = (Enums.DroneNames)dr.Model
            };
        }
        /// <summary>
        ///  converts Parcel from BO to PO
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        PO.Parcel POParcelBO(BO.Parcel parcel)
        {
            return new Parcel
            {
                Id = parcel.Id,
                Priority = (Enums.Priorities)parcel.Priority,
                Sender = parcel.Sender.Name,
                Target = parcel.Target.Name,
                WeightCategories = (Enums.WeightCategories)parcel.WeightCategories
            };

        }
    }
}
