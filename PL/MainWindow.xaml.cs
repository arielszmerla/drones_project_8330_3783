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
using System.Text.RegularExpressions;


namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static SoundPlayer _backgroundMusic = new SoundPlayer();
    
      
       
        BLAPI.IBL bl = BLFactory.GetBL();
        public MainWindow()
        {
            InitializeComponent();
            string soundFilePath = @"mus\hatikva.wav";
            SoundPlayer player = new SoundPlayer(soundFilePath);
            player.Play();
            
        }

        /// <summary>
        /// if entering as a client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_Entry_Click(object sender, RoutedEventArgs e)
        {
            #region visibilaties
            Manager_Entry.Visibility = Visibility.Collapsed;
            Sign_In.Visibility = Visibility.Collapsed;
            LogIn.Visibility = Visibility.Collapsed;
            enterPassword.Visibility = Visibility.Collapsed;
            password.Visibility = Visibility.Collapsed;
            Enter_But.Visibility = Visibility.Visible;
            #endregion

            if (id_check.Text == "")
                MessageBox.Show("please enter your id");
            id_check.Visibility = Visibility.Visible;
            if (id_check.Text != "")
            {
                Regex myReg = new Regex("[^0-9]+"); //gets regular expression that allows only digits
                if (myReg.IsMatch(id_check.Text)) //checks taht key entered is regular expression
                {
                    id_check.Background = Brushes.Red;
                    MessageBox.Show("Please enter only numbers");
                    id_check.Text = "";
                }


            }

        }
        /// <summary>
        /// enter as a manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manager_Entry_Click(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Visible;
            LogIn.Visibility = Visibility.Visible;
            enterPassword.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// checks valid password
        /// </summary>
        /// <param name="s"></param>
        /// <param name="sum"></param>
        private void chek(int s, int sum)
        {

            if (password.Password == "1234")
            {
                password.Password = "";
                MessageBox.Show("Welcome sir!");
                choice.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Collapsed;
                Manager_Entry.Visibility = Visibility.Collapsed;
                Client_Entry.Visibility = Visibility.Collapsed;
                LogIn.Visibility = Visibility.Collapsed;
                enterPassword.Visibility = Visibility.Collapsed;
                Sign_In.Visibility = Visibility.Collapsed;
            }
            else
            {
                password.Password = "";
                password.Background = Brushes.Red;
                MessageBox.Show("Please try again");
            }


        }

        /// <summary>
        /// sign in for new client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sign_In_Click(object sender, RoutedEventArgs e)
        {
            new CustomerActionWindow(bl).Show();
        }
        /// <summary>
        /// login as manafer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (password.Password == "1234")
            {
                password.Password = "";
                MessageBox.Show("Welcome sir!");
                choice.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Collapsed;
                Manager_Entry.Visibility = Visibility.Collapsed;
                Client_Entry.Visibility = Visibility.Collapsed;
                LogIn.Visibility = Visibility.Collapsed;
                enterPassword.Visibility = Visibility.Collapsed;
                Sign_In.Visibility = Visibility.Collapsed;
            }
            else
            {
                password.Password = "";
                password.Background = Brushes.Red;
                MessageBox.Show("Please try again");
            }
        }
        /// <summary>
        /// get drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drones_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow1(bl).Show();
        }
        /// <summary>
        /// get bases window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bases_Click(object sender, RoutedEventArgs e)
        {
            new BaseStationViewWindow(bl).Show();
        }
        /// <summary>
        /// get parcel window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ParcelListWindow(bl).Show();
        }
        /// <summary>
        /// get customer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(bl).Show();
        }

        /// <summary>
        /// button to check the password of a client and open customer action window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.Customer? b = bl.GetCustomer(int.Parse(id_check.Text));
                new CustomerActionWindow(bl, b.Id, 1).Show();
            }
            catch (BO.GetException)
            {

                MessageBox.Show("this customer doesn't exist");
                id_check.Text = "";
            }
        }
    }


}
