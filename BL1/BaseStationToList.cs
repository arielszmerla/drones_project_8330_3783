﻿namespace BO
{/// <summary>
/// implement BaseStationToList class
/// </summary>
    public class BaseStationToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFreeSlots { get; set; }
        public int NumOfSlotsInUse { get; set; }
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