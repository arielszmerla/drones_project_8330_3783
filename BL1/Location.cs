namespace BO
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public override string ToString()///toString erased func
        {
            string _result = "";
            _result += "Long:"+ $"{LocationFuncs.printLong(Longitude)}"+
            $"    Lat:{ LocationFuncs.printLat(Latitude)}";
            return _result;
        }
    }
}