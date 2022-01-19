using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using PO;
using BLAPI;
namespace PL
{


    class UserViewModel
    {
        private IList<User> _UsersList;
        public UserViewModel()
        {



            _UsersList = new List<User>
            {
                new User{UserId = 1,Name="Raj",Phone="Beniwal"},
                new User{UserId=2,Name="Raj",Phone="Beniwal"},
                new User{UserId=3,Name="Raj",Phone="Beniwal"},
                new User{UserId=4,Name="Raj",Phone="Beniwal"},

            };
        }

        public IList<User> Users
        {
            get { return _UsersList; }
            set { _UsersList = value; }
        }

        private ICommand mUpdater;
        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set
            {
                mUpdater = value;
            }
        }

        private class Updater : ICommand
        {


            #region ICommand Members  

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {

            }

            #endregion

        }
    }
}

