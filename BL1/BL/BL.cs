using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLAPI;
using DLAPI;
using DO;
using BO;
using System.Runtime.CompilerServices;
using static BL.BLImp;

namespace BL
{/// <summary>
/// part of BL class containing private funcs and ctor
/// 
/// </summary>
    partial class BLImp : IBL
    {
        public List<DroneToList> drones = new();
        static Random random = new Random();

        internal readonly double[] BatteryUsages;
        internal const int DRONE_FREE = 0;
        internal const int DRONE_CHARGE = 4;

        internal readonly IDal Dal;
        /// <summary>
        /// constructor BL
        /// </summary>
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly BLImp instance = new BLImp();
        }
        private static object syncRoot = new object();
        public static BLImp Instance
        {


            get
            {
                if (Nested.instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Nested.instance == null)
                            return Nested.instance;
                    }
                }
                return Nested.instance;
            }
        }
        #endregion

        private BLImp()
        {
            try
            {
                Dal = DLFactory.GetDL("2");
            }
            catch (DLConfigException ex)
            {
                throw new BLConfigException("", ex);

            }
            BatteryUsages = Dal.DroneElectricConsumations().Select(n => n).ToArray();

            IntializeDrones();


        }
        /// <summary>
        /// iniatialises the drone to list property
        /// </summary>
        /*  void IntializeDrone()
          {
              drones = (from d in Dal.GetDroneList()
                        let drone = (DO.Drone)d
                        select new DroneToList
                        {
                            Id = drone.Id,
                            Model = (Enums.DroneNames)drone.Model,
                            MaxWeight = (Enums.WeightCategories)drone.MaxWeight,
                            Valid = true
                        }).ToList();

              foreach (var drone in drones)
              {
                  try
                  {
                      if (random.NextDouble() < 0.5)
                          throw new Exception(); // to go to catch
                      drone.Location = Dal.GetBaseStation(Dal.GetDroneChargeBaseStationId(drone.Id)).Location();
                      drone.Status = Enums.DroneStatuses.Maintenance;
                      drone.Battery = 0.05 + 0.15 * random.NextDouble();
                  }
                  catch (Exception)
                  {
                      int? parcelId = Dal.GetParcelList(null).FirstOrDefault(p => p.DroneId == drone.Id
                                                                    && p.Scheduled != null
                                                                    && p.Delivered == null).Id;
                      if (parcelId != null)
                      {
                          drone.DeliveryId = parcelId;
                          drone.Status = Enums.DroneStatuses.InDelivery;
                          drone.Location = findDroneLocation(drone);
                          double minBattery = drone.RequiredBattery(this, (int)parcelId);
                          drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);
                      }
                      else
                      {
                          drone.Status = Enums.DroneStatuses.Vacant;
                          drone.Location = findDroneLocation(drone);
                          double minBattery = BatteryUsages[(int)Enums.BatteryUsage.Available] * drone.Distance(FindClosestBaseStation(drone));
                          drone.Battery = minBattery + random.NextDouble() * (1 - minBattery);
                      }
                  }
              }
          }*/


        void IntializeDrones()
        {
            double consumationFreeDrone = Dal.DroneElectricConsumations()[0];
            double consumationLightCarrier = Dal.DroneElectricConsumations()[1];
            double consumationMediumCarrier = Dal.DroneElectricConsumations()[2];
            double consumationHeavyCarrier = Dal.DroneElectricConsumations()[3];
            double chargePerHour = Dal.DroneElectricConsumations()[4];


            drones = (from drone in Dal.GetDroneList(d => d.Valid == true)
                      let dr = (DO.Drone)drone
                      select new DroneToList
                      {
                          Id = dr.Id,
                          Model = (Enums.DroneNames)dr.Model,
                          MaxWeight = (Enums.WeightCategories)dr.MaxWeight,
                          Valid = true,
                          DeliveryId = null,
                          NumOfDeliveredParcel = Dal.GetParcelList(p => p.DroneId == dr.Id && p.Delivered != null).Count()
                      }).ToList();
            foreach (var drone in drones)
            {
                if (random.NextDouble() > 0.5)
                {

                    List<DO.BaseStation> bases = (List<DO.BaseStation>)Dal.GetBaseStationsList(b => b.NumOfSlots > 0);
                    DO.BaseStation b = Dal.GetBaseStation(bases[random.Next(bases.Count() - 1)].Id);
                    drone.Location = dOBaseStation(b).Location;
                    Dal.AddDroneCharge(drone.Id, b.Id);
                    drone.Status = Enums.DroneStatuses.Maintenance;
                    drone.Battery = 5 + 15 * random.NextDouble();
                }
                else
                {

                    int? parcelId = Dal.GetParcelList(p => p.DroneId == drone.Id && p.Scheduled != null && p.Delivered == null).FirstOrDefault().Id;
                    if (parcelId != 0)
                    {
                        drone.DeliveryId = parcelId;
                        drone.Status = Enums.DroneStatuses.InDelivery;
                        drone.Location = findDroneLocation(drone);
                        double minBattery = drone.RequiredBattery(this, (int)parcelId);
                        drone.Battery = minBattery + random.NextDouble() * (100 - minBattery); 
                    }
                    else
                    {
                        drone.Status = Enums.DroneStatuses.Vacant;
                        drone.Location = findDroneLocation(drone);
                        double minBattery = BatteryUsages[(int)Enums.BatteryUsage.Available] * drone.Distances(FindClosestBaseStation(drone));
                        drone.Battery = minBattery + random.NextDouble() * (100 - minBattery);
                    }
                }

            }
        }
        /// <summary>
        /// bl function to start simulation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <param name="checkStop"></param>
        public void StartDroneSimulator(int id, Action update, Func<bool> checkStop)
        {
            new Simulator(this, id, update, checkStop);
        }
      
    }
}



