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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLAPI;
using PL;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum listChoice { Drones, BaseStations, Customers, Parcels };
        IBL bl = BLFactory.GetBL();
        public MainWindow()
        {
            InitializeComponent();
            ViewOptions.ItemsSource = Enum.GetValues(typeof(listChoice));

        }


        private void DronesList_Click_1(object sender, RoutedEventArgs e)
        {
            new DroneListWindow1(bl).Show();
        }

        private void ViewOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listChoice stat = (listChoice)ViewOptions.SelectedItem;
            switch (stat)
            {//Drones, BaseStations, Customers, Parcels
                case
                    listChoice.BaseStations:
                    new BaseStationViewWindow(bl).Show();
                    break;
                case listChoice.Drones :
                    new DroneListWindow1(bl).Show();
                    break;
                case listChoice.Customers:
                   new CustomerListWindow(bl).Show();
                    break;
                case listChoice.Parcels:
                    new ParcelListWindow(bl).Show();
                    break;
            }
        
        }
    }
}
