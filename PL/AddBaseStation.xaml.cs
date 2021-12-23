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

namespace PL
{
    /// <summary>
    /// Interaction logic for AddBaseStation.xaml
    /// </summary>
    public partial class AddBaseStation : Window
    {
        private Location loc = new();
        private IBL bl;
        public AddBaseStation(IBL bl)
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
        public AddBaseStation(IBL bl, BaseStation baseStation)
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

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            PageStop.Visibility = Visibility.Hidden;
            new BaseStationViewWindow(bl).Show();
            this.Close();
        }

        private void AddBaseStationClosing(object sender, CancelEventArgs e)
        {
            if (PageStop.Visibility != Visibility.Hidden)
                e.Cancel = true;

        }


        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            ChooseId.Background = Brushes.Bisque;

        }

        private void Name_input(object sender, TextChangedEventArgs e)
        {
            ChooseName.Background = Brushes.Bisque;
        }

        private void NOFS_Input(object sender, TextChangedEventArgs e)
        {
            ChooseNumOfFreeSlots.Background = Brushes.Bisque;
        }

        private void Latitude_input(object sender, TextChangedEventArgs e)
        {
            ChooseLatitude.Background = Brushes.Bisque;
        }

        private void Longitude_input(object sender, TextChangedEventArgs e)
        {
            ChooseLongitude.Background = Brushes.Bisque;
        }

        private void enter_your_baseStation(object sender, RoutedEventArgs e)
        {
            int s;
            // first we insert id into the base station
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
                    MessageBox.Show("Please enter a positive number");
                    ChooseId.Background = Brushes.Red;
                }
            }

            // entering name into base station

            if (ChooseName.Text == "")
            {
                MessageBox.Show("Please enter a name");
                ChooseName.Background = Brushes.Red;
            }
            else
            {
                ChooseName.Background = Brushes.Transparent;
                bs.Name = ChooseName.Text;
            }

            // number of free slots

           
            int.TryParse(ChooseNumOfFreeSlots.Text, out s);
            if (s > 0)
            {
                ChooseNumOfFreeSlots.Background = Brushes.Transparent;
                bs.NumOfFreeSlots = s;
            }
            else
            {
                ChooseNumOfFreeSlots.Text = "";
                ChooseNumOfFreeSlots.Background = Brushes.Red;
                MessageBox.Show("Please enter a number between 2 to 8");
            }

            //latitude

            double y;
            double.TryParse(ChooseLatitude.Text, out y);
            if (y >= 0)
            {
                ChooseLatitude.Background = Brushes.Transparent;
                loc.Latitude = y;
            }
            else
            {
                ChooseLatitude.Text = "";
                MessageBox.Show("Please enter a latitude between 31.740967 and 31.815177");
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
                MessageBox.Show("Please enter a longitude between 35.171323 and 35.202050");
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
                
                bs.BaseStationLocation = loc;

                try
                {
                    bl.AddBaseStation(bs);
                    MessageBox.Show("Managed Add");
                    enter.Visibility = Visibility.Hidden;
                    this.Close();


                }
                catch (BO.AddException)
                {
                    MessageBox.Show("Missed Add");
                    PageStop.Visibility = Visibility.Hidden;
                }
            }
            else
                MessageBox.Show("you have to fill the red places");
        }

        /// <summary>
        /// name update text, nothing has to be written down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Name_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SLOTS_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {
            SLOTS_UPDATE.Background = Brushes.Bisque;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            int s;
            int.TryParse(SLOTS_UPDATE.Text, out s);
            bs.NumOfFreeSlots = s;
            if (Name_Update.Text != "")
                bs.Name = Name_Update.Text;
            if (bs.NumOfFreeSlots < 0)
            {
                MessageBox.Show("Invalid number, please enter a number between 3 to 8");
                SLOTS_UPDATE.Text = "";
                SLOTS_UPDATE.Background = Brushes.Red;
            }
            try
            {
                bl.UpdateBaseStation(bs.Id, bs.NumOfFreeSlots, bs.Name);
            }
            catch (BO.GetException)
            {
                MessageBox.Show("Update failed");
            }
            PageStop.Visibility = Visibility.Hidden;
            new BaseStationViewWindow(bl).Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeleteBasestation(bs.Id);
            }
            catch (DeleteException)
            {
                MessageBox.Show("Delete failed");
            }
            PageStop.Visibility = Visibility.Hidden;
            new BaseStationViewWindow(bl).Show();
            this.Close();
        }

      
    }
}