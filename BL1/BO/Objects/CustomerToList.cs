using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{/// <summary>
/// implement class CustomerToList
/// </summary>
    public class CustomerToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int NumberOfParcelsSentAndDelivered { get; set; }
        public int NumberOfParcelsSentButNotDelivered { get; set; }
        public int NumberOfParcelsReceived { get; set; }
        public int NumberOfParcelsonTheWay { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"name is {Name}\n";
            str += $"Phone  is {Phone}\n";
            str += $"number of Parcels sent and delivered is {NumberOfParcelsSentAndDelivered}\n";
            str += $"number of Parcels sent but not delivered is {NumberOfParcelsSentButNotDelivered}\n";
            str += $"number of Parcels received is {NumberOfParcelsReceived}\n";
            str += $"number of Parcels on their way is {NumberOfParcelsonTheWay}\n";
            return str;
        }
    }
}
