using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLAPI;
using Microsoft.Maps.MapControl.WPF;

namespace PL
{
    /// <summary>
    /// Interaction logic for MapsDisplay.xaml
    /// </summary>
    public partial class MapsDisplay : Window
    {
        IBL bl;
        private BO.Drone drone;
        private IBL bl1;
        private BO.BaseStation bs;

        public MapsDisplay(IBL IBL)
        {
            bl = IBL;
            InitializeComponent();

            Pushpin pin;
            foreach (var item in bl.GetDroneList())
            {
                pin = new();
                pin.Location = new(item.Location.Latitude, item.Location.Longitude);
                myMap.Children.Add(pin);
            }
            /*foreach (var item in bl.GetCustomerList())
            {
                pin = new();
                pin.Location = new(item., item.Latitude);
                myMap.Children.Add(pin);
            }*/
            /*

            foreach (var item in bl.GetBaseStationList())
            {
                pin = new();
                pin.Location = new(item.  .Latitude, item.DroneLocation.Longitude);
                myMap.Children.Add(pin);
            }
            */
        }

        public MapsDisplay(BO.Drone drone, IBL bl1)
        {
            this.drone = drone;
            this.bl1 = bl1;
            bl = bl1;
            InitializeComponent();


            Pushpin pin = new();
            pin.Location = new(drone.Location.Latitude, drone.Location.Longitude);
            myMap.Children.Add(pin);
        }
        public MapsDisplay(BO.BaseStation bs, IBL bl1)
        {
            this.bs = bs;
            this.bl1 = bl1;
            bl = bl1;
            InitializeComponent();

            Pushpin pushpin = new();
            pushpin.Location = new(bs.Location.Latitude, bs.Location.Longitude);
            myMap.Children.Add(pushpin);
        }
    }



}
