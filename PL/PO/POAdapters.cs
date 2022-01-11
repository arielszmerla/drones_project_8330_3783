using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PO
{
    class POAdapters
    {
        // converts drone from BO to PO
        public PO.Drone BODroneToPo (BO.Drone drone)
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
    }
}
