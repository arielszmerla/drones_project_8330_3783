using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{/// <summary>
 ///  customer class data
 /// </summary>
    public struct Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"Id is {Id}\n";
            _result += $"name is {Name}\n";
            _result += $"Phone is {Phone}\n";
            _result += $"longitude is  {(Longitude)}\n";
            _result += $"latitude is {(Latitude)}\n";
            return _result;
        }
    }
}
