using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace BO
    {/// <summary>
     /// implement Customer class
     /// </summary>
        public class Customer : ILocatable
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location Location { get; set; }
            public IEnumerable<ParcelByCustomer> From { get; set; }
            public IEnumerable<ParcelByCustomer> To { get; set; }
            public override string ToString()
            {
                string str = "";
                str += $"ID is {Id}\n";
                str += $"name is {Name}\n";
                str += $"Location is {Location}\n";
                str += $"Parcels From:\n";
                foreach (var C in From)
                {
                    str += C;
                }
                str += $"Parcels TO:\n";
                foreach (var C in To)
                {
                    str += C;
                }
                return str;
            }
        }
    }

