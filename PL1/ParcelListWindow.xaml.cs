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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    public partial class ParcelListWindow : Window
    {
        BLAPI.IBL bl;
        public ParcelListWindow(BLAPI.IBL bl)
        {
           
            InitializeComponent();
         ParcelViewList.ItemsSource= bl.GetParcelList();
            DataContext = ParcelViewList.ItemsSource;
            Weight_Choice.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelViewList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("WeightCategorie");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void Closing_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CustomerViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Weight_Choice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // IEnumerable <BO.ParcelToList> p= bl.GetParcelList((BO.Enums.WeightCategories)sender);
          
        }
    }
}
