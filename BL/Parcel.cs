using System;
using static IBL.BO.Enums;

namespace IBL.BO
{/// <summary>
/// implement parcel class
/// </summary>
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public WeightCategories WeightCategories { get; set; }
        public Priorities Priorities { get; set; }
        public DroneInParcel DIP { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Assignment { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"Sender is {Sender}\n";
            str += $"Target is {Target}\n";
            str += $"WeightCategorie is {WeightCategories}\n";
            str += $"Priorities is {Priorities}\n";
            if (DIP != null)
                str += $"Drone in parcel is {DIP}\n";
            str += $"Created time is {Created}\n";
            str += $"Assignment time is {Assignment}\n";
            str += $"Pick Up time is {PickedUp}\n";
            str += $"Delivery time is {Delivered}\n";
            return str;
        }

    }
}