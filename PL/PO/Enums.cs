﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PO
{
    public class Enums
    {
        /// <summary>
        /// enums containers for use in other classes in this namespace
        /// </summary>
        public enum BaseStationEmplacemt { TelAviv, PetachTikva, Holon, BneBrak, RamatGan }
        public enum DroneEmplacement { TelAvivCentral, TelAvivAzrieli, BneBrak, Holon, TelAvivUniversity, RamatGanCenter, PetacTikva, BarIlan, TelAvivDizengoff, Ponevezh }
        public enum DroneNames { PHANTOM, MAVIC, RIZY }
        public enum WeightCategories { Light, Middle, Heavy }
        public enum DroneStatuses { Vacant, Maintenance, InDelivery }
        public enum ParcelStatus { Created, Assigned, PickedUp, Delivered }
        public enum Priorities { Normal, Fast, Urgent }
        public enum BatteryUsage { Available, Light, Middle, Heavy, Charging }
    }
}
