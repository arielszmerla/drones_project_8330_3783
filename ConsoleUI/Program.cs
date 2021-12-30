using System;

using DLAPI;
using DO;
namespace ConsoleUI
{

    class Program
    {
        public enum FirstChoice { add = 1, update, display, listDisplay, exit };
        public enum Add { addBase = 1, addDrone, addCustomer, addParcel };
        public enum Update { assignParcelToDrone = 1, dronePickUp, sendDroneCharge, releaseDroneCharge };
        public enum Display { baseDisplay = 1, droneDisplay, customerDisplay, parcelDisplay };
        public enum ListDisplay { baseDisplayList = 1, droneDisplayList, customerDisplayList, parcelDisplayList, parcelNotAssigned, baseNotFullListDisplay };
        /// <summary>
        /// func that calculates distance betweeen two points on the earth globus
        ///knowing their coordinates
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>$(SolutionDir)
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        static double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double myPI = 0.017453292519943295;    // Math.PI / 180
            double a = 0.5 - Math.Cos((lat2 - lat1) * myPI) / 2 +
                    Math.Cos(lat1 * myPI) * Math.Cos(lat2 * myPI) *
                    (1 - Math.Cos((lon2 - lon1) * myPI)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
        /// <summary>
        /// gets and hours/mins / sec presentation to coordinate
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static void printLat(double num)
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

            Console.WriteLine("LATITUDE IS:  " + hours + "° " + minute + "' " + secs + (char)34 + " " + coordin + "\n");

        }
        /// <summary>
        /// gets and hours/mins / sec presentation to coordinate
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static void printLong(double num)
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
            secs /= 100;
            Console.WriteLine("LONGITUDE IS:  " + hours + "° " + minute + "' " + secs + (char)34 + " " + coordin + "\n");

        }
        /// method to ask from datasource a parcel's data 
        /// <param name="myComp"></DalObject.DalObject>
        /// <returns></IDAL.DO.Parcel>
        static DO.Parcel parcelDisplayMain(IDal myComp)
        {
            Console.WriteLine("ENTER THE ID OF THE PARCEL YOU WANT TO DISPLAY\n");
            bool checkP;
            int idP;
            checkP = Int32.TryParse(Console.ReadLine(), out idP);
            return myComp.GetParcel(idP);
        }
        /// method to ask from datasource a drone's data
        /// <param name="myComp"></DalObject.DalObject>
        /// <returns></IDAL.DO.Drone>
        static DO.Drone droneDisplayMain(IDal myComp)
        {
            bool checkC;
            int idC;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE DRONE YOU WANT TO DISPLAY\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idC);
            } while (!checkC);
            return myComp.GetDrone(idC);
        }
        /// method to ask from datasource a customer's data
        /// <param name="myComp"></DalObject.DalObject>
        /// <returns></ IDAL.DO.Customer>
        static DO.Customer customerDisplayMain(IDal myComp)
        {
            bool check;
            int iD;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE CUSTOMER YOU WANT TO DISPLAY\n");
                check = Int32.TryParse(Console.ReadLine(), out iD);
            } while (!check);
            return myComp.GetCustomer(iD);
        }
        /// method to ask from database a base station's data
        /// <param name="myComp"></DalObject.DalObject>
        /// <returns></IDAL.DO.BaseStation>
        static DO.BaseStation baseDisplayMain(IDal myComp)
        {
            int idBD;
            bool checkDis;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE BASESTATION YOU WANT TO DISPLAY\n");
                checkDis = Int32.TryParse(Console.ReadLine(), out idBD);
            } while (!checkDis);
            return myComp.GetBaseStation(idBD);
        }
        /// method to assign a parcel to a drone in datasource
        /// <param name="myComp"></void>
        static void assignParcelToDrone(IDal myComp)
        {
            bool checkP, checkID;
            int idP, idDrone;
            do
            {
                Console.WriteLine("ENTER THE IDs OF THE PARCEL AND THE DRONE YOU WANT TO CONNECT\n");
                checkP = Int32.TryParse(Console.ReadLine(), out idP);
                checkID = Int32.TryParse(Console.ReadLine(), out idDrone);

            } while (!checkP || !checkID);
            // after recieving id of a parcel and drone we send them to a function in DalObjects that assigns the two together
            myComp.UpdateParcelToDrone(idP, idDrone);

        }
        /// <summary>
        /// method to schedule drone to pickup parcel
        /// <param name="myComp"></param>
        /// return void
        static void dronePickUp(IDal myComp)
        {
            bool checkP;
            int idP;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE DRONE YOU WANT TO SET PICKUP TIME\n");
                checkP = Int32.TryParse(Console.ReadLine(), out idP);

            } while (!checkP);
            myComp.UpdateDronePickUp(idP);
        }
        /// <summary>
        /// assign a drone to charging place
        /// <param name="myComp"></param>
        /// return void
        static void sendDroneToCharge(IDal myComp)
        {
            // print out all base statione choices for charging
            Console.WriteLine("choose from following base station's NAME for charging\n");
            foreach (var Base in myComp.GetBaseStationsList(Base=> Base.NumOfSlots > 0))
            {
                
                    Console.WriteLine(Base);
            }
            string baseName = Console.ReadLine();
            bool checkP;
            int idP;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE DRONE YOU WANT TO SEND TO CHARGE\n");
                checkP = Int32.TryParse(Console.ReadLine(), out idP);


            } while (!checkP);
            myComp.UpdateDroneToCharge(idP, baseName);
        }
        /// <summary>
        /// method to set a parcel's delivery time
        /// </summary>
        /// <param name="myComp"></param>
        static void parcelDelivery(IDal myComp)
        {
            bool checkP;
            int idP;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE PARCEL YOU WANT TO SET DELIVERY TIME\n");
                checkP = Int32.TryParse(Console.ReadLine(), out idP);

            } while (!checkP);
            myComp.UpdatesParcelDelivery(idP);
        }
        /// <summary>
        /// method to release drone from chargin
        /// </summary>
        /// <param name="myComp"></param>
        static void releaseDroneCharge(IDal myComp)
        {
            Console.WriteLine("ENTER THE NAME OF THE BASESTATION YOU WANT TO RELEASE DRONE FROM\n"); ;
            string baseName = Console.ReadLine();
            bool checkP;
            int idP;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE DRONE YOU WANT TO RELEASE\n"); ;
                checkP = Int32.TryParse(Console.ReadLine(), out idP);
            } while (!checkP);
            myComp.UpdateReleasDroneCharge(idP, baseName);
        }
        /// <summary>
        /// method that gets data from the user to add a new basestation in company database
        /// </summary>
        /// <param name="myComp"></param>
        static void addBaseMain(IDal myComp)
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
            int numOfSlots;
            do
            {
                Console.WriteLine("ENTER THE NUMBER OF SLOTS IN THE BASESTATION YOU WANT TO ADD\n");
                checkit = int.TryParse(Console.ReadLine(), out numOfSlots);
            } while (!checkit);
            ///create the new object and send it to datasource
            DO.BaseStation bas = new DO.BaseStation()
            {
                Id = myId,
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                NumOfSlots = numOfSlots
            };
            myComp.AddBaseStation(bas);
        }
        /// <summary>
        /// method that gets data from the user to add a new drone in company database
        /// </summary>
        /// <param name="myComp"></param>
        static void addDroneMain(IDal myComp)
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
                Console.WriteLine((DO.DroneNames)i);
            }
            int model = int.Parse(Console.ReadLine());


            Console.WriteLine("ENTER THE WEIGHT CATEGORIE OF THE DRONE YOU WANT TO ADD\n" +
                "CHOOSE FROM THOSE TYPES:\n");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine((DO.WeightCategories)i);
            }
            DO.WeightCategories categ;
            do
            {
                check = DO.WeightCategories.TryParse(Console.ReadLine(), out categ);
            } while (!check);
            ///create the new object and send it to datasource
            DO.Drone drone = new DO.Drone()
            {
                Id = id,
                Model = (DO.DroneNames)model,
                MaxWeight = categ,
                Valid = true



            };
            myComp.AddDrone(drone);
        }
        /// <summary>
        /// method that gets data from the user to add a new customer in company database
        /// </summary>
        /// <param name="myComp"></param>
        static void addCustomerMain(IDal myComp)
        {

            bool checkC;
            int idC;
            do
            {
                Console.WriteLine("PLEASE ENTER CUSTOMER'S ID:\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idC);
            } while (!checkC);
            idC = DO.StringAdapter.lastDigitID(idC);
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
            DO.Customer customer = new DO.Customer()
            {
                Id = idC,
                Name = name,
                Phone = phone,
                Longitude = longitude,
                Latitude = latitude
            };
            (myComp).AddCustomer(customer);
        }
        /// <summary>
        /// method that gets data from the user to add a new parcel in company database
        /// </summary>
        /// <param name="myComp"></param>
        /// return index of new parcel
        static int addParcelMain(IDal myComp)
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
            DO.Priorities priority;
            do
            {
                checkC = DO.Priorities.TryParse(Console.ReadLine(), out priority);
            } while (!checkC);
            ///create the new object and send it to datasource
            DO.Parcel parcel = new DO.Parcel()
            {
                Id = idC,
                SenderId = idSender,
                TargetId = idTarget,
                // Weight = myWeight,
                Delivered = DateTime.MinValue,
                Scheduled = DateTime.MinValue,
                PickedUp = DateTime.MinValue,
                DroneId = 0,
                Requested = DateTime.MinValue,
                //  Priority = priority
            };
            return myComp.AddParcel(parcel); ;
        }

        static void Main(string[] args)
        {

            IDal MyCompany = DLFactory.GetDL("1"); ;

            int choice = 0;
            do
            {
                Console.WriteLine("CHOOSE WHAT YOU WANT TO DO:\n" +
                    "1 TO ADD\n2 TO UPDATE \n3 TO DISPLAY AN OBJECT \n4 TO DISPLAY A LIST\n5 TO PRINT YOUR CHOICE'S COORDINATE TO A PLACE DISTANCE \n6 TO EXIT.\n");


                bool checkP = Int32.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("choose what to add:\n1 for base station\n2 for adding a drone\n" +
                        "3 to add a customer \n4 to add a parcel:\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choice);

                        switch (choice)
                        {
                            case 1:
                                addBaseMain(MyCompany);
                                break;
                            case 2:
                                addDroneMain(MyCompany);
                                break;
                            case 3:
                                addCustomerMain(MyCompany);
                                break;
                            case 4:
                                int index = addParcelMain(MyCompany);
                                break;
                        }
                        break;

                    case 2:
                        Console.WriteLine("Choose what to update,\n1 to assign parcel to drone\n" +
                            "2 to pick up a parcel,\n3 to deliver parcel to customer, \n" +
                        "4 to send a drone to charge,\n5 to release a drone from charging.\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choice);
                        switch (choice)
                        {
                            case 1:
                                assignParcelToDrone(MyCompany);
                                break;
                            case 2:
                                dronePickUp(MyCompany);
                                break;
                            case 3:
                                parcelDelivery(MyCompany);
                                break;
                            case 4:
                                sendDroneToCharge(MyCompany);
                                break;
                            case 5:
                                releaseDroneCharge(MyCompany);
                                break;
                        }
                        break;
                    case 3:
                        Console.WriteLine("choose what to display:\n1 to display a base\n" +
                           "2 to display a drone\n" +
                      "3 to display a customer\n4 to display a parcel.\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choice);
                        switch (choice)
                        {
                            case 1:
                                Console.WriteLine(baseDisplayMain(MyCompany));
                                break;
                            case 2:
                                Console.WriteLine(droneDisplayMain(MyCompany));
                                break;
                            case 3:
                                Console.WriteLine(customerDisplayMain(MyCompany));
                                break;
                            case 4:
                                Console.WriteLine(parcelDisplayMain(MyCompany));
                                break;
                        }
                        break;
                    case 4:
                        Console.WriteLine("choose what list to display\n1 to display the base list\n" +
                          "2 to display the drone list\n" +
                     "3 to display the customer list,\n " +
                     "4 to display the parcel list,\n" +
                     "5 to display the parcel not assigned list\n" +
                     "6 to display the not full base list.\n ");
                        checkP = Int32.TryParse(Console.ReadLine(), out choice);
                        switch (choice)
                        {
                            case 1:

                                foreach (var Base in MyCompany.GetBaseStationsList(null))
                                {
                                    Console.WriteLine(Base);
                                }
                                break;
                            case 2:
                                foreach (var drone in MyCompany.GetDroneList())
                                {
                                    Console.WriteLine(drone);
                                }
                                break;
                            case 3:
                                foreach (var customer in MyCompany.GetCustomerList())
                                {
                                    Console.WriteLine(customer);
                                }
                                break;
                            case 4:
                                foreach (var parcel in MyCompany.GetParcelList(null))
                                {
                                    Console.WriteLine(parcel);
                                }
                                break;
                            case 5:
                                foreach (var parcel in MyCompany.GetParcelList(parcel=> parcel.DroneId == 0))
                                {
                                  
                                        Console.WriteLine(parcel);
                                }
                                break;
                            case 6:
                                foreach (var Base in MyCompany.GetBaseStationsList(b=>b.NumOfSlots>0))
                                {
                                  
                                        Console.WriteLine(Base);
                                }
                                break;
                        }
                        break;
                    case 5:
                        Console.WriteLine("ENTER COORDINATES YOU CHOICE:\n" +
                            "LATITUDE:\n");
                        bool checkit;
                        double longitude;
                        do
                        {
                            checkit = double.TryParse(Console.ReadLine(), out longitude);
                        } while (!checkit);

                        Console.WriteLine(
                           "LONGITUDE:\n");
                        double latitude;
                        do
                        {
                            checkit = double.TryParse(Console.ReadLine(), out latitude);
                        } while (!checkit);

                        Console.WriteLine("IF YOU WANT TO CALCULATE DISTANCE FORM BASESTATION PRESS 1\nIF YOU WANT TO CALCULATE DISTANCE FORM CUSTOMER PRESS 2\n");
                        int choose = int.Parse(Console.ReadLine());
                        if (choose == 1)
                        {
                            Console.WriteLine("CHOOOSE FROM THOSE:\n");
                            foreach (var myBases in MyCompany.GetBaseStationsList(null))
                            {
                                Console.WriteLine(myBases);
                            }
                            string choiceMade = Console.ReadLine();
                            DO.BaseStation myBase = baseDisplayMain(MyCompany);

                            Console.WriteLine(distance(latitude, longitude, myBase.Latitude, myBase.Longitude) + " km.\n");
                        }
                        if (choose == 2)
                        {
                            Console.WriteLine("CHOOOSE FROM THOSE:\n");
                            foreach (DO.Customer customer in MyCompany.GetCustomerList())
                            {
                                Console.WriteLine(customer);
                            }
                            string choiceMade = Console.ReadLine();
                          DO.Customer cus = customerDisplayMain(MyCompany);
                            Console.WriteLine(distance(latitude, longitude, cus.Latitude, cus.Longitude) + " km.\n");
                        }

                        break;
                    case 6:
                        Console.WriteLine("Bye\n");
                        return;

                }
            } while (1 == 1);
        }
    }
}
