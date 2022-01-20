using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Customer show list
    /// </summary>
    public partial class CustomerActionWindow : Window
    {
        int id;
        double latitude { get; set; }
        double longitude { get; set; }
        string phone { get; set; }
        BO.Customer subscribe = new();
        private BLAPI.IBL bl1;
        public static Model Model { get; } = Model.Instance;
        /// <summary>
        /// show customer
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="n"></param>
        /// <param name="v"></param>
        public CustomerActionWindow(BLAPI.IBL bl, int n, int v = 0)
        {
            InitializeComponent();
            this.bl1 = bl;
            if (v == 0)
            {
                try
                {
                    bl1.DeleteCustomer(n);
                    MessageBox.Show("Delete Done!");
                }
                catch (BO.DeleteException exc)
                {
                    Model.Error(exc.Message);

                    Close();
                }
            }
            else
            {
                customers.Add(bl.GetCustomerToList(n));
                CustomerView.ItemsSource = customers;
                id = customers[0].Id;
            }


        }
        public CustomerActionWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            this.bl1 = bl;
            add_customer_stack.Visibility = Visibility.Visible;
            add_CUSTOMER_titles.Visibility = Visibility.Visible;
            CustomerView.Visibility = Visibility.Collapsed;
            updates.Visibility = Visibility.Collapsed;
        }
        List<BO.CustomerToList> customers = new();
        BO.Customer customer = new();
        /// <summary>
        /// show customer windows
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="cus"></param>
        public CustomerActionWindow(BLAPI.IBL bl, BO.CustomerToList cus)
        {
            InitializeComponent();
            this.bl1 = bl;
            id = cus.Id;
            customers.Add(cus);
            CustomerView.ItemsSource = customers;
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Update(object sender, RoutedEventArgs e)
        {
            if (Phone.Text != "\r" && Phone.Text != "") //checks that key wasn't enter
            {
                Regex myReg = new Regex("[^0-9]+"); //gets regular expression that allows only digits
                if (!myReg.IsMatch(Phone.Text)) //checks taht key entered is regular expression
                    customer.Phone = Phone.Text;
                else
                {
                    Phone.Text = "";

                    Model.Error("Please enter a positive number");

                    Phone.Background = Brushes.Red;
                    return;
                }
            }
            customer.Name = Name_update.Text;

            if (Name_update.Text == "" && Phone.Text == "")
            {
                MessageBoxButton b = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Nothing to update, Please fill a box\nTo close press 'NO'", "Choice Box", b);
                if (result == MessageBoxResult.No)
                {
                    Close();
                }
                else
                {
                    Phone.Background = Brushes.Red;
                    Name_update.Background = Brushes.Red;
                }
            }
            else
            {
                try
                {
                    bl1.UpdateCustomerInfo(id, Name_update.Text, Phone.Text);
                    MessageBox.Show("Update Managed!", "GOOD JOB");
                    Close();

                }
                catch (BO.GetException exc)
                {
                    Model.Error(exc.Message);

                }

            }
        }


        private void _SelectCustomerViewListionChanged(object sender, SelectionChangedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// add a client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            Regex myReg = new Regex("[^0-9]+"); //gets regular expression that allows only digits
            if (!myReg.IsMatch(ChooseId.Text)) //checks taht key entered is regular expression

                subscribe.Id = Int32.Parse(ChooseId.Text);

            else
            {
                ChooseId.Text = "";
                Model.Error("Please enter a positive number");
                ChooseId.Background = Brushes.Red;
            }
            if (!myReg.IsMatch(ChoosePhone.Text)) //checks taht key entered is regular expression
                subscribe.Phone = ChoosePhone.Text;
            else
            {
                Phone.Text = "";
                Model.Error("Please enter a positive number");
                Phone.Background = Brushes.Red;
            }
            Location loc = new();
            if (myReg.IsMatch(ChooseLatitude.Text))
                if (double.Parse(ChooseLatitude.Text) >= 0)
                    loc.Latitude = double.Parse(ChooseLatitude.Text);
            if (ChooseLatitude.Text == "")
            {
                Model.Error("Please, number > 0");
                ChooseLatitude.Background = Brushes.Red;
            }
            if (myReg.IsMatch(ChooseLongitude.Text))
                if (double.Parse(ChooseLongitude.Text) >= 0)
                    loc.Longitude = double.Parse(ChooseLongitude.Text);
            if (ChooseLongitude.Text == "")
            {
                Model.Error("Please, number > 0");
                ChooseLongitude.Background = Brushes.Red;

            }
            if (ChoosePhone.Text != "" && ChooseId.Text != "" && ChooseLongitude.Text != "" && ChooseLatitude.Text != "" && ChooseId.Text != "")
            {
                subscribe.Location = loc;
                try
                {
                    bl1.AddCustomer(subscribe);
                    MessageBox.Show("WELLCOME SIR");
                    Close();
                }
                catch (BO.AddException ex)
                {
                    Model.Error(ex.Message);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addParcel(object sender, RoutedEventArgs e)
        {
            new ParcelActionWindow(bl1).Show();
        }
    }
}

