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
            this.bl = bl;

        }
        BO.BaseStation bs = new();
        private void End_the_page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bs, bl).Show();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            int s;

            if (int.TryParse(ChooseId.Text, out s))
            {
                if(s > 0)
                {
                    ID.Background = Brushes.Transparent;
                    bs.Id = s;
                }
                else
                {
                    ChooseId.Text = "";
                    MessageBox.Show("Please enter a positive number");
                    ChooseId.Background = Brushes.Red;
                }
            }
           
        }

        private void Name_input(object sender, TextChangedEventArgs e)
        {
            string name;
            if()

        }
    }
}
