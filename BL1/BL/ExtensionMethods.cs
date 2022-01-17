using System;
using BO;
using System.Linq;
using static BL.BLImp;

namespace BL
{
    /// <summary>
    /// extension methods class
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// func that calculets distance between two objects
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns> distance </returns>
        internal static double Distances(this ILocatable from, ILocatable to)
        {
            int R = 6371 * 1000; // metres
            double phi1 = from.Location.Latitude * Math.PI / 180; // φ, λ in radians
            double phi2 = to.Location.Latitude * Math.PI / 180;
            double deltaPhi = (to.Location.Latitude - from.Location.Latitude) * Math.PI / 180;
            double deltaLambda = (to.Location.Longitude - from.Location.Longitude) * Math.PI / 180;

            double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                       Math.Cos(phi1) * Math.Cos(phi2) *
                       Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c / 1000; // in kilometres
            return d;
        }

        /// <summary>
        /// helper calculate assistance
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal static double EuclideanDistance(this Location from, Location to) =>
            Math.Sqrt(Math.Pow(to.Longitude - from.Longitude, 2) + Math.Pow(to.Latitude - from.Latitude, 2));

        /// <summary>
        /// return a customer in a parcel
        /// </summary>
        /// <param name="customer"></param>
        /// <returns> customers id and name </returns>
        internal static CustomerInParcel GetDeliveryCustomer(this DO.Customer customer) =>
            new()
            {
                Id = customer.Id,
                Name = customer.Name
            };

        /// <summary>
        /// location of base station from DO to BO
        /// </summary>
        /// <param name="baseStation"></param>
        /// <returns>BO Location of base station </returns>
        internal static Location Location(this DO.BaseStation baseStation) =>
            new() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };
        /// <summary>
        /// location of customer in DO to BO
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Location of customer in BO</returns>
        internal static Location Location(this DO.Customer customer) =>
            new() { Latitude = customer.Latitude, Longitude = customer.Longitude };
        /// <summary>
        /// method that checks and returns required battery for a drone to pick up a parcel and go 
        /// charge in a close base station
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="bl"></param>
        /// <param name="parcelId"></param>
        /// <returns> battey required </returns>
        internal static double RequiredBattery(this ILocatable drone, BLImp bl, int parcelId)
        {
            DO.Parcel parcel = bl.Dal.GetParcel(parcelId);
            Customer sender = bl.GetCustomer(parcel.SenderId);
            Customer target = bl.GetCustomer(parcel.TargetId);
            double battery = bl.BatteryUsages[(int)Enum.Parse(typeof(Enums.BatteryUsage), parcel.Weight.ToString())] * sender.Distances(target);
            battery += bl.BatteryUsages[DRONE_FREE] * target.Distances(bl.FindClosestBaseStation(target));
            if (parcel.PickedUp is null)
                battery += bl.BatteryUsages[DRONE_FREE] * drone.Distances(sender);
            return battery;
        }
       
    }
}

