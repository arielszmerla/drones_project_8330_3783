using BO;


namespace BO

{/// <summary>
/// implement ParcelInDelivery class
/// </summary>
    public class ParcelInDelivery : ILocatable
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public BO.Enums.Priorities Prioritie { get; set; }
        public BO.Enums.WeightCategories WeightCategorie { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public Location Location { get; set; }
        public Location TargetLocation { get; set; }
        public double Distance { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"ID is {Id}\n";
            _result += $"Sender is {Sender}\n";
            _result += $"Target is {Target}\n";
            _result += $"WeightCategorie is {WeightCategorie}\n";
            _result += $"Priority is {Prioritie}\n";
            _result += $"Status is {Status}\n";
            _result += $"Pick Up Location is {Location}\n";
            _result += $"Target Cusomer Location is {TargetLocation}\n";
            return _result;
        }
    }
}