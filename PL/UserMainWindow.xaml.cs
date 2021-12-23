using BLAPI;
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
using PO;
using BLAPI;
namespace PL
{
    /// <summary>
    /// Interaction logic for UserMainWindow.xaml
    /// </summary>
    public partial class UserMainWindow : Window
    {

        public UserMainWindow(IBL bl,int a)

        {
            this.bl = bl;
            InitializeComponent();
            UserGrid.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            
        }
        BLAPI.IBL bl;
        public UserMainWindow(IBL bl)

        {
            this.bl = bl;
            InitializeComponent();
            List<User> l = new();
            foreach (var cs in bl.GetCustomerList(null))
            {
                l.Add(new User { Name = cs.Name, Phone = cs.Phone, UserId = cs.Id });
            }
            this.UserGrid.ItemsSource = l;

        }
        private int id;
        private User user;
        public UserMainWindow(int id, IBL bl)

        {
            this.bl = bl;
            this.id = id;
            InitializeComponent();
            List<User> l = new();
            bl.GetCustomerList(cs => cs.Id == id);
            foreach (var cs in bl.GetCustomerList(cs => cs.Id == id)) {
                l.Add(new User { Name = cs.Name, Phone = cs.Phone, UserId = cs.Id });

            }
            this.UserGrid.ItemsSource = l;
            client_choices.Items.Add("add a parcel");
            client_choices.Items.Add("show parcels");
            client_choices.Items.Add("permisions");

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            client_choices.Visibility = Visibility.Visible;

        }

 

        private void client_choices_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (client_choices.SelectedItem == "add a parcel")
            {
                new ParcelActionWindow(bl).Show();
            }
            else if (client_choices.SelectedItem == "show parcels")
            {
                new ParcelListWindow(bl, txtFirstName.Text).Show();


            }
            else 
            {


            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int s;
            BO.Customer c = new();
            BO.Location loc = new();
            double lat=0;
            double longi=0;
            // first we insert id into the base station
            if (int.TryParse(txtUserId.Text, out s))
            {
                if (s > 0)
                {
                    txtUserId.Background = Brushes.Bisque;
                    c.Id = s;
                }
                else
                {
                    txtUserId.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    txtUserId.Background = Brushes.Red;
                }
            }
            else {

                txtUserId.Text = "";
                MessageBox.Show("Please enter a positive number");
                txtUserId.Background = Brushes.Red;

            }
            if (double.TryParse(latitue.Text, out lat))
            {
                if (lat > 0)
                {
                    latitue.Background = Brushes.Bisque;

                }
                else
                {
                    latitue.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    latitue.Background = Brushes.Red;
                }
            }
            else {

                latitue.Text = "";
                MessageBox.Show("Please enter a positive number");
                latitue.Background = Brushes.Red;


            }
            if (double.TryParse(longitue.Text, out longi))
            {
                if (longi > 0)
                {
                    latitue.Background = Brushes.Bisque;
                }
                else
                {
                    longitue.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    longitue.Background = Brushes.Red;
                }
            }
            else
            {

                longitue.Text = "";
                MessageBox.Show("Please enter a positive number");
                longitue.Background = Brushes.Red;


            }

            // entering name into customer

            if (txtFirstName.Text == "")
            {
                MessageBox.Show("Please enter a name");
                txtFirstName.Background = Brushes.Red;
            }
            else
            {
                txtFirstName.Background = Brushes.Transparent;
                c.Name = txtFirstName.Text;
            }

            // number phone

            if (txtphone.Text == "")
            {
                MessageBox.Show("Please enter a phone number");
                txtphone.Background = Brushes.Red;
            }
            else
            {
                txtphone.Background = Brushes.Transparent;
                c.Phone = txtphone.Text;
            }
            if (lat != 0 && longi != 0)
            {
                loc.Latitude = lat;
                loc.Longitude = longi;
            }
            if (longi != 0 && lat != 0 && txtphone.Text != "" && txtUserId.Text != "" && txtFirstName.Text != "") 
            {
                c.Location = loc;
                try {
                    bl.AddCustomer(c);
                    MessageBox.Show("signed in");
                    new UserMainWindow(c.Id, bl).Show();
                }
                catch( BO.AddException ex) { 
                    MessageBox.Show(ex.ToString());
                }


            }
        }
    }
}
