using System.Collections.Generic;
using BO;

namespace BO
{
    /// <summary>
    /// implement class BaseStation
    /// </summary>
    public class BaseStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location BaseStationLocation { get; set; }
        public int NumOfFreeSlots { get; set; }
        public List<DroneCharge> ChargingDrones { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"name is {Name}\n";
            str += $"Location is {BaseStationLocation}\n";
            foreach (var it in ChargingDrones)
            {
                str += it;
            }
            str += $"number of Slots is {(NumOfFreeSlots)}\n";
            return str;
        }
    }
}