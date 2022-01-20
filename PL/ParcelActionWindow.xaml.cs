using BLAPI;
using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for ParcelActionWindow.xaml
    /// </summary>
    public partial class ParcelActionWindow : Window
    {
        public static Model Model { get; } = Model.Instance;
        private IBL bl;
        private ParcelToList p;
        public ParcelActionWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// parcel create window
        /// </summary>
        /// <param name="bl"></param>
        public ParcelActionWindow(IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            set_parcel_Weight.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));
            set_parcel_Priority.ItemsSource = Enum.GetValues(typeof(BO.Enums.Priorities));
            set_parcel_Priority.Visibility = Visibility.Visible;
            set_parcel_Weight.Visibility = Visibility.Visible;
            enter.Visibility = Visibility.Visible;
            show_parcel_name_drone.Visibility = Visibility.Collapsed;
            show_client.Visibility = Visibility.Collapsed;
        }
        Parcel parcel = new();
        /// <summary>
        /// parcel show window
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="p"></param>
        public ParcelActionWindow(IBL bl, ParcelToList p)
        {
            InitializeComponent();
            this.bl = bl;
            this.p = p;
            try
            {
                DataContext = p;
            }
            catch (BO.GetException g) { MessageBox.Show(g.Message); }
            set_parcel_Priority.Visibility = Visibility.Collapsed;
            set_parcel_Weight.Visibility = Visibility.Collapsed;
            show_parcel_Weight.Visibility = Visibility.Visible;
            show_Priority.Visibility = Visibility.Visible;
            parcel = bl.GetParcel(p.Id);
        }
        /// <summary>
        /// delete parcel window
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="p"></param>
        /// <param name="v"></param>
        public ParcelActionWindow(IBL bl, ParcelToList p, int v)
        {
            InitializeComponent();
            this.bl = bl;
            this.p = p;
            try
            {
                bl.DeleteParcel(p.Id);
            }
            catch (BO.DeleteException exc)
            { Model.Error(exc.Message); }

        }
        /// <summary>
        /// gert id parceel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int s;

            if (int.TryParse(this.show_parcel_id.Text, out s))
            {
                if (s > 0)
                {
                    parcel.Id = s;
                }
            }
            else
            {
                show_parcel_id.Text = "";
                MessageBox.Show("Please enter a positive number");
                show_parcel_id.Background = Brushes.Red;
            }
        }
        /// <summary>
        /// enter the parcel to the data 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enter_your_parcel(object sender, RoutedEventArgs e)
        {
            //first gets the values
            int s;
            int idNew = 0;
            if (int.TryParse(show_parcel_sender.Text, out s))
            {
                if (s > 0)
                {
                    idNew = s;

                }
                parcel.Sender = new CustomerInParcel();
                parcel.Sender.Id = idNew;
            }
            else
            {
                show_parcel_sender.Text = "";
                show_parcel_sender.Background = Brushes.Red;
            }
            if (int.TryParse(show_parcel_target.Text, out s))
            {
                if (s > 0)
                {
                    idNew = s;
                }
                parcel.Target = new CustomerInParcel();
                parcel.Target.Id = idNew;

            }
            else
            {
                show_parcel_target.Text = "";
                show_parcel_target.Background = Brushes.Red;
            }
            // 
            if (set_parcel_Priority.SelectedItem != null && set_parcel_Weight.SelectedItem != null)
            {
                parcel.Priority = (Enums.Priorities)set_parcel_Priority.SelectedItem;
                parcel.WeightCategories = (Enums.WeightCategories)set_parcel_Weight.SelectedItem;
            }



            if (int.TryParse(this.show_parcel_id.Text, out s))
            {
                if (s > 0)
                {
                    parcel.Id = s;
                }
            }
            else
            {
                show_parcel_id.Text = "";
                show_parcel_id.Background = Brushes.Red;
            }
            //if no all values are legal
            bool flag = false;
            if (set_parcel_Priority.SelectedItem == null)
            {
                Model.Error("please enter priority");
                flag = true;
            }
            if (set_parcel_Weight.SelectedItem == null)
            {
                Model.Error("please enter weight");
                flag = true;
            }
            if (show_parcel_id.Background == Brushes.Red)
            {

                Model.Error("please enter id");
                flag = true;
            }
            if (show_parcel_sender.Background == Brushes.Red)
            {
                Model.Error("please enter id of sender");
                flag = true;
            }
            if (show_parcel_target.Background == Brushes.Red)
            {
                Model.Error("please enter id of target");
                flag = true;
            }
            //if all values all legally set
            if (flag == false)
            {
                parcel.Created = DateTime.Now;
                parcel.DIP = new();
                parcel.DIP.Id = 0;
                try
                {
                    bl.AddParcel(parcel);
                    MessageBox.Show("Managed Add");
                    Close();
                    new ParcelListWindow(bl).Show();
                }
                catch (AddException exc)
                {
                    Model.Error(exc.Message);
                }
                catch (GetException exc)
                {
                    Model.Error(exc.Message);
                }
            }

        }
        /// <summary>
        /// show drone carrier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void show_drone_Click(object sender, RoutedEventArgs e)
        {
            if (parcel.Assignment <= DateTime.Now && parcel.Delivered == null)
            {
                new DroneAction(bl, bl.GetDroneOnParcel(parcel.Id).Id).Show();
            }
            else MessageBox.Show("not on drone");
        }
        /// <summary>
        /// show client owwner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void show_client_Click(object sender, RoutedEventArgs e)
        {
            new CustomerActionWindow(bl, parcel.Sender.Id, 1).Show();
        }

        /// <summary>
        /// close button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Closing_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// prevent regular closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Closing_Button.Visibility != Visibility.Hidden)
                e.Cancel = true;
        }
    }
}
