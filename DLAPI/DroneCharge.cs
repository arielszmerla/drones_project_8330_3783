using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace DO
    {/// <summary>
     /// DroneCharge dataclass
     /// </summary>
        public struct DroneCharge
        {
            public int DroneId { get; set; }
            public int StationId { get; set; }
            public override string ToString()///toString erased func
            {
                string _result = "";
                _result += $"DroneId is {DroneId}\n";
                _result += $"StationId is {StationId}\n";
                return _result;
            }
        }
    }

