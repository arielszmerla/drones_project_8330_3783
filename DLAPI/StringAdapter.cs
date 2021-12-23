using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace DO
    {
       public class StringAdapter
        {
            public static string printLat(double num)
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

                return " " + hours + "° " + minute + "' " + secs + (char)34 + " " + coordin;

            }
            /// <summary>
            /// gets and hours/mins / sec presentation to coordinate
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
            static public string PrintLong(double num)
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

                return " " + hours + "° " + minute + "' " + secs + (char)34 + " " + coordin;
            }

            /// <summary>
            /// func to get security digit from id
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
          public  static int sumDigits(int num)
            {//Gets number and calculate and return the sum of its units
                int sum = 0;
                while (num > 0)
                {
                    sum = sum + (num % 10);
                    num = num / 10;
                }
                return sum;
            }
            public static int lastDigitID(int numberId)
            {//gets id number and print the security number
            
                int secureNumber = 0;
                int i = 0;
                while (i < 8)
                {
                    if (i % 2 == 0)
                        secureNumber = secureNumber + sumDigits((numberId % 10) * 2);
                    else
                        secureNumber = secureNumber + (numberId % 10);

                    numberId = numberId / 10;
                    i++;
                }
                return numberId * 10 + ((10 - (secureNumber % 10)) % 10);
            }
        }
    }

