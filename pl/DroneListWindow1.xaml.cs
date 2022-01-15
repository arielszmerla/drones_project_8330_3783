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
using BLAPI;
using BO;


namespace PL
{

    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow1 : Window
    {

        private IBL bl1;
        private int id;
        private IBL bl;
        List<PO.DroneToList> poDroneToList = new();
        public static Model Model { get; } = Model.Instance;

        void Window_Loaded(object sender, RoutedEventArgs e) => Model.DronesRefresh();
        // drone list view
        public DroneListWindow1(IBL bl1)
        {

            InitializeComponent();
            this.bl1 = bl1;

   
       
            StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.Enums.DroneStatuses));
            WeightChoise.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DroneListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Model");
            view.GroupDescriptions.Add(groupDescription);

        }



        public DroneListWindow1(int id, IBL bl)
        {
            this.id = id;
            this.bl = bl;
            InitializeComponent();
            DroneListView.ItemsSource = bl.GetDronesInBaseStationList(id);
            StatusSelector.Visibility = Visibility.Hidden;
            Status_label.Visibility = Visibility.Hidden;
            Weight_label.Visibility = Visibility.Hidden;
            WeightChoise.Visibility = Visibility.Hidden;
            actions.Visibility = Visibility.Hidden;
           // DroneListView.MouseDoubleClick -= drone_action;


        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneListView.ItemsSource = bl1.GetDroneList((BO.Enums.DroneStatuses?)StatusSelector.SelectedItem, (BO.Enums.WeightCategories?)WeightChoise.SelectedItem);
            reset.Visibility = Visibility.Visible;
        }

        private void WeightChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneListView.ItemsSource = bl1.GetDroneList((BO.Enums.DroneStatuses?)StatusSelector.SelectedItem, (BO.Enums.WeightCategories?)WeightChoise.SelectedItem);
            reset.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Closing_Button.Visibility = Visibility.Hidden;
            new AddDrone(bl1).Show();

        }
        private void Drone_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            new AddDrone(bl,((DroneToList)((ListViewItem)sender).DataContext).Id).Show();

        }
        private void Closing_Button_Click(object sender, RoutedEventArgs e)
        {
            Closing_Button.Visibility = Visibility.Hidden;
            Close();
        }
        private void list_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Closing_Button.Visibility != Visibility.Hidden)
                e.Cancel = true;
        }
        private void DroneListView_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if(sender!=null)
            new AddDrone(bl, ((DroneToList)((ListView)sender).SelectedItem).Id).Show();
        }

        private void Reset_List(object sender, RoutedEventArgs e)
        {

            DroneListView.ItemsSource = bl1.GetDroneList();
            WeightChoise.SelectedItem = null;
            StatusSelector.SelectedItem = null;
            reset.Visibility = Visibility.Hidden;

        }

        private void View_Map(object sender, RoutedEventArgs e)
        {
            new MapsDisplay(bl1).Show();
        }
    }
}
