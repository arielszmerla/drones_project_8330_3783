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
        public static Model Model { get; } = Model.Instance;
        /// <summary>
        /// build the list page
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="name"></param>
        public ParcelListWindow(BLAPI.IBL bl,string name="")
        {

            InitializeComponent();
            this.bl = bl;
            ParcelViewList.ItemsSource = bl.GetParcelList(name);
            DataContext = ParcelViewList.ItemsSource;
            Weight_Choice.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));
            you_want_grouping.Items.Add("SenderName");
            you_want_grouping.Items.Add("TargetName");
        }
        /// <summary>
        /// close page button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Closing_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// prevent regular closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Closing_Button.Visibility != Visibility.Hidden)
                e.Cancel = true;
        }

        /// <summary>
        /// grouping by weight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Weight_Choice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelViewList.ItemsSource = bl.GetParcelList((BO.Enums.WeightCategories?)Weight_Choice.SelectedItem);
            reset.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// grouping by clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void you_want_grouping_Checked(object sender, SelectionChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelViewList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription(you_want_grouping.SelectedItem.ToString());
            view.GroupDescriptions.Add(groupDescription);
            ParcelViewList.Items.Refresh();
        }
        /// <summary>
        /// reset show all the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_List(object sender, RoutedEventArgs e)
        {
            ParcelViewList.ItemsSource = (System.Collections.IEnumerable)DataContext;
            reset.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// add a parcel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new ParcelActionWindow(bl).Show();
            ParcelViewList.Items.Refresh();
            Close();
        }

        /// <summary>
        /// set event appen clicked on member in listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParcelViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.ParcelToList p = (BO.ParcelToList)ParcelViewList.SelectedItem;
            MessageBoxButton button  = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Do you want to update ?", "Make Your Choice", button);
            ///set update page for parcel
            if (result == MessageBoxResult.Yes)
            {
                Closing_Button.Visibility = Visibility.Hidden;
                Close();
                new ParcelActionWindow(bl, p).Show();
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Do you want to delete?", "Make Your Choice", button);
                //if wants to delete the parcel
                if (res == MessageBoxResult.No)
                {
                    try
                    {
                        bl.DeleteParcel(p.Id);
                        MessageBox.Show("Delete done");
                    }
                    catch (BO.DeleteException exc)
                    {
                        Model.Error(exc.Message);
                    }
                    Closing_Button.Visibility = Visibility.Hidden;
                }
            }
          
        }
    }
}
