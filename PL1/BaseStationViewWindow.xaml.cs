using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for BaseStationViewWindow.xaml
    /// </summary>
    public partial class BaseStationViewWindow : Window
    {
        private BLAPI.IBL bl;
        enum options {Regular, Free_Base_Stations, Num_Of_Free_Bases }

        public BaseStationViewWindow(BLAPI.IBL bl)
        { 
            InitializeComponent();
            this.bl = bl;
            BaseViewOptions.ItemsSource = Enum.GetValues(typeof(options));
            BaseStationView.ItemsSource = bl.GetBaseStationList(vs => vs.Id == 10000);
        }

        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BaseViewOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            options stat = (options)((ComboBox)sender).SelectedItem;
            switch(stat)
            {
                case options.Regular:
                  //  BaseStationView.BindingGroup
                    //bl.GetBaseStationList();
                    break;
                case options.Free_Base_Stations:
                    break;
                case options.Num_Of_Free_Bases:
                    break;
            }
        }

        private void ResetList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
