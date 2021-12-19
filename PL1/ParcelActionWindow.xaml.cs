using BLAPI;
using BO;
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
    /// Interaction logic for ParcelActionWindow.xaml
    /// </summary>
    public partial class ParcelActionWindow : Window
    {
        private IBL bl;
        private ParcelToList p;

        public ParcelActionWindow()
        {
            InitializeComponent();
        }

        public ParcelActionWindow(IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
        }

        public ParcelActionWindow(IBL bl, ParcelToList p) 
        {
            InitializeComponent();
            this.bl = bl;
            this.p = p;
            DataContext = bl.GetParcel(p.Id).ToString();
            this.show_parcel.Text = (string)DataContext;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }
    }
}
