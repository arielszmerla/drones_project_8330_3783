namespace BO
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"{"Longitude is:",-30}  { LocationFuncs.printLong(Longitude),45}\n";
            _result += $"{"Latitude is:",-30}   { LocationFuncs.printLat(Latitude),45}\n";
            return _result;
        }
    }
}