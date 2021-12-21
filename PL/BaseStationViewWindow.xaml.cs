using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Runtime.CompilerServices;

namespace PL
{
    /// <summary>
    /// Interaction logic for BaseStationViewWindow.xaml
    /// </summary>
    public partial class BaseStationViewWindow : Window
    {
        private BLAPI.IBL bl;
        // enum options {Free_Base_Stations, Num_Of_Free_Bases }

        public BaseStationViewWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
        
            BaseStationView.ItemsSource = bl.GetBaseStationList();
            DataContext = BaseStationView.ItemsSource;
            BaseOptions.Items.Add("NumOfFreeSlots");
            BaseOptions.Items.Add("");
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void ResetList_Click(object sender, RoutedEventArgs e)
        {
            BaseStationView.ItemsSource = bl.GetBaseStationList();
            ResetList.Visibility = Visibility.Hidden;
        }

        private void BaseStationView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           BO.BaseStation bs = (BO.BaseStation)BaseStationView.SelectedItem;
            BO.BaseStation baseStation = new BO.BaseStation
            {
                Id = bs.Id,
                BaseStationLocation = bs.BaseStationLocation,
                ChargingDrones = bs.ChargingDrones,
                Name = bs.Name,
                NumOfFreeSlots = bs.NumOfFreeSlots
            };
            
            //BO.Drone dr = new BO.Drone
            //{
            //    Id = drone.Id,
            //    BatteryStatus = drone.BatteryStatus,
            //    DronePlace = drone.DroneLocation,
            //    MaxWeight = drone.MaxWeight,
            //    Model = drone.Model,
            //    PID = null,
            //    Status = drone.Status
            //};
            //Closing_Button.Visibility = Visibility.Hidden;
            //new AddDrone(bl1, dr).Show();
            //Close();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

      

        private void BaseOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BaseStationView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription(BaseOptions.SelectedItem.ToString());
            view.GroupDescriptions.Add(groupDescription);
            BaseStationView.Items.Refresh();
            ResetList.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            new AddBaseStation(bl).Show();
        }

       
    }
}
