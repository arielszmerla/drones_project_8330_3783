namespace BO
{/// <summary>
/// implement class DroneInParcel
/// </summary>
    public class DroneInParcel  :ILocatable
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public Location Location { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"Battery is: {BatteryStatus}\n";
            str += $"Location is  {Location}\n";
            return str;
        }
    }
}