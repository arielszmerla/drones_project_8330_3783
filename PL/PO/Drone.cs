using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PO
{
    class Drone : INotifyPropertyChanged
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

        private int deliveryId;
        public int DeliveryId
        {
            get => deliveryId;
            set { deliveryId = value; OnPropertyChanged("deliveryId"); }
        }


        private double distance;
        public double Distance
        {
            get => distance;
            set { distance = value; OnPropertyChanged("distance"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
