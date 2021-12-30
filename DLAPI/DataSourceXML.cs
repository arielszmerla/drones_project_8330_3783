using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DO;

namespace DS
{
    internal class DataSourceXML
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
        static string dronesPath = @"xml\drones.xml";
        static string parcelsPath = @"xml\parcels.xml";

    }
}
