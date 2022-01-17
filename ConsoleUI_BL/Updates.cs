
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleUI_BL
{/// <summary>
/// part of main helping funcs (Updaters)
/// </summary>
    public partial class Program
    {

        /// <summary>
        /// method to update a base station
        /// </summary>
        /// <param name="mycomp"></param>
        static void updateBaseStation(BLAPI.IBL mycomp)
        {
            bool checkit;
            int myId;
            do
            {
                Console.WriteLine("ENTER THE ID OF THE BASESTATION YOU WANT TO UPDATE\n");
                checkit = Int32.TryParse(Console.ReadLine(), out myId);
            } while (!checkit);
            Console.WriteLine("ENTER THE NAME OF THE BASESTATION YOU WANT TO ADD. IF YOU DON'T WANT TO UPDATE PRESS SPACE\n");
            string name = "Base " + Console.ReadLine();
            int numOfSlots;
            do
            {
                Console.WriteLine("ENTER THE NUMBER OF SLOTS IN THE BASESTATION YOU WANT TO UPDATE, IF YOU DON'T WANT TO UPDATE" +
                    "PLAESE ENTER '-1'\n");
                checkit = int.TryParse(Console.ReadLine(), out numOfSlots);
            } while (!checkit);
            try
            {
                mycomp.UpdateBaseStation(myId, numOfSlots, name);
                Console.WriteLine("Update done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }
        }
        /// <summary>
        /// update the info of a customer- his name or his phone number
        /// </summary>
        /// <param name="mycomp"></param>
        static void updateCustomerInfo(BLAPI.IBL mycomp)
        {
            bool checkC;
            int idC;
            do
            {
                Console.WriteLine("PLEASE ENTER CUSTOMER'S ID:\n");
                checkC = Int32.TryParse(Console.ReadLine(), out idC);
            } while (!checkC);
            Console.WriteLine("PLEASE ENTER CUSTOMER'S NAME. IF YOU DON'T WANT TO UPDATE, PLEASE PRESS 'SPACE'\n");
            string name = Console.ReadLine();
            Console.WriteLine("PLEASE ENTER CUSTOMER'S PHONE NUMBER. IF YOU DON'T WANT TO UPDATE, PLEASE PRESS 'SPACE'\n");
            string phone = Console.ReadLine();
            try
            {
                mycomp.UpdateCustomerInfo(idC, name, phone);
                Console.WriteLine("Update done!");
            }
            catch (BO.AddException p)
            {
                Console.WriteLine(p);
            }
        }

        /// <summary>
        /// function for all drone updates
        /// </summary>
        /// <param name="myComp"></param>
        /// <param name="a"></param>
        static void updateMenu(BLAPI.IBL myComp, int a)
        {

            bool checkC;
            int id;


            switch (a)
            {
                case 1:
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine("ENTER THE MODEL OF THE DRONE YOU WANT TO UPDATE\n" +
                    "CHOOSE FROM THOSE TYPES:\n");
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine((BO.Enums.DroneNames)i);
                    }
                   int  model =int.Parse( Console.ReadLine());
                    try
                    {
                        myComp.UpdateNameDrone(id, (BO.Enums.DroneNames)model);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }

                    break;
                case 2:
                    try
                    {

                        updateBaseStation(myComp);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }
                    
                    break;
                case 3:
                    try
                    {
                        updateCustomerInfo(myComp);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }
                    break;
                case 4:
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);
                    try
                    {
                        myComp.UpdateDroneSentToCharge(id);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }
                    break;
                case 5:
                    TimeSpan duration = new TimeSpan();
                    do
                    {
                        Console.WriteLine("PLEASE ENTER AMOUNT OF TIME LEFT FOR CHARGING:\n");
                        checkC = TimeSpan.TryParse(Console.ReadLine(), out duration);
                    } while (!checkC);
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);
                    try
                    {
                        myComp.UpdateReleaseDroneFromCharge(id, duration);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }

                    break;
                case 6:
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);
                    try
                    {
                        myComp.UpdateAssignParcelToDrone(id);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }

                    break;
                case 7:
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);

                    try
                    {
                        myComp.UpdateDroneToPickUpAParcel(id);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }
                    break;
                case 8:
                    Console.WriteLine("PLEASE ENTER DRONE'S ID:\n");
                    checkC = Int32.TryParse(Console.ReadLine(), out id);
                    try
                    {
                        myComp.UpdateDeliverParcel(id);
                        Console.WriteLine("Update done!");
                    }
                    catch (BO.AddException p)
                    {
                        Console.WriteLine(p);
                    }

                    break;

                default:
                    break;
            }

        }

    }
}
