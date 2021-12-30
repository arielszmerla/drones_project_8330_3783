using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI_BL
{/// <summary>
/// part of main helping funcs (Getters)
/// </summary>
    public partial class Program
    {
        #region
        public BO.Drone GetDrone(BLAPI.IBL myCompany)
        {
            bool checkP;
            Console.WriteLine("ENTER THE ID OF THE ELEMENT YOU WANT TO DISPLAY\n");
            int idP;
            checkP = Int32.TryParse(Console.ReadLine(), out idP);
            try
            {
                myCompany.GetDrone(idP);
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }
            return myCompany.GetDrone(idP);
        }

        public BO.Parcel GetParcel(BLAPI.IBL myCompany)
        {
            bool checkP;
            Console.WriteLine("ENTER THE ID OF THE ELEMENT YOU WANT TO DISPLAY\n");
            int idP;
            checkP = Int32.TryParse(Console.ReadLine(), out idP);
            try
            {
                myCompany.GetParcel(idP);
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }
            return myCompany.GetParcel(idP);
        }

        public BO.Customer GetCustomer(BLAPI.IBL myCompany)
        {
            bool checkP;
            Console.WriteLine("ENTER THE ID OF THE ELEMENT YOU WANT TO DISPLAY\n");
            int idP;
            checkP = Int32.TryParse(Console.ReadLine(), out idP);
            try
            {
                myCompany.GetCustomer(idP);
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }
            return myCompany.GetCustomer(idP);
        }

        public BO.BaseStation GetBaseStation(BLAPI.IBL myCompany)
        {
            bool checkP;
            Console.WriteLine("ENTER THE ID OF THE ELEMENT YOU WANT TO DISPLAY\n");
            int idP;
            checkP = Int32.TryParse(Console.ReadLine(), out idP);
            try
            {
                myCompany.GetBaseStation(idP);
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }
            return myCompany.GetBaseStation(idP);
        }



        #endregion

        #region
        public IEnumerable<BO.CustomerToList> GetCustomerList(BLAPI.IBL myComp)
        {
            return myComp.GetCustomerList().ToList();

        }
        public IEnumerable<BO.DroneToList> GetDroneList(BLAPI.IBL myComp)
        {
            return myComp.GetDroneList().ToList();

        }
        public IEnumerable<BO.ParcelToList> GetParcelList(BLAPI.IBL myComp)
        {
            return myComp.GetParcelList().ToList();

        }
        public IEnumerable<BO.ParcelToList> GetParcelNotAssignedList(BLAPI.IBL myComp)
        {
            return myComp.GetParcelNotAssignedList().ToList();

        }
        public IEnumerable<BO.BaseStationToList> GetBaseStationList(BLAPI.IBL myComp)
        {
            return myComp.GetBaseStationList().ToList();

        }
        public IEnumerable<BO.BaseStationToList> GetFreeSlotsBaseStationList(BLAPI.IBL myComp)
        {
            return myComp.GetListOfBaseStationsWithFreeSlots().ToList();

        }
        /// <summary>
        /// gets the list the user wants
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="myCompany"></param>
        private static void getListsChoice(int choice, BLAPI.IBL myCompany)
        {

            switch (choice)
            {
                case 1:

                    foreach (var Base in myCompany.GetBaseStationList())
                    {
                        Console.WriteLine(Base);
                    }
                    break;
                case 2:
                    foreach (var drone in myCompany.GetDroneList())
                    {
                        Console.WriteLine(drone);
                    }
                    break;
                case 3:
                    foreach (var customer in myCompany.GetCustomerList())
                    {
                        Console.WriteLine(customer);
                    }
                    break;
                case 4:
                    foreach (var parcel in myCompany.GetParcelList())
                    {
                        Console.WriteLine(parcel);
                    }
                    break;
                case 5:
                    foreach (var parcel in myCompany.GetParcelNotAssignedList())
                    {
                        Console.WriteLine(parcel);
                    }
                    break;
                case 6:
                    foreach (var Base in myCompany.GetListOfBaseStationsWithFreeSlots())
                    {
                        Console.WriteLine(Base);
                    }
                    break;
            }
        }
        /// <summary>
        /// func that get the right func for the user choice
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="myCompany"></param>
        private static void getSingleElementChoice(int choice, BLAPI.IBL myCompany)
        {
            bool checkP;
            int id;
            Console.WriteLine("ENTER THE ID OF THE ELEMENT YOU WANT TO GET\n");
            checkP = Int32.TryParse(Console.ReadLine(), out id);
            try
            {
                switch (choice)
                {

                    case 1:

                        Console.WriteLine(myCompany.GetBaseStation(id));
                        break;
                    case 2:

                        Console.WriteLine(myCompany.GetDrone(id));
                        break;
                    case 3:

                        Console.WriteLine(myCompany.GetCustomer(id));
                        break;
                    case 4:
                        Console.WriteLine(myCompany.GetParcel(id));
                        break;
                }
            }
            catch (BO.GetException p)
            {
                Console.WriteLine(p);
            }

        }
    }
    #endregion
}

