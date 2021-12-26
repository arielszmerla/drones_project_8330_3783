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
        public static List<Drone> Drones = new List<Drone>();
        public static List<BaseStation> BaseStations = new List<BaseStation>();
        static public List<Customer> Customers = new List<Customer>();
        static public List<Parcel> Parcels = new List<Parcel>();
        static public List<DroneCharge> DroneCharges = new List<DroneCharge>();
        public class Config//starting company with value
        {
            //indexes showing amount of items in arrays
            static public double powerUseFreeDrone = 0.5;
            static public double powerUseLightCarrying = 0.7;
            static public double powerUseMediumCarrying = 0.9;
            static public double powerUseHeavyCarrying = 1;
            static public double chargePerHour = 35;//השאלה האם צריך static
            static public int totalNumOfParcels = 0;
            /// <summary>
            /// function that intialize the datasource (company main data) with randomal but
            /// logical states
            /// </summary>
            static Random random = new Random();
            static public void Initialize()
            {
                BaseStation B = new BaseStation()///create new objects as asked
                {
                    Id = 1 + 9999,
                    //Israel coordinates range  31.740967, 35.171323
                    Latitude = 31.745156,
                    Longitude =
                    35.1751589,
                    Name = "Base " + (DO.BaseStationEmplacemt)(0), // baseInput,
                    NumOfSlots = random.Next(2, 4),
                    Valid = true,
                };
                BaseStations.Add(B);
                B = new BaseStation()///create new objects as asked
                { Valid=true,
                    Id = 2 + 9999,
                    //Israel coordinates range  31.740967, 35.171323
                    Latitude = 31.8099077,
                    Longitude = 35.1855049,
                    Name = "Base " + (DO.BaseStationEmplacemt)(1), // baseInput,
                    NumOfSlots = random.Next(2, 7)
                        
                };
                BaseStations.Add(B);
                B = new BaseStation()///create new objects as asked
                {
                    Id = 3 + 9999,
                    Valid = true,
                    Latitude = 31.7861624,
                    Longitude = 
                    35.1822896,
                    
                    Name = "Base " + (DO.BaseStationEmplacemt)(2), // baseInput,
                    NumOfSlots = random.Next(2, 8)
                };
                BaseStations.Add(B);
                BaseStations.Add(new BaseStation
                {
                    Id = 4 + 9999,
                    Valid = true,
                    Latitude = 31.7897954,
                    Longitude =
                    35.202992,
                    Name = "Base " + (DO.BaseStationEmplacemt)(3), // baseInput,
                    NumOfSlots = random.Next(2, 8)
                });
                BaseStations.Add(new BaseStation
                {
                    Id = 5 + 9999,
                    Valid = true,
                    Latitude = 31.7835426,
                    Longitude = 35.208293,
                    Name = "Base " + (DO.BaseStationEmplacemt)(5), // baseInput,
                    NumOfSlots = random.Next(2, 8)
                });
                BaseStations.Add(new BaseStation
                {
                    Id = 6 + 9999,
                    Valid = true,
                    Latitude = 31.764764,
                    Longitude = 35.189358,
                    Name = "Base " + (DO.BaseStationEmplacemt)(4), // baseInput,
                    NumOfSlots = random.Next(2, 8)
                });

                DO.Drone d = new Drone///create new objects as asked
                {
                    Id = 100001,
                    Model = ((DO.DroneNames)random.Next(3)),
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                    Valid = true,
                };
                
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100002,
                    Model = ((DO.DroneNames)random.Next(3)),
                    Valid = true,
                    MaxWeight = (DO.WeightCategories)random.Next(3),

                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100003,
                    Model = ((DO.DroneNames)random.Next(3)),
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                    Valid = true,
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100004,
                    Model = ((DO.DroneNames)random.Next(3)),
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                    Valid = true,
                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100005,
                    Model = ((DO.DroneNames)random.Next(3)),
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                    Valid = true,

                };
                Drones.Add(d);
                d = new Drone()///create new objects as asked
                {
                    Id = 100006,
                    Model = ((DO.DroneNames)random.Next(3)),
                    MaxWeight = (DO.WeightCategories)random.Next(3),
                    Valid = true,
                };
                Drones.Add(d);
                int customersInput;

                for (customersInput = 9; customersInput >= 0; customersInput--)  // 10 min, 100 max values to initialize
                    Customers.Add(new Customer()///create new objects as asked
                    {
                        Id =(100000000+customersInput),
                        //Israel coordinates range  31.740967, 35.171323
                        Latitude = (double)random.Next(31740967, 31815177) / (double)1000000,
                        Longitude = (double)random.Next(35171323, 35202050) / (double)1000000,
                        Name = "myName" + customersInput,
                        Phone = $"{random.Next(100000000)}",
                    });
                int parcelsInput = 51;
                totalNumOfParcels += 52;

                TimeSpan duration = new TimeSpan(random.Next(2), random.Next(60), random.Next(60));
                for (int i = 0; i < parcelsInput; i++) // 10 min, 100-0 max values to initialize
                {
                    DO.Parcel p = new Parcel()///create new objects as asked
                    {
                        DroneId = 0,
                        Id = i + 2,
                        Priority = (DO.Priorities)random.Next(3),
                        Weight = (DO.WeightCategories)random.Next(3),
                        SenderId = Customers[random.Next(5)].Id,
                        TargetId = Customers[random.Next(0, 6)].Id,
                        Requested = new DateTime(2021, 12, 30, 9, 0, 00) + duration,
                        Scheduled = new DateTime(2021, 12, 30, 9, 0, 00) + duration,
                        PickedUp = new DateTime(2021, 12, 30, 10, 0, 00) + duration,
                        Delivered = new DateTime(2021, 12, 30, 12, 0, 00) + duration
                    };
                    Parcels.Add(p);
                }
            }
        }
    }
}
