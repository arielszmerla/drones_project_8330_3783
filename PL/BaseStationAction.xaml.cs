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
using BO;
using BLAPI;
using System.ComponentModel;
using System.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for AddBaseStation.xaml
    /// </summary>
    public partial class BaseStationAction : Window
    {
        public static Model Model { get; } = Model.Instance;
        private Location loc = new();
        private IBL bl;
        /// <summary>
        /// constructor for adding a base station option
        /// </summary>
        /// <param name="bl"></param>
        public BaseStationAction(IBL bl)
        {
     
            Title = "ADD A BASE STATION";
            InitializeComponent();
            this.bl = bl;
            add_BaseStation_titles.Visibility = Visibility.Visible;
            add_BaseStation_stack.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// constructor for double click on updating base station
        /// </summary>
        BO.BaseStation bs = new();
        public BaseStationAction(IBL bl, BaseStation baseStation)
        {
            Title = "UPDATE";
            InitializeComponent();
            this.bl = bl;
            bs = baseStation;
            Update_BaseStation.Visibility = Visibility.Visible;
            show_BaseStation_titles.Visibility = Visibility.Visible;
            Show_BaseStation_stack.Visibility = Visibility.Visible;
            DataContext = baseStation;
        }

        /// <summary>
        /// function for closing the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void End_the_page(object sender, RoutedEventArgs e)
        {
            PageStop.Visibility = Visibility.Hidden;
            this.Close();
        }

        /// <summary>
        /// function to provide the user from clicking the exit button 
        /// on the corner of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBaseStationClosing(object sender, CancelEventArgs e)
        {
            if (PageStop.Visibility != Visibility.Hidden)
                e.Cancel = true;

        }

        /// <summary>
        /// button for adding a base station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enter_your_baseStation(object sender, RoutedEventArgs e)
        {
            // we check very basic logic of each content before sending for update
            int s;
          
            if (int.TryParse(ChooseId.Text, out s))
            {
                if (s > 0)
                {
                    ChooseId.Background = Brushes.Bisque;
                    bs.Id = s;
                }
                else
                {
                    ChooseId.Text = "";
                    Model.Error("Please enter a positive number");
                    ChooseId.Background = Brushes.Red;
                }
            }

            // entering name into base station

            if (ChooseName.Text == "")
            {
                Model.Error("Please enter a name");
                ChooseName.Background = Brushes.Red;
            }
            else
            {
                ChooseName.Background = Brushes.Bisque;
                bs.Name = ChooseName.Text;
            }

            // number of free slots

           
            int.TryParse(ChooseNumOfFreeSlots.Text, out s);
            if (s > 0)
            {
                ChooseNumOfFreeSlots.Background = Brushes.Bisque;
                bs.NumOfFreeSlots = s;
            }
            else
            {
                ChooseNumOfFreeSlots.Text = "";
                ChooseNumOfFreeSlots.Background = Brushes.Red;
                Model.Error("Please enter a number between 2 to 8");
            }

            //latitude

            double y;
            double.TryParse(ChooseLatitude.Text, out y);
            if (y >= 0)
            {
                ChooseLatitude.Background = Brushes.Bisque;
                loc.Latitude = y;
            }
            else
            {
                ChooseLatitude.Text = "";
                Model.Error("Please enter a latitude between 31.740967 and 31.815177");
                ChooseLatitude.Background = Brushes.Red;
            }

            //longititude

            
            double.TryParse(ChooseLongitude.Text, out y); 
            if (y >= 0)
            {
                loc.Longitude = y;
            }
            else
            {
                ChooseLongitude.Text = "";
                Model.Error("Please enter a longitude between 35.171323 and 35.202050");
                ChooseLongitude.Background = Brushes.Red;
            }

            bool flag = true;

            if (ChooseId.Text == "")
            {
                flag = false;
            }
            if (ChooseName.Text == "")
            {
                flag = false;
            }

            if (flag)
            {
                
                if (ChooseLongitude.Text == "" || ChooseLatitude.Text == "" || ChooseNumOfFreeSlots.Text == "")
                    return;
                
                int.TryParse(ChooseNumOfFreeSlots.Text, out s);
                
                bs.Location = loc;

                try
                {
                    bl.AddBaseStation(bs);
                    MessageBox.Show("Managed Add");
                    PageStop.Visibility = Visibility.Hidden;
                    new BaseStationViewWindow(bl).Show();
                    this.Close();


                }
                catch (BO.AddException x)
                {
                    Model.Error(x.Message);
                }
            }
            else
                MessageBox.Show("Fill out missing data");
        }

        /// <summary>
        /// name update text, nothing has to be written down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Name_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        /// <summary>
        /// update number of slots 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SLOTS_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {
            //when the user writes anything background return to bisque
            SLOTS_UPDATE.Background = Brushes.Bisque;
        }

        /// <summary>
        /// update func
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            int s;
            int.TryParse(SLOTS_UPDATE.Text, out s);
            bs.NumOfFreeSlots = s;
            if (Name_Update.Text != "")
                bs.Name = Name_Update.Text;
            if (bs.NumOfFreeSlots < 0)
            {
                Model.Error("Please enter a positive number");
                SLOTS_UPDATE.Text = "";
                SLOTS_UPDATE.Background = Brushes.Red;
            }
            else
            {
                try
                {
                    bl.UpdateBaseStation(bs.Id, bs.NumOfFreeSlots, bs.Name);
                }
                catch (GetException x)
                {
                    Model.Error(x.Message);
                }
                PageStop.Visibility = Visibility.Hidden;
                MessageBox.Show("Managed update!");
                this.Close();
            }
           
        }

        /// <summary>
        /// delete function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeleteBasestation(bs.Id);
            }
            catch (DeleteException x)
            {
                Model.Error(x.Message);
            }
            PageStop.Visibility = Visibility.Hidden;
            new BaseStationViewWindow(bl).Show();
            this.Close();
        }

        /// <summary>
        /// function to show drones in a certain base station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewDrones_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow1(bs.Id,bl).Show();
        }

       

    }
}