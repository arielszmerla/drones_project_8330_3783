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
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneAdd : Window
    {
        private IBL.IBL bl1;

        public DroneAdd()
        {
            InitializeComponent();
        }

        public DroneAdd(IBL.IBL bl1)
        {
            InitializeComponent();
            this.bl1 = bl1;
        }
    }
}
