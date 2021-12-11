using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS
{
   
    /// <summary>
    /// class that contains object 'company' with struct and init func
    /// </summary>
    public static class DataSource//storage class for the whole company
    {
        //arrays for each item
        static internal List<Drone> Drones = new List<Drone>();
        static internal List<BaseStation> BaseStations = new List<BaseStation>();
        static internal List<Customer> Customers = new List<Customer>();
        static internal List<Parcel> Parcels = new List<Parcel>();
        static internal List<DroneCharge> DroneCharges = new List<DroneCharge>();
        internal class Config//starting company with value
        {
            //indexes showing amount of items in arrays
            static internal double powerUseFreeDrone= 0.5;
            static internal double powerUseLightCarrying=0.7;
            static internal double powerUseMediumCarrying=0.9;
            static internal double powerUseHeavyCarrying=1;
            static internal double chargePerHour= 35;//השאלה האם צריך static
            static internal int totalNumOfParcels = 0;
            /// <summary>
            /// function that intialize the datasource (company main data) with randomal but
            /// logical states
            /// </summary>
            static Random random = new Random();
            static internal void Initialize()
            {
                BaseStation B = new BaseStation()///create new objects as asked
                {
                    Id = 1 + 9999,
                    //Israel coordinates range

                    Latitude = (double)random.Next(29000000, 34000000) / (double)1000000,
                    Longitude = (double)random.Next(34000000, 35000000) / (double)1000000,
                    Name = "Base " + (DO.BaseStationEmplacemt)(0), // baseInput,
                    NumOfSlots = random.Next(2, 4)
                };
                BaseStations.Add(B);
                B = new BaseStation()///create new objects as asked
                {
                    Id = 2 + 9999,
                    //Israel coordinates range

                    Latitude = (double)random.Next(29000000, 34000000) / (double)1000000,
                    Longitude = (double)random.Next(34000000, 35000000) / (double)1000000,
                    Name = "Base " + (DO.BaseStationEmplacemt)(1), // baseInput,
                    NumOfSlots = random.Next(2, 4)
                };
                BaseStations.Add(B);
                B = new BaseStation()///create new objects as asked
                {
                    Id = 3 + 9999,
                    //Israel coordinates range

                    Latitude = (double)random.Next(29000000, 34000000) / (double)1000000,
                    Longitude = (double)random.Next(34000000, 35000000) / (double)1000000,
                    Name = "Base " + (DO.BaseStationEmplacemt)(2), // baseInput,
                    NumOfSlots = random.Next(2, 4)
                };
                BaseStations.Add(B);
                DO.Drone d = new Drone()///create new objects as asked
                {
                    Id =100001,
                    Model = ((DO.DroneNames)random.Next(3)).ToString() + " " + 1,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100002,
                    Model = ((DO.DroneNames)random.Next(3)).ToString() + " " + 2,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                  
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100003,
                    Model =((DO.DroneNames)random.Next(3)).ToString() + " " + 3,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
   
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100004,
                    Model = ((DO.DroneNames)random.Next(3)).ToString() + " " + 4,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100005,
                    Model = ((DO.DroneNames)random.Next(3)).ToString() + " " + 5,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
             
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100006,
                    Model = ((DO.DroneNames)random.Next(3)).ToString() + " " + 6,
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                };
                Drones.Add(d);
                int customersInput;
  
                for (customersInput = 11; customersInput >= 0; customersInput--)  // 10 min, 100 max values to initialize
                    Customers.Add(new Customer()///create new objects as asked
                    {
                        Id =DO.StringAdapter.lastDigitID( customersInput),
                        //Israel coordinates range
                        Latitude = (double)random.Next(29000000, 34000000) / (double)1000000,
                        Longitude = (double)random.Next(34000000, 35000000) / (double)1000000,
                        Name = "myName" + customersInput,
                        Phone = $"{random.Next(100000000)}",
                    });
                int parcelsInput = 101;
                totalNumOfParcels += 102;

                TimeSpan duration = new TimeSpan(random.Next(2), random.Next(60), random.Next(60));
                for (int i = 0; i < parcelsInput; i++) // 10 min, 100-0 max values to initialize
                {
                        DO.Parcel p = new Parcel()///create new objects as asked
                    {
                        DroneId = 0,
                        Id = i + 2,
                        Priority = (DO.Priorities)random.Next(3),
                        SenderId = Customers[random.Next(5)].Id,
                        TargetId = Customers[random.Next(0, 6)].Id,
                        Requested = new DateTime(2021, 12, 30, 9, 0, 00) + duration,
                        Scheduled = new DateTime(2021, 12, 30,9, 0, 00) + duration,
                        PickedUp = new DateTime(2021, 12, 30,10, 0, 00) + duration,
                        Delivered = new DateTime(2021, 12, 30, 12, 0, 00) +duration
                     
                    };
                    Parcels.Add(p);
                }
            }
        }
    }
}
