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
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        BLAPI.IBL bl1;
        public CustomerListWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            bl1 = bl;
            CustomerViewList.ItemsSource = bl.GetCustomerList();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.CustomerToList  customer = (BO.CustomerToList)CustomerViewList.SelectedItem;
            if (customer == null)
            {
                MessageBox.Show("click on a Customer please");
            }
            else
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Do you want to update ?", "Make Your Choice", button);
                if (result == MessageBoxResult.Yes)
                {
                    //Closing_Button.Visibility = Visibility.Hidden;
                    new CustomerActionWindow(bl1, customer).Show();
                   
                }
                else
                {
                    result = MessageBox.Show("Do you want to delete?", "Make Your Choice", button);
                    if (result == MessageBoxResult.Yes)
                    {
                        //  Closing_Button.Visibility = Visibility.Hidden;
                        new CustomerActionWindow(bl1, customer.Id).Show();
                    }
                }
            }
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
