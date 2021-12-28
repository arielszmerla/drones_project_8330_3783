using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLAPI;
using PL;


namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum listChoice { Drones, BaseStations, Customers, Parcels };
       private static SoundPlayer _backgroundMusic = new SoundPlayer();

        BLAPI.IBL bl = BLFactory.GetBL();
        public MainWindow()
        {
            InitializeComponent();
            ViewOptions.ItemsSource = Enum.GetValues(typeof(listChoice));
            //string soundFilePath = "../../pl/hatikva.wav";
          //  SoundPlayer player = new SoundPlayer(soundFilePath);
      //  player.Play();
        }
    
        
    
        private void DronesList_Click_1(object sender, RoutedEventArgs e)
        {
            new DroneListWindow1(bl).Show();
        }

        private void ViewOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listChoice stat = (listChoice)ViewOptions.SelectedItem;
            switch (stat)
            {//Drones, BaseStations, Customers, Parcels
                case
                    listChoice.BaseStations:
                    new BaseStationViewWindow(bl).Show();
                    Close();
                    break;
                case listChoice.Drones :
                    new DroneListWindow1(bl).Show();
                    break;
                case listChoice.Customers:
                   new CustomerListWindow(bl).Show();
                    break;
                case listChoice.Parcels:
                    new ParcelListWindow(bl).Show();
                    break;
            }
        
        }

        private void Client_Entry_Click(object sender, RoutedEventArgs e)
        {
           // new UserMainWindow(bl).Show();
            Manager_Entry.Visibility = Visibility.Collapsed;
            id_check.Visibility = Visibility.Visible;
        }
        private void Manager_Entry_Click(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Visible;
            LogIn.Visibility = Visibility.Visible;
            enterPassword.Visibility = Visibility.Visible;
        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            password.Background = Brushes.Transparent;
        }
        private void chek(int s,int sum) {

            if (s == 1234) {
                password.Text = "";
                MessageBox.Show("Welcome sir!");
                ViewOptions.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Collapsed;
                Manager_Entry.Visibility = Visibility.Collapsed;
                Client_Entry.Visibility = Visibility.Collapsed;
                LogIn.Visibility = Visibility.Collapsed;
                enterPassword.Visibility = Visibility.Collapsed;
                Sign_In.Visibility = Visibility.Collapsed;
            }
            if (s > sum && s != 1234) {
                password.Text = "";
                password.Background = Brushes.Red;
                MessageBox.Show("Please try again");
            }
      
          
        }

        private void client_id_TextChanged(object sender, TextChangedEventArgs e)
        {

            int s;

            if (int.TryParse(id_check.Text, out s))
            {
                if (s < 0)
                {
                    id_check.Background = Brushes.Red;
                    MessageBox.Show("Please try again");
                    id_check.Text = "";
                }
            }

            if (s >= 100000000)
            {
                BO.Customer? b = bl.GetCustomer(s);
                if (b == null)
                {
                    MessageBox.Show("this address doesn't exist");
                    id_check.Text = "";

                }
                else
                {
                    new UserMainWindow(b.Id,bl).Show();

                }
                
            }
        }


        private void Sign_In_Click(object sender, RoutedEventArgs e)
        {
            new UserMainWindow(bl, 2).Show();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            int s;

            if (int.TryParse(password.Text, out s))
            {
                if (s < 0)
                {
                    password.Background = Brushes.Red;
                    MessageBox.Show("Please try again");
                }
            }

            chek(s, 1000);

        }
    }
}
