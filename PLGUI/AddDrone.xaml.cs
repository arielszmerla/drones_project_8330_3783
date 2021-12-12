using IBL.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class AddDrone : Window
    {

        private Location loc = new();
        private IBL.IBL bl1;

  
        public AddDrone(IBL.IBL bl1)
        {

            InitializeComponent();
            Title = "ADD A DRONE";
            this.bl1 = bl1;
            WeightCategSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.Enums.WeightCategories));
        
            StatusSelectorToadd.ItemsSource= Enum.GetValues(typeof(IBL.BO.Enums.DroneStatuses));
            add_drone_stack.Visibility = Visibility.Visible;
            add_drone_titles.Visibility = Visibility.Visible;
            enter.Visibility = Visibility.Visible;

        }
        IBL.BO.Drone dr = new();
        public AddDrone(IBL.IBL bl1, Drone drone)
        {
            this.bl1 = bl1;
            dr = drone;
            InitializeComponent();
            Title = "ACTIONS";
            update_drone.Visibility = Visibility.Visible;
        }

        IBL.BO.Drone drone = new();



        private void cmbWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IBL.BO.Enums.WeightCategories weightCategories = (IBL.BO.Enums.WeightCategories)WeightCategSelector.SelectedItem;
            drone.MaxWeight = weightCategories;
            Categorie_weight.Background= Brushes.Transparent; 

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            int s;

            if (int.TryParse(ChooseId.Text, out s))
            {
                if (s > 0)
                {
                    ID.Background = Brushes.Transparent;
                    dr.Id = s;
                }
            }
            else
            {
                ChooseId.Text = "";
                MessageBox.Show("Please enter a positive number");
                ChooseId.Background = Brushes.Red;
            }
        }

        private void enter_your_drone(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (WeightCategSelector.SelectedItem == null)
            {
                Categorie_weight.Background = Brushes.Red;
                flag = false;
            }

             if (this.Choose_model.Background == Brushes.Red)
            {
                return;
            }
            if (ChooseId.Text == "")
            {
                ID.Background = Brushes.Red;
                flag = false;
            }
       
            if (flag ==true)
            {
                Random rand = new Random();
                dr.BatteryStatus = rand.Next(99) + rand.NextDouble();

                if (Choose_model.Text=="")
                {
                    
                    Choose_model.Background = Brushes.Red;
                    MessageBox.Show("enter a name please");
                    return;
                }
                if (loc.latitude< 29.000000 || loc.latitude>34.000000)
                {
                    ChooseLatitude.Text = "";
                    ChooseLatitude.Background = Brushes.Red;
                    MessageBox.Show("enter a latitude between 29.000000 and 34.000000");
                } 
                if(loc.longitude < 34.000000 || loc.longitude > 35.000000)
                {
                    
                    ChooseLongitude.Text = "";
                    ChooseLatitude.Background = Brushes.Red;
                    MessageBox.Show("enter a latitude between 34.000000 and 35.000000");
                }
                if (ChooseLongitude.Text == "" ||
                     ChooseLatitude.Text == "")
                    return;
                if (stats.Background == Brushes.Red)
                    return;
                dr.DronePlace = loc;
                dr.PID = null;

                try
                {

                    bl1.AddDrone(dr);
                    MessageBox.Show("Managed Add");
                    enter.Visibility = Visibility.Hidden;
                    this.Close();
          

                }
                catch (IBL.BO.AddException)
                {
                    MessageBox.Show("Missed Add");
                }
            }
            else
             MessageBox.Show("you have to fill the red places");
        }

        private void ShowDroneAdded_TextChanged(object sender, TextChangedEventArgs e)
        {

            ShowDroneAdded.Text = dr.ToString();


        }

        private void Update_Drone_Click(object sender, RoutedEventArgs e)
        {
            if (dr.Status == Enums.DroneStatuses.Vacant || dr.Status == Enums.DroneStatuses.Maintenance)
            {
                sendDrone.Visibility = Visibility.Visible;
            }
            Get_model.Visibility = Visibility.Visible;
        }

        private void Get_model_TextChanged(object sender, TextChangedEventArgs e)
        {
            bl1.UpdateNameDrone(dr.Id, Get_model.Text);
            Get_model.Visibility = Visibility.Hidden;
            this.dr = bl1.GetDrone(dr.Id);
            MessageBox.Show("Managed Update");
            this.ShowDroneAdded.Text = dr.ToString();
            this.ShowDroneAdded.Visibility = Visibility.Visible;
        }



        private void SendTo_charge(object sender, RoutedEventArgs e)
        {
            if (dr.Status == Enums.DroneStatuses.Vacant)
                bl1.UpdateDroneSentToCharge(dr.Id);
            else
            {

                MessageBox.Show("insert how many hours to charge");
                timespan_get.Visibility = Visibility.Visible;

            }

        }
        private void timespan_get_TextChanged(object sender, TextChangedEventArgs e)
        {
            TimeSpan time = new TimeSpan(int.Parse(timespan_get.Text), 0, 0);
            bl1.UpdateReleaseDroneFromCharge(dr.Id, time);
            timespan_get.Visibility = Visibility.Hidden;
            this.ShowDroneAdded.Text = dr.ToString();
            this.ShowDroneAdded.Visibility = Visibility.Visible;
        }
        private void DeliveryChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dr.Status == Enums.DroneStatuses.Vacant)
                {
                    bl1.UpdateAssignParcelToDrone(dr.Id);
                }
                else if (dr.Status == Enums.DroneStatuses.InDelivery)
                {
                    IBL.BO.Drone drone = bl1.GetDrone(dr.Id);

                    if (drone.PID != null)
                    {
                        IBL.BO.Parcel p = bl1.GetParcel(drone.PID.Id);
                        if (p.PickedUp < DateTime.Now && p.Delivered > DateTime.Now)
                        {
                            bl1.UpdateDeliverParcel(dr.Id);
                        }
                        if (p.PickedUp > DateTime.Now)
                        {
                            bl1.UpdateDroneToPickUpAParcel(dr.Id);
                        }
                    }
                }
            }
            catch (IBL.BO.AddException)
            {
                this.myEvent("Missed Update");
            }
            this.myEvent("Managed Update");
            this.ShowDroneAdded.Text = dr.ToString();
            this.ShowDroneAdded.Visibility = Visibility.Visible;
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            enter.Visibility = Visibility.Hidden;
           
            this.Close();
        }
        private void AddDrone_Closing(object sender, CancelEventArgs e)
        {
            if (this.enter.Visibility != Visibility.Hidden)
                e.Cancel = true;
            else
                new DroneListWindow1(bl1).Show();
        }


        private void myEvent(string s)
        {
            MessageBox.Show(s);
        }
        private void Form2_FormClosing(object sender, CancelEventArgs e)
        {
            this.ShowDroneAdded.Text = dr.ToString();
            this.Show();

        }

        private void Latitude_input(object sender, TextChangedEventArgs e)
        {

            TextBox t = (TextBox)sender;
            double s;
            if (double.TryParse(t.Text, out s))
            {

                if (s >= 0)
                {
                    loc.latitude = s;
                }
            }
            if (loc.latitude == 0)
            {
                ChooseLatitude.Text = "";
                MessageBox.Show("Please, number > 0");
                ChooseLatitude.Background = Brushes.Red;
            }

        }

        private void Longitude_input(object sender, TextChangedEventArgs e)
        {
           

            TextBox t = (TextBox)sender;
            double s;
            if (double.TryParse(t.Text, out s))
            {
                if (s >= 0)
                {

                    loc.longitude = s;
                }
            }
            if (loc.longitude ==0)
          
            {
                ChooseLongitude.Text = "";
                MessageBox.Show("Please, number between 34.000000 and 35.000000");
                ChooseLongitude.Background = Brushes.Red;

            }

        }

        private void Name_input(object sender, TextChangedEventArgs e)
        {
            dr.Model =( (TextBox)sender).Text;
            
        }

        private void statust_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stats.Background = Brushes.Transparent;
            Enums.DroneStatuses stat= (Enums.DroneStatuses)((ComboBox)sender).SelectedItem;

            if (stat == Enums.DroneStatuses.InDelivery)
            {
                MessageBox.Show(" your choice is impossible");
                stats.Background = Brushes.Red;
            }
            else
                dr.Status = (Enums.DroneStatuses)StatusSelectorToadd.SelectedItem;
        }

        private void Model_input(object sender, TextChangedEventArgs e)
        {
            Choose_model.Background = Brushes.Transparent;
            string s = ((TextBox)sender).Text;
            if (s == "")
            {
                MessageBox.Show("Please enter a name");
                Choose_model.Background = Brushes.Red;
            }
            else dr.Model = s;
        }
    }
}



