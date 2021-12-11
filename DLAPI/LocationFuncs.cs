using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DLAPI
{
    public class LocationFuncs
    {
        public string printLat(double num)
        {
            char coordin;
            int hours = (int)num;
            if (hours < 0)
            {
                coordin = 'S';
                num *= -1;
                hours *= -1;
            }
            else coordin = 'N';
            double minutes = (num - hours) * 60;
            int minute = (int)minutes;
            double second = (minutes - minute) * 600000;
            int sec = (int)second;
            double secs = sec / 10;
            secs /= 1000;
            return hours + "° " + minute + "' " + secs + (char)34 + " " + coordin;
        }
        public string printLong(double num)
        {
            char coordin;
            int hours = (int)num;
            if (hours < 0)
            {
                coordin = 'W';
                num *= -1;
                hours *= -1;
            }
            else coordin = 'E';
            double minutes = (num - hours) * 60;
            int minute = (int)minutes;
            double second = (minutes - minute) * 600000;
            int sec = (int)second;
            double secs = sec / 10;
            secs /= 1000;
            return hours + "° " + minute + "' " + secs + (char)34 + " " + coordin;
        }
        /// <summary>
        /// func that calculates distance betweeen two points on the earth globus
        ///knowing their coordinates
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <returns></returns>
        public double Distance(double lat1, double lon1, double lat2, double lon2)
        {


            double myPI = 0.017453292519943295;    // Math.PI / 180
            double a = 0.5 - Math.Cos((lat2 - lat1) * myPI) / 2 +
                    Math.Cos(lat1 * myPI) * Math.Cos(lat2 * myPI) *
                    (1 - Math.Cos((lon2 - lon1) * myPI)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
    }
}

