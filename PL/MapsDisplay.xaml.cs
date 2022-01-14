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
        public static Model Model { get; } = Model.Instance;
        IBL bl;
        private BO.Drone drone;
        private IBL bl1;
        private BO.BaseStation bs;
        void Window_Loaded(object sender, RoutedEventArgs e) => Model.DronesRefresh();
        public MapsDisplay(IBL IBL)
        {
            bl = IBL;
            InitializeComponent();

            Pushpin pin;
            foreach (var item in Model.Drones)
            {
                pin = new();
                pin.Location = new(item.Location.Latitude, item.Location.Longitude);
                ToolTipService.SetToolTip(pin, item);
                MapLayer.SetPosition(pin, pin.Location);
                // var mytemplate As System.Windows.Controls.ControlTemplate = FindResource("PushpinControlTemplate");
                pin.MouseDoubleClick += PinClicked;
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

        private void PinClicked(object sender, System.EventArgs e)
        {
           Pushpin id =(Pushpin)sender;
          BO. DroneToList d=  bl.GetDroneList(null).FirstOrDefault(p => p.Location.Latitude == id.Location.Latitude);
            new AddDrone(bl, d.Id).Show();
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
            ToolTipService.SetToolTip(pin, pin.Location);
            MapLayer.SetPosition(pin, pin.Location);

        }
        public MapsDisplay(BO.BaseStation bs, IBL bl1)
        {
            this.bs = bs;
            this.bl1 = bl1;
            bl = bl1;
            InitializeComponent();

            Pushpin pin = new();
            pin.Location = new(bs.Location.Latitude, bs.Location.Longitude);
            myMap.Children.Add(pin);
            ToolTipService.SetToolTip(pin, pin.Location);
            MapLayer.SetPosition(pin, pin.Location);

        }
    }



}
