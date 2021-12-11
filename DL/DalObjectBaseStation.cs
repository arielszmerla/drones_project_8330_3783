using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DLAPI;
using DO;

namespace DL
{
    internal partial class DalObject : IDal
    {
 
        /// <summary>
        /// gets basestation from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns the basestation got>
        public BaseStation GetBaseStation(int id)
        {
            BaseStation? myBase = null;
            myBase = DataSource.BaseStations.Find(bs => bs.Id == id);
            if (myBase == null)
                throw new BaseExeption("id of base not found");
            return (BaseStation)myBase;
        }
        /// <summary>
        /// send a new base to database
        /// </summary>
        /// <param name="baseStation"></param>
        public void AddBaseStation(BaseStation baseStation)
        {
            if (DataSource.BaseStations.Any(bs => bs.Id == baseStation.Id))
            {
                throw new BaseExeption("id allready exist");
            }
            DataSource.BaseStations.Add(baseStation);
        }
        /// <summary>
        /// get list of base stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseStation> GetAllBaseStations(Func<BaseStation, bool> predicat = null)
        {
            if (predicat == null)
                return DataSource.BaseStations.ToList();
            else
                return (from item in DataSource.BaseStations
                        where predicat(item)
                        select item);

        }
        /// <summary>
        /// update in dal a basestation
        /// </summary>
        /// <param name="bs"></param>
        public void UpdateBaseStationFromBl(BaseStation bs)
        {
            int index = DataSource.BaseStations.FindIndex(ba => ba.Id == bs.Id);
            if (index == -1)
            {
                throw new BaseExeption($"base station {bs.Id} not found\n");
            }
            DataSource.BaseStations[index] = bs;
        }


    }

}