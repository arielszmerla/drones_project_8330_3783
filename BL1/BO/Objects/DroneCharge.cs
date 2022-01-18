namespace BO
{/// <summary>
/// implement class DroneCharge
/// </summary>
    public class DroneCharge
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"Battery is {BatteryStatus}\n";
            return str;
        }
    }
}