namespace BO
{/// <summary>
 /// implement class drone
 /// </summary>
    public class Drone :ILocatable
    {
        public int Id { get; set; }//
        public BO.Enums. DroneNames Model { get; set; }//
        public BO.Enums.WeightCategories MaxWeight { get; set; }//
        public double Battery { get; set; }
        public BO.Enums.DroneStatuses Status { get; set; }//
        public ParcelInDelivery PID { get; set; }
        public Location Location { get; set; }
        public int DeliveryId { get; set; }//   DeliveryId
        public double Distance { get; set; } // Distance to next stop
        public override string ToString()///toString erased func
        {
            string result = "";
            result += $"ID is {Id}\n";
            result += $"model is {Model}\n";
            result += $"maxWeight is {MaxWeight}\n";
            result += $"Battery Status is {Battery}\n";
            result += $"Status is {Status}\n";
            if (PID != null)
            {
                result += $"Parcel in delivery is {PID}\n";
            }
            result += $"Location is {Location}\n";

            return result;
        }

    }
}
