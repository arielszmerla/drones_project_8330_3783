using System;
using BO;
using System.Linq;
using static BL.BLImp;

namespace BL
{
    internal static class ExtensionMethods
    {
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

        internal static double EuclideanDistance(this Location from, Location to) =>
            Math.Sqrt(Math.Pow(to.Longitude - from.Longitude, 2) + Math.Pow(to.Latitude - from.Latitude, 2));

        internal static CustomerInParcel GetDeliveryCustomer(this DO.Customer customer) =>
            new()
            {
                Id = customer.Id,
                Name = customer.Name
               // Location = new Location { Latitude = customer.Latitude, Longitude = customer.Longitude }
            };

        internal static Location Location(this DO.BaseStation baseStation) =>
            new() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };

        internal static Location Location(this DO.Customer customer) =>
            new() { Latitude = customer.Latitude, Longitude = customer.Longitude };

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
        /*
        internal static (Enums.DroneStatuses, int) nextActionA(this Drone drone, BLImp bl)
        {
            DO.Parcel? parcel;
            return drone.Status switch
            {
                Enums.DroneStatuses.Vacant =>
                        (parcel = bl.Dal.GetParcelList(p => p?.Scheduled == null
                                        && (Enums.WeightCategories)(p?.Weight) <= drone.MaxWeight
                                        && drone.RequiredBattery(bl, (int)p?.Id) < drone.Battery)
                                                .OrderByDescending(p => p?.Priority)
                                                .ThenByDescending(p => p?.Weight).FirstOrDefault()) == null
                        ? (drone.Battery == 1.0 ? (Enums.DroneStatuses.Vacant, 0)
                                                : (Enums.DroneStatuses.Maintenance, bl.FindClosestBaseStation(drone, charge: true).Id))
                        : (Enums.DroneStatuses.InDelivery, (int)parcel?.Id),

                Enums.DroneStatuses.InDelivery =>
                    bl.Dal.GetParcel((int)drone.DeliveryId).Delivered != null ? (Enums.DroneStatuses.Vacant, 0)
                                                                              : (Enums.DroneStatuses.InDelivery, (int)drone.DeliveryId),

                Enums.DroneStatuses.Maintenance => (Enums.DroneStatuses.Vacant, 0),

                Enums.DroneStatuses.None => (Enums.DroneStatuses.None, 0),

                _ => (Enums.DroneStatuses.None, 0)
            };
        }
        */
    }
}

