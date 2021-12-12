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
using IBL;
using IBL.BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IBL.IBL bl = new IBL.BL();
        public MainWindow()
        {
            bl = new BL();
            InitializeComponent();
        }

      
        private void DronesList_Click_1(object sender, RoutedEventArgs e)
        {
            new DroneListWindow1(bl).Show();
        }

       
    }
}
