using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO
{

    /// <summary>
    /// parcel class 
    /// </summary>
    public struct Parcel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public Priorities Priority { get; set; }
        public DateTime? Requested { get; set; }
        public int DroneId { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }
        public WeightCategories Weight { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"ID is {Id}\n";
            _result += $"Sender ID is {SenderId}\n";
            _result += $"Target ID is {TargetId}\n";
            _result += $"Requested is {Requested}\n";
            _result += $"DroneId is {DroneId}\n";
            _result += $"Scheduled is {Scheduled}\n";
            _result += $"PickedUp is {PickedUp}\n";
            _result += $"Delivered is {Delivered}\n";
            return _result;
        }
    }
}

