using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;


namespace ConsoleUI_BL
{/// <summary>
/// part of main helping funcs (adders)
/// </summary>
    public partial class Program
    {/// <summary>
    /// func to reach right add to do
    /// </summary>
    /// <param name="choice"></param>
    /// <param name="myCompany"></param>
        private static void addAnElement(int choice, BLAPI.IBL myCompany)
        {
            switch (choice)
            {
                case 1:
                  
                        addBaseMain(myCompany);
              
                    break;
                case 2:
                    
                        addDroneMain(myCompany);
                  
                    break;
                case 3:
                   
                        addCustomerMain(myCompany);
                   
                    break;
                case 4:
                    
                  
                        addParcelMain(myCompany);
                   
                    break;
            }
        }



        /// <summary>
        /// func to get security digit from id
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static int sumDigits(int num)
        {//Gets number and calculate and return the sum of its units
            int sum = 0;
            while (num > 0)
            {
                sum = sum + (num % 10);
                num = num / 10;
            }
            return sum;
        }
        static int lastDigitID(int numberId)
        {//gets id number and print the security number
            int secureNumber = 0;
            int i = 0;
            while (i < 8)
            {
                if (i % 2==0)
                    secureNumber = secureNumber + sumDigits((numberId % 10) * 2);
                else
                    secureNumber = secureNumber + (numberId % 10);
                
                numberId = numberId / 10;
                i++;
            }
            return numberId* 10+((10 - (secureNumber % 10))%10);
        }
        /// <summary>
        /// add a base station
        /// </summary>
        /// <param name="cmp"></param>
        static void addBaseMain(BLAPI.IBL cmp)
        {
            bool checkit;
            int myId;
            do
            {

                Console.WriteLine("ENTER THE ID OF THE BASESTATION YOU WANT TO ADD\n");
                checkit = Int32.TryParse(Console.ReadLine(), out myId);
            } while (!checkit);
            Console.WriteLine("ENTER THE NAME OF THE BASESTATION YOU WANT TO ADD\n");
            string name = "Base " + Console.ReadLine();
            double longitude;
            do
            {
                Console.WriteLine("ENTER THE LONGITUDE OF THE BASESTATION YOU WANT TO ADD\n");
                checkit = double.TryParse(Console.ReadLine(), out longitude);
            } while (!checkit);
            double latitude;
            do
            {
                Console.WriteLine("ENTER THE LATITUDE OF THE BASESTATION YOU WANT TO ADD\n");
                checkit = double.TryParse(Console.ReadLine(), out latitude);
            } while (!checkit);
            BO.Location ls = new();
            ls.Latitude = latitude;
            ls.Longitude = longitude;
            int numOfSlots;
            do
            {
                Console.WriteLine("ENTER THE NUMBER OF SLOTS IN THE BASESTATION YOU WANT TO ADD\n");
                checkit = int.TryParse(Console.ReadLine(), out numOfSlots);
            } while (!checkit);
            ///create the new object and send it to datasource
            BO.BaseStation bas = new BO.BaseStation()
            {
                Id = myId,
                Name = name,
                Location = ls,
                ChargingDrones = new List<BO.DroneCharge>(),
                NumOfFreeSlots = numOfSlots

            };
            try
            {
                cmp.AddBaseStation(bas);
                Console.WriteLine("Add done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }
         
        
    }


        /// <summary>
        /// method to add a drone through main 
        /// </summary>
        /// <param name="myComp"></param>
        static void addDroneMain(BLAPI.IBL myComp)
        {
            bool check;
            int id;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE DRONE YOU WANT TO ADD\n");

                check = Int32.TryParse(Console.ReadLine(), out id);
            } while (!check);
            Console.WriteLine("ENTER THE MODEL OF THE DRONE YOU WANT TO ADD\n" +
                "CHOOSE FROM THOSE TYPES:\n");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine((BO.Enums.DroneNames)i);
            }
            string names;
            names= Console.ReadLine();

            Console.WriteLine("ENTER THE WEIGHT CATEGORIE OF THE DRONE YOU WANT TO ADD\n" +
                "CHOOSE FROM THOSE TYPES:\n");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine((BO.Enums.WeightCategories)i);
            }
            BO.Enums.WeightCategories categ;
            do
            {
                check = DO.WeightCategories.TryParse(Console.ReadLine(), out categ);
            } while (!check);
            

            DO.Drone dr = new DO.Drone
            {
                Id = id,
                MaxWeight = (DO.WeightCategories)categ,
                Model = DO.DroneNames.MAVIC
            };
            try
            {
                myComp.AddDrone(dr, id);
                Console.WriteLine("Add done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }


        }
        /// <summary>
        /// method that gets data from the user to add a new customer in company database
        /// </summary>
        /// <param name="myComp"></param>
        static void addCustomerMain(BLAPI.IBL cmp)
        {

            bool checkC;
            int idC;
            do
            {
                Console.WriteLine("PLEASE ENTER CUSTOMER'S ID, IT HAS TO BE MAX EIGHT DIGITS\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idC);
            } while (!checkC);
            idC = lastDigitID(idC);
            Console.WriteLine("PLEASE ENTER CUSTOMER'S NAME:\n");
            string name = Console.ReadLine();
            Console.WriteLine("PLEASE ENTER CUSTOMER'S PHONE NUMBER:\n");
            string phone = Console.ReadLine();
            Console.WriteLine("PLEASE ENTER CUSTOMER'S LOCATION'S LONGITUDE:\n");
            double longitude;
            do
            {
                checkC = double.TryParse(Console.ReadLine(), out longitude);
            } while (!checkC);
            double latitude;
            Console.WriteLine("PLEASE ENTER CUSTOMER'S LOCATION'S LATITUDE:\n");
            do
            {
                checkC = double.TryParse(Console.ReadLine(), out latitude);
            } while (!checkC);
            ///create the new object and send it to datasource
            BO.Location loc = new BO.Location
            {
                Latitude = latitude,
                Longitude = longitude
            };
            BO.Customer customer = new BO.Customer()
            {
                Id = idC,
                Name = name,
                Phone = phone,
                Location = loc,
                From = null,
                To = null
            };
            try
            {
                cmp.AddCustomer(customer);
                Console.WriteLine("Add done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }

        }
        /// <summary>
        /// method that gets data from the user to add a new parcel in company database
        /// </summary>
        /// <param name="myComp"></param>
        /// return index of new parcel
        static void addParcelMain(BLAPI.IBL myComp)
        {

            bool checkC;
            int idC;
            do
            {
                Console.WriteLine("PLEASE ENTER PARCEL'S ID:\n");

                checkC = Int32.TryParse(Console.ReadLine(), out idC);
            } while (!checkC);
            int idSender;
            do
            {
                Console.WriteLine("PLEASE ENTER PARCEL'S SENDER'S ID:\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idSender);
            } while (!checkC);
            int idTarget;
            do
            {
                Console.WriteLine("PLEASE ENTER PARCEL'S TARGET'S ID:\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idTarget);
            } while (!checkC);
           DO.WeightCategories myWeight;
            Console.WriteLine("PLEASE ENTER PARCEL'S WEIGHT TYPE:\n" + "CHOOSE FROM THOSE:\n");
            for (int i = 0; i < 3; i++)
                Console.WriteLine((DO.WeightCategories)i);
            do
            {
                checkC = DO.WeightCategories.TryParse(Console.ReadLine(), out myWeight);
            } while (!checkC);
            Console.WriteLine("PLEASE ENTER PARCEL'S PRIORITY TYPE:\n" + "CHOOSE FROM THOSE:\n");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine((DO.Priorities)i);
            }
            BO.Enums.Priorities priority;
            do
            {
                checkC = BO.Enums.Priorities.TryParse(Console.ReadLine(), out priority);
            } while (!checkC);
            BO.Customer sender = new();
            try {
               sender = myComp.GetCustomer(idSender); }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }
            BO.CustomerInParcel sende = new();
            sende.Id = sender.Id;
            sende.Name = sender.Name;
           BO.Customer target = new();
            try
            {
               target = myComp.GetCustomer(idTarget);
                Console.WriteLine("Add done!");
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
             
            }
            BO.CustomerInParcel targe = new();
            targe.Id = target.Id;
            sende.Name = target.Name;
            ///create the new object and send it to datasource
            BO.Parcel parcel = new BO.Parcel()
            {
                Id = idC,
                Sender = sende,
                Target = targe,
                WeightCategories = (BO.Enums.WeightCategories)myWeight,
                Created = DateTime.Now,
                Assignment = DateTime.MinValue,
                Delivered = DateTime.MinValue,
                DIP = null,
                PickedUp = DateTime.MinValue,
                Priority = BO.Enums.Priorities.Normal
            };

            try
            {
                myComp.AddParcel(parcel);
                Console.WriteLine("Add done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }


        }
    }
}
