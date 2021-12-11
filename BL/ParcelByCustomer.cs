using static BO.Enums;

namespace BO
{/// <summary>
/// implement ParcelByCustomer class
/// </summary>
    public class ParcelByCustomer
    {
        public int Id { get; set; }
        public WeightCategories WeightCategorie { get; set; }
        public Priorities Priorities { get; set; }
        public ParcelStatus ParcelStatus { get; set; }
        public CustomerInParcel CIP { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"Parcel Status is {ParcelStatus}\n";
            str += $"WeightCategorie is {WeightCategorie}\n";
            str += $"Priorities is {Priorities}\n";
            str += $"Customer in parcel is {CIP}\n";
        
            return str;
        }
    }
}