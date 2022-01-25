using System.ComponentModel;
using BO;
using BLAPI;
using System.Collections.ObjectModel;
using System.Windows;
using System.Media;

namespace PL
{
    /// <summary>
    /// model to all the other classes
    /// </summary>
    public class Model : INotifyPropertyChanged
    {
        //event for changes in drone list
        public event PropertyChangedEventHandler PropertyChanged;
        static readonly IBL bl = BLFactory.GetBL();

        Model() { }
        /// <summary>
        /// singleton creation
        /// </summary>
        public static Model Instance { get; } = new Model();
        /// <summary>
        /// observable list so changes are felt by event
        /// </summary>
        ObservableCollection<DroneToList> drones = new(bl.GetDroneList());
        public ObservableCollection<DroneToList> Drones
        {
            get => drones;
            private set
            {
                drones = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Drones)));
            }
        }


        Enums.DroneStatuses? statusSelector =null;
        public Enums.DroneStatuses? StatusSelector
        {
            get => statusSelector;
            set
            {
                statusSelector = value;
                DronesRefresh();
            }
        }

        Enums.WeightCategories? weightSelector = null;
        public Enums.WeightCategories? WeightSelector
        {
            get => weightSelector;
            set
            {
                weightSelector = value;
                DronesRefresh();
            }
        }
        //refresh list on changing conditions
        public void DronesRefresh()
        {
            Drones = new(bl.GetDroneList((BO.Enums.DroneStatuses?)StatusSelector, (Enums.WeightCategories?)WeightSelector));
        }
        /// <summary>
        /// error model announcemenet
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(string ex)
        {
            SystemSounds.Beep.Play();
            MessageBox.Show(ex,"", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}

