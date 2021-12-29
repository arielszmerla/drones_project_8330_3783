using System;
using BL;
using BLAPI;
namespace ConsoleUI_BL
{

    public partial class Program
    {


        static void Main(string[] args)
        {

            BLAPI.IBL myCompany = BLFactory.GetBL();
       
            int choic;
            int choice;
            do
            {
                Console.WriteLine("CHOOSE WHAT YOU WANT TO DO:\n" +
                    "1 TO ADD\n2 TO UPDATE \n3 TO DISPLAY AN OBJECT \n4 TO DISPLAY A LIST\n5 TO EXIT.\n");

                bool checkP = Int32.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("choose what to add:\n1 for base station\n2 for adding a drone\n" +
                        "3 to add a customer \n4 to add a parcel:\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choic);

                        addAnElement(choic, myCompany);
                        break;
                    case 2:
                        Console.WriteLine("choose what to update:\n1 for update name of a drone\n2 for update base station data\n" +
                        "3 to update customer data\n4to send a drone to charge\n5 to release drone from charging\n" +
                        "6 to assign a parcel to a drone\n7 to send a drone to pick up a parcel\n8 to deliver a parcel  :\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choic);
                        updateMenu(myCompany, choic);
                        break;
                    case 3:
                        Console.WriteLine("choose what to display and the id of what you want to be displayed:\n" +
                            "1 to display a base\n2 to display a drone\n" +
                      "3 to display a customer\n4 to display a parcel.\n");
                        checkP = Int32.TryParse(Console.ReadLine(), out choic);
                        getSingleElementChoice(choic, myCompany);
                        break;
                    case 4:
                        Console.WriteLine("choose what list to display\n1 to display the base list\n" +
                          "2 to display the drone list\n" +
                     "3 to display the customer list\n" +
                     "4 to display the parcel list\n" +
                     "5 to display the parcel not assigned list\n" +
                     "6 to display the not full base list.\n ");
       
                        checkP = Int32.TryParse(Console.ReadLine(), out choic);
                        getListsChoice(choic, myCompany);

                        break;

                }

            } while (choice != 5);
        }


    }
}

