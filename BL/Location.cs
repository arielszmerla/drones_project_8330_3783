namespace BO
{
    public class Location
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += $"{"Longitude is:", - 30}  { LocationFuncs. printLong( longitude),45}\n";
            _result += $"{"Latitude is:", -30}   { LocationFuncs.printLat(latitude),45}\n";
            return _result;
        }
    }
}