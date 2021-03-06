

namespace BO
{/// <summary>
/// implement ParcelToList class
/// </summary>
    public class ParcelToList
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string TargetName { get; set; }
        public BO.Enums.WeightCategories WeightCategorie { get; set; }
        public BO.Enums.Priorities Priority { get; set; }
        /// <summary>
        /// to string override
        /// </summary>
        /// <returns></returns>
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"ID is {Id}\n";
            _result += $"Sender is {SenderName}\n";
            _result += $"Target is {TargetName}\n";
            _result += $"WeightCategorie is {WeightCategorie}\n";
            _result += $"Priorities is {Priority}\n";
            return _result;
        }
    }
}