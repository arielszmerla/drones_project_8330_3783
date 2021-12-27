namespace BO
{/// <summary>
 /// implement class drone
 /// </summary>
    public class Drone
    {
        public int Id { get; set; }//
        public BO.Enums. DroneNames Model { get; set; }//
        public BO.Enums.WeightCategories MaxWeight { get; set; }//
        public double BatteryStatus { get; set; }
        public BO.Enums.DroneStatuses Status { get; set; }//
        public ParcelInDelivery PID { get; set; }
        public Location Location { get; set; }
        public override string ToString()///toString erased func
        {
            string result = "";
            result += $"ID is {Id}\n";
            result += $"model is {Model}\n";
            result += $"maxWeight is {MaxWeight}\n";
            result += $"Battery Status is {BatteryStatus}\n";
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
