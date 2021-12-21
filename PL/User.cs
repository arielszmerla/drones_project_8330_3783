using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace PO
{
    public class User : INotifyPropertyChanged
    {
        private int numberOfParcelsSentAndDelivered;
        private int numberOfParcelsSentButNotDelivered;
        private int numberOfParcelsReceived;
        private int numberOfParcelsonTheWay;
        private int userId;
        private string name;
        private string phone;
  
        public int UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
                OnPropertyChanged("UserId");
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }
        public int NumberOfParcelsSentAndDelivered
        {
            get
            {
                return numberOfParcelsSentAndDelivered;
            }
            set
            {
                numberOfParcelsSentAndDelivered = value;
                OnPropertyChanged("NumberOfParcelsSentAndDelivered");
            }
        }

        public int NumberOfParcelsSentButNotDelivered
        {
            get
            {
                return numberOfParcelsSentButNotDelivered;
            }
            set
            {
                numberOfParcelsSentButNotDelivered = value;
                OnPropertyChanged("NumberOfParcelsSentButNotDelivered");
            }
        }
        public int NumberOfParcelsReceived
        {
            get
            {
                return numberOfParcelsReceived;
            }
            set
            {
                numberOfParcelsReceived = value;
                OnPropertyChanged("NumberOfParcelsReceived");
            }
        }
        public int NumberOfParcelsonTheWay
        {
            get
            {
                return numberOfParcelsonTheWay;
            }
            set
            {
                numberOfParcelsonTheWay = value;
                OnPropertyChanged("NumberOfParcelsonTheWay");
            }
        }

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
