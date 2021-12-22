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
            int s;

            if (int.TryParse(ChooseId.Text, out s))
            {
                if (s > 0)
                {
                    ID.Background = Brushes.Transparent;
                    bs.Id = s;
                }
                else
                {
                    ChooseId.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    ID.Background = Brushes.Red;
                }
            }

        }

        private void Name_input(object sender, TextChangedEventArgs e)
        {

            if (ChooseName.Text == "")
            {
                ChooseName.Text = "";
                MessageBox.Show("Please enter a name");
                Name.Background = Brushes.Red;
            }
            else
            {
                Name.Background = Brushes.Transparent;
                bs.Name = ChooseName.Text;
            }

        }

        private void NOFS_Input(object sender, TextChangedEventArgs e)
        {
            int s;
            int.TryParse(ChooseNumOfFreeSlots.Text, out s);
            if (s >= 1)
            {
                Free_Slots.Background = Brushes.Transparent;
                bs.NumOfFreeSlots = s;
            }
            else
            {
                ChooseNumOfFreeSlots.Text = "";
                Free_Slots.Background = Brushes.Red;
                MessageBox.Show("Please enter a number between 2 to 8");
            }
        }

        private void Latitude_input(object sender, TextChangedEventArgs e)
        {
            double s;
            double.TryParse(ChooseLatitude.Text, out s);
            if (s >= 0)
            {
                loc.Latitude = s;
            }
            else
            {
                ChooseLatitude.Text = "";
                MessageBox.Show("Please insert a number bigger then 0");
                ChooseLatitude.Background = Brushes.Red;
            }

        }

        private void Longitude_input(object sender, TextChangedEventArgs e)
        {
            double s;
            double.TryParse(ChooseLongitude.Text, out s);
            if (s >= 0)
            {
                loc.Longitude = s;
            }
            else
            {
                ChooseLongitude.Text = "";
                MessageBox.Show("Please insert a number bigger then 0");
                ChooseLongitude.Background = Brushes.Red;
            }

        }

        private void enter_your_baseStation(object sender, RoutedEventArgs e)
        {
            bool flag = true;

            if (ChooseId.Text == "")
            {
                ID.Background = Brushes.Red;
                flag = false;
            }
            if (ChooseName.Text == "")
            {
                Name.Background = Brushes.Red;
                flag = false;
            }

            if (flag)
            {
                if (loc.Latitude < 31.740967 || loc.Latitude > 31.815177)
                {
                    ChooseLatitude.Text = "";
                    ChooseLatitude.Background = Brushes.Red;
                    MessageBox.Show("enter a latitude between 29.000000 and 34.000000");
                }
                if (loc.Longitude < 35.171323 || loc.Longitude > 35.202050)
                {

                    ChooseLongitude.Text = "";
                    ChooseLatitude.Background = Brushes.Red;
                    MessageBox.Show("enter a latitude between 34.000000 and 35.000000");
                }
                if (ChooseLongitude.Text == "" || ChooseLatitude.Text == "" || ChooseNumOfFreeSlots.Text == "")
                    return;
                int s;
                int.TryParse(ChooseNumOfFreeSlots.Text, out s);
                if (s < 2 || s > 8)
                {
                    ChooseNumOfFreeSlots.Text = "";
                    ChooseNumOfFreeSlots.Background = Brushes.Red;
                    MessageBox.Show("enter number between 2 to 8");
                }
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

        private void Name_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Name_Update.Text != "")
                bs.Name = Name_Update.Text;

        }

        private void SLOTS_UPDATE_TextChanged(object sender, TextChangedEventArgs e)
        {
            SLOTS_UPDATE.Background = Brushes.Bisque;
            int s;
            int.TryParse(SLOTS_UPDATE.Text, out s);
            if (s >= 1)
                bs.NumOfFreeSlots = s;
            else
            {
                MessageBox.Show("invalid number of slots");
                SLOTS_UPDATE.Background = Brushes.Red;
            }

        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (bs.NumOfFreeSlots < 0)
            {
                MessageBox.Show("Invalid number, please enter a number between 3 to 8");
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