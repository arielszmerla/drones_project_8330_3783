
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
    public partial class DroneAction : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        readonly IBL bl = BLFactory.GetBL();
        public static Model Model { get; } = Model.Instance;
        private Location loc = new();
        private PO.POAdapters poadapt = new PO.POAdapters();

        BO.Drone drone;
        public Drone Drone { get => drone; }
        /// <summary>
        /// cunstrocter incase adding a drone
        /// </summary>
        /// <param name="bl"></param>
        public DroneAction(IBL bl)
        {
            drone = new();
            InitializeComponent();
            Title = "ADD A DRONE";
            this.bl = bl;
            /// combo box options
            WeightCategSelector.ItemsSource = Enum.GetValues(typeof(Enums.WeightCategories));
            Choose_model.ItemsSource = Enum.GetValues(typeof(Enums.DroneNames));
            StatusSelectorToadd.ItemsSource = Enum.GetValues(typeof(Enums.DroneStatuses));
            #region visibilaties
            simul.Visibility = Visibility.Collapsed;
            add_drone_stack.Visibility = Visibility.Visible;
            add_drone_titles.Visibility = Visibility.Visible;
            enter.Visibility = Visibility.Visible;
            update_drone.Visibility = Visibility.Collapsed;
            Show_BaseStation_stack.Visibility = Visibility.Collapsed;
            show_Drone_titles.Visibility = Visibility.Collapsed;
            Auto.Visibility = Visibility.Collapsed;
            #endregion

        }

        /// <summary>
        /// ctor show drone
        /// </summary>
        /// <param name="bl1">bl unit</param>
        /// <param name="i">drone id</param>
        public DroneAction(IBL bl1, int i)
        {
            drone = this.bl.GetDrone(i);
            InitializeComponent();
            locasa = drone.Location;
            Title = "ACTIONS  " + i;
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
            Regex myReg = new Regex("[^0-9]+"); //gets regular expression that allows only digits
            if (!myReg.IsMatch(ChooseId.Text)) //checks taht key entered is regular expression
                drone.Id = Int32.Parse(ChooseId.Text);
            else
            {
                ChooseId.Text = "";
                Model.Error("Please enter a positive number");
                ChooseId.Background = Brushes.Red;
            }

            //checks if latitde is only numbers
            if (myReg.IsMatch(ChooseLatitude.Text))
            {

                if (double.Parse(ChooseLatitude.Text) >= 0)
                {
                    loc.Latitude = double.Parse(ChooseLatitude.Text);
                }
            }
            //if no latitude was chosen
            if (ChooseLatitude.Text == "")
            {
                Model.Error("Please, enter a positive number");
                ChooseLatitude.Background = Brushes.Red;
            }


            if (myReg.IsMatch(ChooseLongitude.Text))
            {
                if (double.Parse(ChooseLongitude.Text) >= 0)
                {
                    loc.Longitude = double.Parse(ChooseLongitude.Text);
                }
            }
            // basic check for longitude
            if (ChooseLongitude.Text == "")
            {
                Model.Error("Please, number > 0");
                ChooseLongitude.Background = Brushes.Red;

            }
           // insert all info into drone and send update to bl to try and update the drone
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

       /// <summary>
       /// function to view drone in map
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bl.GetDrone(Drone.Id), bl).Show();
        }


        /// <summary>
        /// function to send drone to charge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendTo_charge(object sender, RoutedEventArgs e)
        {

            TimeSpan time;
            // if the drone is vacant we ssend him to charge
            if (Drone.Status == (Enums.DroneStatuses)Enums.DroneStatuses.Vacant)
            {
                bl.UpdateDroneSentToCharge(Drone.Id);

            }
            // if the drone is already in maintanance we release the drone from charging
            else if (Drone.Status == (Enums.DroneStatuses)Enums.DroneStatuses.Maintenance)
            {
                time = new TimeSpan(3, 0, 0);
                bl.UpdateReleaseDroneFromCharge(Drone.Id, time);

            }
            // after updating we update the view of the drone
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
            updateDroneView();
        }

        /// <summary>
        /// close page function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void End_the_page(object sender, RoutedEventArgs e)
        {
            enter.Visibility = Visibility.Hidden;

            this.Close();
        }

        /// <summary>
        /// function to prevent closing from regular button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneAction_Closing(object sender, CancelEventArgs e)
        {
            if (this.enter.Visibility != Visibility.Hidden)
                e.Cancel = true;
        }

        /// <summary>
        /// helper function to save code writing for message box
        /// </summary>
        /// <param name="s"></param>
        private void myEvent(string s)
        {
            MessageBox.Show(s);
        }


  


      
     /*  private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }*/

        /// <summary>
        /// button to show parcel in drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void show_parcel_inDrone(object sender, RoutedEventArgs e)
        {
            try
            {
                new ParcelActionWindow(bl, bl.GetParcelToListonDrone(Drone.Id)).Show();
            }
            catch (GetException ex)
            {
                Model.Error(ex.Message);
            }
        }

        /// <summary>
        /// combo box to change model of drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Choose_models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bl.UpdateNameDrone(Drone.Id, (Enums.DroneNames)Choose_models.SelectedItem);
            this.drone = bl.GetDrone(drone.Id);
            MessageBox.Show("Managed Update");
            updateDroneView();
        }

        BackgroundWorker worker;
        private void updateDrone() => worker.ReportProgress(0);
        private bool checkStop() => worker.CancellationPending;

        /// <summary>
        /// button to stop automatic simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            worker?.CancelAsync();

            hide(false);

        }

        /// <summary>
        /// button to turn on simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// helper functoin for what to be viewed with simulator
        /// </summary>
        /// <param name="flag"></param>
        private void hide(bool flag)
        {
            //if the simulator is on we send true, therfore we can't see the update drone option, the simul button, and we can't exit
            // the page
            if (flag)
            {
                update_drone.Visibility = Visibility.Collapsed;
                Manual.Visibility = Visibility.Visible;
                simul.Visibility = Visibility.Collapsed;
                PageStop.Visibility = Visibility.Collapsed;
            }
            else
            // if the simulator is off we can't see the manual button but we can see the other ones
            {
                update_drone.Visibility = Visibility.Visible;
                Manual.Visibility = Visibility.Collapsed;
                simul.Visibility = Visibility.Visible;
                PageStop.Visibility = Visibility.Visible;
            }

        }
        Location locasa = new();

        /// <summary>
        /// hlper function to update the view we currently see of the drone
        /// </summary>
        private void updateDroneView()
        {
            lock (bl)
            {
                drone = bl.GetDrone(Drone.Id);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Drone"));

                DroneToList droneForList = Model.Drones.FirstOrDefault(d => d.Id == Drone.Id);
                // we search for the drone with same id, update him and replace with old drone (erase old drone)
                int index = Model.Drones.IndexOf(droneForList);
                if (index >= 0)
                {
                    Model.Drones.Remove(droneForList);
                    Model.Drones.Insert(index, bl.GetDroneList(Drone.Status).Where(d => d.Id == Drone.Id).FirstOrDefault());
                }
            }
        }

       
    }
}



