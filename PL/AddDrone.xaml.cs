
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
using System.Text.RegularExpressions;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class AddDrone : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        readonly IBL bl = BLFactory.GetBL();
        public static Model Model { get; } = Model.Instance;
        private Location loc = new();
        private PO.POAdapters poadapt = new PO.POAdapters();

        BO.Drone drone;
        public Drone Drone { get => drone; }
        public AddDrone(IBL bl)
        {
            drone = new();
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
            Auto.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// ctor show drone
        /// </summary>
        /// <param name="bl1">bl unit</param>
        /// <param name="i">drone id</param>
        public AddDrone(IBL bl1, int i)
        {
            drone = this.bl.GetDrone(i);
            InitializeComponent();
            locasa = drone.Location;
            Title = "ACTIONS  "+i;
            Choose_models.ItemsSource = Enum.GetValues(typeof(Enums.DroneNames));
            update_drone.Visibility = Visibility.Visible;
            simul.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// func to create a drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enter_your_drone(object sender, RoutedEventArgs e)
        {

            int i;
            Regex myReg = new Regex("[^0-9]+"); //gets regular expression that allows only digits
            if (!myReg.IsMatch(ChooseId.Text)) //checks taht key entered is regular expression

                drone.Id = Int32.Parse(ChooseId.Text);

            else
            {
                ChooseId.Text = "";
                Model.Error("Please enter a positive number");
                ChooseId.Background = Brushes.Red;
            }


            double s;
            if (myReg.IsMatch(ChooseLatitude.Text))
            {

                if (double.Parse(ChooseLatitude.Text) >= 0)
                {
                    loc.Latitude = double.Parse(ChooseLatitude.Text);
                }
            }
            if (ChooseLatitude.Text == "")
            {
                Model.Error("Please, number > 0");
                ChooseLatitude.Background = Brushes.Red;
            }
            s = 0;
        
                if (myReg.IsMatch(ChooseLongitude.Text))
                {
                if (double.Parse(ChooseLongitude.Text) >= 0)
                {
                    loc.Longitude = double.Parse(ChooseLongitude.Text);
                }
                }
            
            if (ChooseLongitude.Text == "")
            {
                Model.Error("Please, number > 0");
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
                    updateDroneView();
                    this.Close();
                }
                catch (BO.AddException ex)
                {
                    Model.Error(ex.Message);
                }

            }
        }

        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bl.GetDrone(Drone.Id), bl).Show();
        }

        private void Update_Drone_Click(object sender, RoutedEventArgs e)
        {
            if (Drone.Status is (BO.Enums.DroneStatuses)Enums.DroneStatuses.Vacant or (BO.Enums.DroneStatuses)Enums.DroneStatuses.Maintenance)
            {
                sendDrone.Visibility = Visibility.Visible;
            }
            updateDroneView();
        }


        private void SendTo_charge(object sender, RoutedEventArgs e)
        {

            TimeSpan time;
            if (Drone.Status == (Enums.DroneStatuses)Enums.DroneStatuses.Vacant)
            {
                bl.UpdateDroneSentToCharge(Drone.Id);

            }
            else if (Drone.Status == (Enums.DroneStatuses)Enums.DroneStatuses.Maintenance)
            {
                time = new TimeSpan(3, 0, 0);
                bl.UpdateReleaseDroneFromCharge(Drone.Id, time);
                timespan_get.Visibility = Visibility.Hidden;

            }

            updateDroneView();
        }
        /// <summary>
        /// set the next action to do with parcel depending on parcels case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeliveryChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (drone.Status == Enums.DroneStatuses.Vacant)//if no parcel on drone
                {
                    try
                    {
                        bl.UpdateAssignParcelToDrone(Drone.Id);
                    }
                    catch (GetException ex) { Model.Error(ex.Message); }

                }
                else if (drone.Status == Enums.DroneStatuses.InDelivery)//if parcel on drone
                {
                    ParcelToList parcel = bl.GetParcelToListonDrone(Drone.Id);

                    if (parcel.Id != 0)
                    {
                        Parcel p = bl.GetParcel(parcel.Id);
                        if (p.PickedUp == null && p.Assignment != null)
                        {
                            try
                            {
                                bl.UpdateDroneToPickUpAParcel(Drone.Id);
                            }
                            catch (GetException ex) { Model.Error(ex.Message); }

                        }
                        else if (p.Delivered == null)
                        {
                            bl.UpdateDeliverParcel(Drone.Id);
                        }

                    }
                }
                else//if drone on maintenance
                {
                    this.myEvent("In Maintenance");
                }
            }
            catch (AddException)
            {
                Model.Error("Missed Update");
            }
            this.myEvent("Managed Update");
            //  poDrone = poadapt.BODroneToPo(bl.GetDrone(poDrone.Id), poDrone);
            updateDroneView();
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
                Model.Error("your choice is impossible");
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
                new ParcelActionWindow(bl, bl.GetParcelToListonDrone(Drone.Id)).Show();
            }
            catch (BO.GetException ex)
            {
                Model.Error(ex.Message);
            }
        }

        private void Choose_models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bl.UpdateNameDrone(Drone.Id, (Enums.DroneNames)Choose_models.SelectedItem);
            this.drone = bl.GetDrone(drone.Id);
            MessageBox.Show("Managed Update");
            // poDrone = poadapt.BODroneToPo(bl.GetDrone(Drone.Id), poDrone);
            updateDroneView();
        }

        BackgroundWorker worker;
        private void updateDrone() => worker.ReportProgress(0);
        private bool checkStop() => worker.CancellationPending;


        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            worker?.CancelAsync();

            hide(false);

        }

        private void simul_Click(object sender, RoutedEventArgs e)
        {
            hide(true);
            worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            worker.DoWork += (sender, args) => bl.StartDroneSimulator(Drone.Id, updateDrone, checkStop);
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
                simul.Visibility = Visibility.Collapsed;
                PageStop.Visibility = Visibility.Collapsed;
            }
            else
            {
                update_drone.Visibility = Visibility.Visible;
                Manual.Visibility = Visibility.Collapsed;
                simul.Visibility = Visibility.Visible;
                PageStop.Visibility = Visibility.Visible;
            }

        }
        Location locasa = new();
        private void updateDroneView()
        {
            lock (bl)
            {
                drone = bl.GetDrone(Drone.Id);
                // updateFlags();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Drone"));

                }

                DroneToList droneForList = Model.Drones.FirstOrDefault(d => d.Id == Drone.Id);

                int index = Model.Drones.IndexOf(droneForList);
                if (index >= 0)
                {
                    Model.Drones.Remove(droneForList);
                    Model.Drones.Insert(index, bl.GetDroneList(Drone.Status).Where(d => d.Id == Drone.Id).FirstOrDefault());
                }
            }
        }

        private void BatteryProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}



