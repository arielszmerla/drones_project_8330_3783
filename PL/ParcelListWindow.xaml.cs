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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    public partial class ParcelListWindow : Window
    {
        BLAPI.IBL bl;
        public ParcelListWindow(BLAPI.IBL bl)
        {

            InitializeComponent();
            this.bl = bl;
            ParcelViewList.ItemsSource = bl.GetParcelList();
            DataContext = ParcelViewList.ItemsSource;
            Weight_Choice.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));
            you_want_grouping.Items.Add("SenderName");
            you_want_grouping.Items.Add("TargetName");

        }

        private void Closing_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Weight_Choice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelViewList.ItemsSource = bl.GetParcelList((BO.Enums.WeightCategories?)Weight_Choice.SelectedItem);
            reset.Visibility = Visibility.Visible;
        }


        private void you_want_grouping_Checked(object sender, SelectionChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelViewList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription(you_want_grouping.SelectedItem.ToString());
            view.GroupDescriptions.Add(groupDescription);
            ParcelViewList.Items.Refresh();
        }

        private void Reset_List(object sender, RoutedEventArgs e)
        {

            ParcelViewList.ItemsSource = (System.Collections.IEnumerable)DataContext;
            reset.Visibility = Visibility.Collapsed;
        }




        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new ParcelActionWindow(bl).Show();
            ParcelViewList.Items.Refresh();
            Close();
        }


        private void ParcelViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.ParcelToList p = (BO.ParcelToList)ParcelViewList.SelectedItem;
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Do you want to update ?", "Make Your Choice", button);
            if (result == MessageBoxResult.Yes)
            {
                Closing_Button.Visibility = Visibility.Hidden;
                new ParcelActionWindow(bl, p).Show();
                Close();
            }
            else
            {
                result = MessageBox.Show("Do you want to delete?", "Make Your Choice", button);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bl.DeleteParcel(p.Id);
                        MessageBox.Show("Delete done");
                    } 
            catch (BO.DeleteException exc)
                { MessageBox.Show(exc.ToString()); }
                Closing_Button.Visibility = Visibility.Hidden;
                
                   
                }
            }
        }
    }
}
