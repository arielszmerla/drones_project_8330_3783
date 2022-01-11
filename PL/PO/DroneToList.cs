using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace PO
{
    public class DroneToList : INotifyPropertyChanged
    {
        private int id;
        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged("id"); }
        }

        private string model;
        public string Model
        {
            get => model;
            set { model = value; OnPropertyChanged("model"); }
        }

        private Enums.WeightCategories maxWeight;
        public Enums.WeightCategories MaxWeight
        {
            get => maxWeight;
            set { maxWeight = value; OnPropertyChanged("weight"); }
        }

        private double battery;
        public double Battery
        {
            get => battery;
            set { battery = value; OnPropertyChanged("battery"); }
        }

      

        private Enums.DroneStatuses status;
        public Enums.DroneStatuses Status
        {
            get => status;
            set { status = value; OnPropertyChanged("status"); }
        }

        private Location location;
        public Location Location
        {
            get => location;
            set { location = value; OnPropertyChanged("location"); }
        }

        private int numOfDeliveredParcel;
        public int NumOfDeliveredParcel
        {
            get => numOfDeliveredParcel;
            set { numOfDeliveredParcel = value; OnPropertyChanged("numOfDeliveredParcel"); }
        }


        private int? deliveryId;
        public int? DeliveryId
        {
            get => deliveryId;
            set { deliveryId = value; OnPropertyChanged("deliveryId"); }
        }

      

        public override string ToString()
        {
            string result = "";
            result += $"{"Drone ID is:",-30} {id,44}\n";
            result += $"{"Model name is:",-30} {model,41}\n";
            result += $"{"Maximal transported Weight:",-30} {maxWeight,30}\n";
            result += $"{"Status is:",-30} {status,44}\n";
            result += $"{"Battery level is:",-30} {(int)battery,40}%\n";
            result += $"Emplacement is: \n" + Location;
            result += $"{"Number of delivered parcels is:",-30} {NumOfDeliveredParcel,30}\n";
            for (int i = 0; i < 60; i++)
            {
                result += "_";
            }
            return result;
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}