using System;
using System.ComponentModel;

namespace PO
{/// <summary>
/// implement parcel class
/// </summary>
    public class Parcel : INotifyPropertyChanged
    {
        private int id;
        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged("id"); }
        }
        public string sender;
        public string Sender
        {
            get => sender; set { sender = value; OnPropertyChanged("sender"); }
        }
        private string target;
        public string Target
        {
            get => target; set { target = value; OnPropertyChanged("target"); }
        }
        private Enums.WeightCategories weightCategories;
        public Enums.WeightCategories WeightCategories
        {
            get => weightCategories; set { weightCategories = value; OnPropertyChanged("weightCategories"); }
        }
        private Enums.Priorities priority;
        public Enums.Priorities Priority
        {
            get => priority; set { priority = value; OnPropertyChanged("priority"); }
        }
     


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}