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

        public UserMainWindow()

        {
            InitializeComponent();

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
    }
}
