using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class LocationFuncs
    {
        static internal string printLat(double num)
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
            return hours + "°" + minute + "'" + secs + (char)34 + "" + coordin;
        }
        static internal string printLong(double num)
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
            return hours + "°" + minute + "'" + secs + (char)34 + "" + coordin;
        }
        /// <summary>
        /// func that calculates distance betweeen two points on the earth globus
        ///knowing their coordinates
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <returns></returns>
        static internal double Distance(Location l1, Location l2)
        {
            double lat1 = l1.Latitude;
            double lat2 = l2.Latitude;
            double lon1 = l1.Longitude;
            double lon2 = l2.Longitude;
            double myPI = 0.017453292519943295;    // Math.PI / 180
            double a = 0.5 - Math.Cos((lat2 - lat1) * myPI) / 2 +
                    Math.Cos(lat1 * myPI) * Math.Cos(lat2 * myPI) *
                    (1 - Math.Cos((lon2 - lon1) * myPI)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
    }
}
