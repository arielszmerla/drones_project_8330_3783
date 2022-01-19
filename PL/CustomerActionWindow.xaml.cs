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
    }
}
