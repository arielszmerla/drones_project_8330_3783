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
using System.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for BaseStationViewWindow.xaml
    /// </summary>
    public partial class BaseStationViewWindow : Window
    {
        private BLAPI.IBL bl;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="bl"></param>
        public BaseStationViewWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            BaseStationView.ItemsSource = bl.GetBaseStationList();
            DataContext = BaseStationView.ItemsSource;
        }

        /// <summary>
        /// end page button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void End_the_page(object sender, RoutedEventArgs e)
        {
            PageStop.Visibility = Visibility.Hidden;
            Close();

        }
        /// <summary>
        /// helper function so the regular close button wont work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseSViewClosing(object sender, CancelEventArgs e)
        {
            if (PageStop.Visibility != Visibility.Hidden)
                e.Cancel = true;

        }

      

        /// <summary>
        /// reset button to reset the base station viewed list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetList_Click(object sender, RoutedEventArgs e)
        {
            BaseStationView.ItemsSource = bl.GetBaseStationList();
            ResetList.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// function for double click on a certin base station in order to view or update
        /// its info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseStationView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.BaseStationToList bs = (BO.BaseStationToList)BaseStationView.SelectedItem;
            if (bs == null)
            {
                MessageBox.Show("Please click on a base station");
            }
            else
            {
               //we update the info of base station and send it to baseation actions
                BO.BaseStation baseStation = new BO.BaseStation
                {
                    Id = bs.Id,
                    Name = bs.Name,
                    NumOfFreeSlots = bs.NumOfFreeSlots,
                    Location = bs.Location,
                    ChargingDrones = bs.ChargingDrones
                };
                new BaseStationAction(bl, baseStation).Show();
            }
        }

    



    /// <summary>
    /// function to exit page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            new BaseStationAction(bl).Show();
        }

        /// <summary>
        /// if we want to devide the base stations viewed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseOptions_Click(object sender, RoutedEventArgs e)
        {
            BaseStationView.ItemsSource = bl.GetBaseStationListGroup();
            BaseStationView.Items.Refresh();
            ResetList.Visibility = Visibility.Visible;
        }
    }
}

