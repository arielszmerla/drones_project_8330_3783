namespace BO
{
    public class DroneToList :ILocatable
    {
        public int Id { get; set; }
        public BO.Enums.DroneNames Model { get; set; }
        public BO.Enums.WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public BO.Enums.DroneStatuses Status { get; set; }
        public Location Location { get; set; }
        public int NumOfDeliveredParcel { get; set; }
        public bool Valid { get; set; }
        public int? DeliveryId { get; set; }
        public override string ToString()///toString erased func
        {
            string result = "";
            result += $"{"Drone ID is:",-30} {Id,44}\n";
            result += $"{"Model name is:",-30} {Model,41}\n";
            result += $"{"Maximal transported Weight:",-30} {MaxWeight,30}\n";
            result += $"{"Status is:",-30} {Status,44}\n";
            result += $"{"Battery level is:",-30} {(int)Battery,40}%\n";
            result += $"Emplacement is: \n" + Location;
            result += $"{"Number of delivered parcels is:",-30} {NumOfDeliveredParcel,30}\n";
            for (int i = 0; i < 60; i++)
            {
                result += "_";
            }
            return result;
        }
    }
}