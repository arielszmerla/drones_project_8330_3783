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
        public static Model Model { get; } = Model.Instance;
        BLAPI.IBL bl;
        /// <summary>
        /// cunstrocter
        /// </summary>
        /// <param name="bl"></param>
        public CustomerListWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            CustomerViewList.ItemsSource = bl.GetCustomerList();
        }

        /// <summary>
        /// if we double click a customer to view his info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.CustomerToList  customer = (BO.CustomerToList)CustomerViewList.SelectedItem;
            if (customer == null)
            {
                Model.Error("click on a Customer please");
            }
            else
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Do you want to update ?", "Make Your Choice", button);
                if (result == MessageBoxResult.Yes)
                {
                    //Closing_Button.Visibility = Visibility.Hidden;
                    new CustomerActionWindow(bl, customer).Show();
                   
                }
                else if (result==MessageBoxResult.No)
                {
                    result = MessageBox.Show("Do you want to delete?", "Make Your Choice", button);
                    if (result == MessageBoxResult.Yes)
                    {
                        //  Closing_Button.Visibility = Visibility.Hidden;
                        new CustomerActionWindow(bl, customer.Id).Show();
                    }
                }
              
                    result = MessageBox.Show("Do you want to see the parcels?", "Make Your Choice", button);
                    if (result == MessageBoxResult.Yes)
                    {
                        new ParcelListWindow(bl, customer.Name).Show();
    
                }
                
            }
        }
        /// <summary>
        /// end page function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
