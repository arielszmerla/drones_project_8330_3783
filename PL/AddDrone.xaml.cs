﻿
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
using BO;
using BLAPI;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class AddDrone : Window
    {

        private Location loc = new();
        private IBL bl;
        private PO.POAdapters poadapt = new PO.POAdapters();

        public AddDrone(IBL bl)
        {

            InitializeComponent();
            Title = "ADD A DRONE";
            this.bl = bl;
            WeightCategSelector.ItemsSource = Enum.GetValues(typeof(Enums.WeightCategories));
            Choose_model.ItemsSource = Enum.GetValues(typeof(Enums.DroneNames));
            StatusSelectorToadd.ItemsSource = Enum.GetValues(typeof(Enums.DroneStatuses));
            add_drone_stack.Visibility = Visibility.Visible;
            add_drone_titles.Visibility = Visibility.Visible;
            enter.Visibility = Visibility.Visible;
            update_drone.Visibility = Visibility.Collapsed; Show_BaseStation_stack.Visibility = Visibility.Collapsed; show_Drone_titles.Visibility = Visibility.Collapsed;

        }
        BO.Drone drone = new();
        PO.Drone poDrone = new();
        public AddDrone(IBL bl1, Drone drone)
        {
            InitializeComponent();
         
            this.DataContext = poDrone;
            this.bl = bl1;
            Title = "ACTIONS";
            Choose_models.ItemsSource = Enum.GetValues(typeof(Enums.DroneNames));
           
            update_drone.Visibility = Visibility.Visible;
            poDrone = poadapt.BODroneToPo(drone, poDrone);
        }
 
        private void enter_your_drone(object sender, RoutedEventArgs e)
        {

            int i;

            if (int.TryParse(ChooseId.Text, out i))
            {
                if (i > 0)
                {

                    drone.Id = i;
                }
            }
            else
            {
                ChooseId.Text = "";
                MessageBox.Show("Please enter a positive number");
                ChooseId.Background = Brushes.Red;
            }


            double s;
            if (double.TryParse(ChooseLatitude.Text, out s))
            {

                if (i >= 0)
                {
                    loc.Latitude = i;
                }
            }
            if (ChooseLatitude.Text == "")
            {
                ;
                MessageBox.Show("Please, number > 0");
                ChooseLatitude.Background = Brushes.Red;
            }

            if (double.TryParse(ChooseLongitude.Text, out s))
            {
                if (s >= 0)
                {

                    loc.Longitude = s;
                }
            }
            if (ChooseLongitude.Text == "")
            {
                MessageBox.Show("Please, number > 0");
                ChooseLongitude.Background = Brushes.Red;

            }
            bool flag = true;
            if (WeightCategSelector.SelectedItem == null || Choose_model.SelectedItem == null || StatusSelectorToadd.SelectedItem == null ||
              ChooseLongitude.Text == "" || ChooseLatitude.Text == "" || Choose_model.SelectedItem == null || ChooseId.Text == "")
            {

                flag = false;
            }
            else
            {
                drone.MaxWeight = (BO.Enums.WeightCategories)WeightCategSelector.SelectedItem;
                drone.Model = (Enums.DroneNames)Choose_model.SelectedItem;
                drone.Id = i;
                drone.Status = (Enums.DroneStatuses)StatusSelectorToadd.SelectedItem;
                drone.Location = loc;


                Random rand = new Random();
                drone.Battery = rand.Next(99) + rand.NextDouble();

                drone.Location = loc;
                drone.PID = null;

                try
                {

                    bl.AddDrone(drone);
                    MessageBox.Show("Managed Add");
                    enter.Visibility = Visibility.Hidden;
                    this.Close();


                }
                catch (BO.AddException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bl.GetDrone(poDrone.Id), bl).Show();
        }

        private void Update_Drone_Click(object sender, RoutedEventArgs e)
        {
            if (poDrone.Status is (PO.Enums.DroneStatuses)Enums.DroneStatuses.Vacant or (PO.Enums.DroneStatuses)Enums.DroneStatuses.Maintenance)
            {
                sendDrone.Visibility = Visibility.Visible;
            }

        }


        private void SendTo_charge(object sender, RoutedEventArgs e)
        {

            TimeSpan time;
            if (poDrone.Status == (PO.Enums.DroneStatuses)Enums.DroneStatuses.Vacant)
            {
                bl.UpdateDroneSentToCharge(poDrone.Id);
                poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
            }
            else
            {

                MessageBox.Show("insert how many hours to charge");
                timespan_get.Visibility = Visibility.Visible;
                int i;

                if (int.TryParse(timespan_get.Text, out i))
                {
                    if (i > 0)
                    {
                        time = new TimeSpan(int.Parse(timespan_get.Text), 0, 0);
                        bl.UpdateReleaseDroneFromCharge(poDrone.Id, time);
                        timespan_get.Visibility = Visibility.Hidden;
                        poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
                    }

                }
                else
                {
                    timespan_get.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    timespan_get.Background = Brushes.Red;
                }


            }


        }

        private void DeliveryChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (drone.Status == Enums.DroneStatuses.Vacant)
                {
                    bl.UpdateAssignParcelToDrone(poDrone.Id);
                    drone.Status = Enums.DroneStatuses.InDelivery;
                }
                else if (drone.Status == Enums.DroneStatuses.InDelivery)
                {
                    ParcelToList parcel = bl.GetParcelToListonDrone(poDrone.Id);

                    if (parcel != null)
                    {
                        Parcel p = bl.GetParcel(parcel.Id);
                        if (p.PickedUp == null && p.Assignment!=null)
                        {
                            bl.UpdateDroneToPickUpAParcel(poDrone.Id);
                        }
                        else if (p.Delivered == null)
                        {
                            bl.UpdateDeliverParcel(poDrone.Id);
                        }

                    }
                }
                else
                {
                    this.myEvent("In Maintenance");
                }
            }
            catch (AddException)
            {
                this.myEvent("Missed Update");
            }
            this.myEvent("Managed Update");
            poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
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
                new DroneListWindow1(bl).Show();
        }


        private void myEvent(string s)
        {
            MessageBox.Show(s);
        }
        private void Form2_FormClosing(object sender, CancelEventArgs e)
        {

            this.Show();

        }


        private void statust_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stats.Background = Brushes.Transparent;
            Enums.DroneStatuses stat = (Enums.DroneStatuses)((ComboBox)sender).SelectedItem;

            if (stat == Enums.DroneStatuses.InDelivery)
            {
                MessageBox.Show(" your choice is impossible");
                stats.Background = Brushes.Red;
            }
            else
                drone.Status = (Enums.DroneStatuses)StatusSelectorToadd.SelectedItem;

        }



        private void Choose_model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Choose_model.Background = Brushes.Transparent;
            drone.Model = (Enums.DroneNames)Choose_model.SelectedItem;
        }

        private void show_parcel_inDrone(object sender, RoutedEventArgs e)
        {
            try
            {
                new ParcelActionWindow(bl, bl.GetParcelToListonDrone(poDrone.Id)).Show();
            }
            catch (BO.GetException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Choose_models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bl.UpdateNameDrone(poDrone.Id, (Enums.DroneNames)Choose_models.SelectedItem);
            this.drone = bl.GetDrone(drone.Id);
            MessageBox.Show("Managed Update");
            poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
        }

        BackgroundWorker worker;
        private void updateDrone() => worker.ReportProgress(0);
        private bool checkStop() => worker.CancellationPending;


        private void Manual_Click(object sender, RoutedEventArgs e) => worker?.CancelAsync();

        private void simul_Click(object sender, RoutedEventArgs e)
        {
            hide(true);
            worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            worker.DoWork += (sender, args) => bl.StartDroneSimulator(poDrone.Id, updateDrone, checkStop);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                Auto.IsChecked = false;
                worker = null;
                if (enter.Visibility == Visibility.Hidden) Close();
            };
             worker.ProgressChanged += (sender, args) => updateDroneView();
            worker.RunWorkerAsync(drone.Id);
        }

        private void hide(bool flag)
        {
            if (flag)
            {
                update_drone.Visibility = Visibility.Collapsed;
                Manual.Visibility = Visibility.Visible;
            }
            else
                update_drone.Visibility = Visibility.Visible;
            Manual.Visibility = Visibility.Collapsed;

        }
        private void updateDroneView() 
        {
           
            poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
        }



    }
}



