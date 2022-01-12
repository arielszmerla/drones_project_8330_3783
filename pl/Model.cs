using System.ComponentModel;
using BO;
using BLAPI;
using System.Collections.ObjectModel;

namespace PL
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        static readonly IBL bl = BLFactory.GetBL();

        Model() { }
        public static Model Instance { get; } = new Model();

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

        public void DronesRefresh()
        {
            Drones = new(bl.GetDroneList((BO.Enums.DroneStatuses?)StatusSelector, (BO.Enums.WeightCategories?)WeightSelector));
        }

    }
}

