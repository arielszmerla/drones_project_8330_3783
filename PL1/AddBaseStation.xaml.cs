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
using BO;
using BLAPI;

namespace PL
{
    /// <summary>
    /// Interaction logic for AddBaseStation.xaml
    /// </summary>
    public partial class AddBaseStation : Window
    {
        private Location loc = new();
        private IBL bl;
        public AddBaseStation(IBL bl)
        {
            Title = "ADD A BASE STATION";
            InitializeComponent();
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
