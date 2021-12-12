using System;

namespace IBL
{
    namespace BO
    {/// <summary>
    /// Enums needed to BL class
    /// </summary>
        public class Enums
        {
            /// <summary>
            /// enums containers for use in other classes in this namespace
            /// </summary>
            public enum BaseStationEmplacemt { TelAviv, PetachTikva, Holon, BneBrak, RamatGan }
            public enum DroneEmplacement { TelAvivCentral, TelAvivAzrieli, BneBrak, Holon, TelAvivUniversity, RamatGanCenter, PetacTikva, BarIlan, TelAvivDizengoff, Ponevezh }
            public enum DroneNames { PHANTOM, MAVIC, RIZY }
            public enum WeightCategories { Light, Medium, Heavy,Unknown }
            public enum DroneStatuses  {  Vacant, Maintenance ,InDelivery }
             public enum ParcelStatus { Created, Assigned, PickedUp, Delivered}
            public enum Priorities { Normal, Fast, Urgent }
        }
    }
}
