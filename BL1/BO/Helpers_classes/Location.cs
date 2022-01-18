namespace BO
{
    /// <summary>
    /// create Location type
    /// </summary>
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"{LocationFuncs.printLong(Longitude)} "+" " + $"{ LocationFuncs.printLat(Latitude)}";
            return _result;
        }
    }
}