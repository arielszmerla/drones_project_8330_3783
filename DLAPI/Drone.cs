using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace DO 
    {/// <summary>
    /// drone data 
    /// </summary>
        public struct Drone
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            public override string ToString()///toString erased func
            {
                string result = "";
                result += $"ID is {Id}\n";
                result += $"model is {Model}\n";
                result += $"maxWeight is {MaxWeight}\n";
               // result += $"status is {Status}\n";
               // result += $"battery is {(float)Battery}\n";
                return result;
            }

        }
    }

