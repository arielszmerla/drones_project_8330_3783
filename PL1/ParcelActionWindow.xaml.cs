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
        private IBL bl;
        private ParcelToList p;
        public ParcelActionWindow()
        {
            InitializeComponent();
        }

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
        }
        Parcel parcel = new();
        public ParcelActionWindow(IBL bl, ParcelToList p)
        {
            InitializeComponent();
            this.bl = bl;
            this.p = p;
            try
            {
                DataContext = bl.GetParcel(p.Id);
            }
            catch (BO.GetException g) { MessageBox.Show(g.ToString()); }
            set_parcel_Priority.Visibility = Visibility.Collapsed;
            set_parcel_Weight.Visibility = Visibility.Collapsed;
            show_parcel_Weight.Visibility = Visibility.Visible;
            show_Priority.Visibility = Visibility.Visible;
        }

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
            { MessageBox.Show(exc.ToString()); }

        }
        /*
         
         public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public  BO.Enums.WeightCategories WeightCategories { get; set; }
        public BO.Enums.Priorities Priority { get; set; }
        public DroneInParcel DIP { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Assignment { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }*/
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

        private void set_parcel_Weight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parcel.WeightCategories = (BO.Enums.WeightCategories)set_parcel_Weight.SelectedItem;
        }

        private void set_parcel_Priority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parcel.Priority = (BO.Enums.Priorities)set_parcel_Priority.SelectedItem;
        }

        private void TextBox_set_target_id(object sender, TextChangedEventArgs e)
        {

            int s;
            int idNew = 0;
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
                MessageBox.Show("Please enter a positive number");
                show_parcel_target.Background = Brushes.Red;
            }
        }

        private void TextBox_set_sender_id(object sender, TextChangedEventArgs e)
        {
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
                MessageBox.Show("Please enter a positive number");
                show_parcel_sender.Background = Brushes.Red;
            }
        }

        private void enter_your_parcel(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            if (set_parcel_Priority.SelectedItem == null)
            {
                MessageBox.Show("please enter priority");
                flag = true;
            }
            if (set_parcel_Weight.SelectedItem == null)
            {
                MessageBox.Show("please enter weight");
                flag = true;
            }
            if (show_parcel_id.Background == Brushes.Red)
            {

                MessageBox.Show("please enter id");
                flag = true;
            }

            if (show_parcel_sender.Background == Brushes.Red)
            {
                MessageBox.Show("please enter id of sender");
                flag = true;
            }
            if (show_parcel_target.Background == Brushes.Red)
            {
                MessageBox.Show("please enter id of target");
                flag = true;
            }
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
                catch (BO.AddException exc)
                {
                    MessageBox.Show(exc.ToString());
                }
                catch (BO.GetException exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            }

        }
    }
}
