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
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            PageStop.Visibility = Visibility.Hidden;
            new MainWindow().Show();
            Close();

        }
        private void BaseSViewClosing(object sender, CancelEventArgs e)
        {
            if (PageStop.Visibility != Visibility.Hidden)
                e.Cancel = true;

        }

        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bl).Show();
        }


        private void ResetList_Click(object sender, RoutedEventArgs e)
        {
            BaseStationView.ItemsSource = bl.GetBaseStationList();
            ResetList.Visibility = Visibility.Hidden;
        }

        private void BaseStationView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.BaseStationToList bs = (BO.BaseStationToList)BaseStationView.SelectedItem;
            if (bs == null)
            {
                MessageBox.Show("Please click on a base station");
            }
            else
            {
                BO.BaseStation baseStation = new BO.BaseStation
                {
                    Id = bs.Id,
                    Name = bs.Name,
                    NumOfFreeSlots = bs.NumOfFreeSlots,
                    Location = bs.Location,
                    ChargingDrones = bs.ChargingDrones
                };
                PageStop.Visibility = Visibility.Hidden;
                new AddBaseStation(bl, baseStation).Show();
                Close();
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



    
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PageStop.Visibility = Visibility.Hidden;
            new AddBaseStation(bl).Show();
            Close();

        }

        private void BaseOptions_Click(object sender, RoutedEventArgs e)
        {
            BaseStationView.ItemsSource = bl.GetBaseStationListGroup();
            BaseStationView.Items.Refresh();
            ResetList.Visibility = Visibility.Visible;
        }
    }
}
