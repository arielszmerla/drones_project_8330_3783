﻿using IBL;
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
        public DroneListWindow1(IBL bl1)
        {
            InitializeComponent();
            this.bl1 = bl1;
            DroneListView.ItemsSource = bl1.GetDroneList();
            StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.Enums.DroneStatuses));
            WeightChoise.ItemsSource = Enum.GetValues(typeof(BO.Enums.WeightCategories));
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
            Close();
            new AddDrone(bl1).Show();

        }

        private void drone_action(object sender, MouseButtonEventArgs e)
        {
            BO.DroneToList drone = (BO.DroneToList)DroneListView.SelectedItem;
            BO.Drone dr = new BO.Drone
            {
                Id = drone.Id,
                BatteryStatus = drone.BatteryStatus,
                DronePlace = drone.DroneLocation,
                MaxWeight = drone.MaxWeight,
                Model = drone.Model,
                PID = null,
                Status = drone.Status
            };
            Closing_Button.Visibility = Visibility.Hidden;
            new AddDrone(bl1, dr).Show();
            Close();
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
        private void DroneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Reset_List(object sender, RoutedEventArgs e)
        {
            DroneListView.ItemsSource = bl1.GetDroneList();
            WeightChoise.SelectedItem = -1;
            StatusSelector.SelectedItem = -1;
            reset.Visibility = Visibility.Hidden;
        }
    }
}