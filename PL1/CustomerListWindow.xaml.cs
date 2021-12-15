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

namespace PL1
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        BLAPI.IBL bl;
        public CustomerListWindow(BLAPI.IBL bl)
        {
            InitializeComponent();
            CustomerViewList.ItemsSource = bl.GetCustomerList();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
