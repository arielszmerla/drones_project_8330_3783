using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    /// <summary>
    /// basestation data class
    /// </summary>
    public struct BaseStation
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int NumOfSlots { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"ID is {Id}\n";
            _result += $"name is {Name}\n";
            _result += $"longitude is {StringAdapter.PrintLong(Longitude)}\n";
            _result += $"latitude is {StringAdapter.printLat(Latitude)}\n";
            _result += $"number of Slots is {(NumOfSlots)}\n";
            return _result;
        }
        /// <summary>
        /// gets and hours/mins / sec presentation to coordinate
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>

    }
}


