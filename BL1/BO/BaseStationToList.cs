using System.Collections.Generic;

namespace BO
{/// <summary>
/// implement BaseStationToList class
/// </summary>
    public class BaseStationToList : ILocatable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFreeSlots { get; set; }
        public int NumOfSlotsInUse { get; set; }
        public Location Location { get; set; }
        public List<DroneCharge> ChargingDrones { get; set; }
        public bool Valid { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"name is {Name}\n";
            str += $"number of Slots is {(NumOfFreeSlots)}\n";
            str += $"number of Slots in use is {(NumOfSlotsInUse)}\n";
            return str;

        }
    }
}