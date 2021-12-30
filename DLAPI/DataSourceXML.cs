using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DO;
using DalXML;
using DLAPI;

namespace DS
{
    public class DataSourceXML
    {
        static string stationsPath = @"xml\stations.xml";
        XElement stationRoot;
        public XElement BaseStations
        {
            get
            {
               // stationRoot=LoadD

                return stationRoot;
            }
            
        }
      public DataSourceXML() {
            XMLTolls.SaveListToXMLSerializer(DS.DataSource.Drones, dronesPath);
            XMLTolls.SaveListToXMLSerializer(DS.DataSource.Parcels, parcelsPath);
            XMLTolls.SaveListToXMLSerializer(DS.DataSource.Customers, customersPath);
            SaveBaseList(DS.DataSource.BaseStations);
        }

        public void SaveBaseList(List<DO.BaseStation> List)
        {
            stationRoot = new XElement("BaseStations");

            foreach (BaseStation item in List)
            {
                XElement id = new XElement("id", item.Id);
                XElement name = new XElement("name", item.Name);
                XElement numOfSlots = new XElement("numOfSlots", item.NumOfSlots);
                XElement latitude = new XElement("latitude", item.Latitude);
                XElement longitude = new XElement("longitude", item.Longitude);
                XElement valid = new XElement("valid", item.Valid);
                XElement baseStation = new XElement("baseStation", id, name, numOfSlots, latitude, longitude, valid);
                stationRoot.Add(baseStation);
            }
            stationRoot.Save(stationsPath);

        }
        static string dronesPath = @"drones.xml";
        static string parcelsPath = @"parcels.xml";
        static string customersPath = @"customer.xml";


        


    }
}
