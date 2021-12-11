namespace BO
{/// <summary>
/// implement class DroneInParcel
/// </summary>
    public class DroneInParcel
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public Location DronePlace { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"Battery is: {BatteryStatus}\n";
            str += $"Location is  {DronePlace}\n";
            return str;
        }
    }
}