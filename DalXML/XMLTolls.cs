using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using DO;

namespace DalXML
{
    class XMLTolls
    {
        static string dir = @"xml\";
        static XMLTolls()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

        }
        #region SaveLoadWithXElement
        /// <summary>
        /// Saving List of entities To XMLElement
        /// </summary>
        /// <param name="rootElem">xelement file</param>
        /// <param name="filePath">specific file in the directory</param>
        public static void SaveListToXMLElement(XElement rootElem, string filePath)
        {
            try
            {
                rootElem.Save(dir + filePath);
            }
            catch (Exception ex)
            {
                throw new DO.XMLFileLoadCreateException(filePath, $"fail to create xml file: {filePath}", ex);
            }
        }
        /// <summary>
        /// Loading List of entities from XMLElement
        /// </summary>
        /// <param name="filePath">specific file in the directory</param>
        /// <returns>list of the entities</returns>
        public static XElement LoadListFromXMLElement(string filePath)
        {
            try
            {
                if (File.Exists(dir + filePath))
                {
                    return XElement.Load(dir + filePath);
                }
                else
                {
                    XElement rootElem = new XElement(dir + filePath);
                    rootElem.Save(dir + filePath);
                    return rootElem;
                }
            }
            catch (Exception ex)
            {
                throw new DO.XMLFileLoadCreateException(filePath, $"fail to load xml file: {filePath}", ex);
            }
        }
        #endregion

        #region SaveLoadWithXMLSerializer
        /// <summary>
        /// Saving List of entities To XMLSerializer
        /// </summary>
        /// <typeparam name="T">the entitie's type</typeparam>
        /// <param name="list">list of the entities</param>
        /// <param name="filePath">specific file in the directory</param>
        public static void SaveListToXMLSerializer<T>(List<T> list, string filePath)
        {
            try
            {
                FileStream file = new FileStream(dir + filePath, FileMode.Create);
                XmlSerializer x = new XmlSerializer(list.GetType());
                x.Serialize(file, list);
                file.Close();
            }
            catch (Exception ex)
            {
                throw new DO.XMLFileLoadCreateException(filePath, $"fail to create xml file: {filePath}", ex);
            }
        }
        /// <summary>
        /// Loading List of entities To XMLSerializer
        /// </summary>
        /// <typeparam name="T">the entitie's type</typeparam>
        /// <param name="filePath">specific file in the directory</param>
        /// <returns>list of entities</returns>
        public static List<T> LoadListFromXMLSerializer<T>(string filePath)
        {
            try
            {
                if (File.Exists(dir + filePath))
                {
                    List<T> list;
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    FileStream file = new FileStream(dir + filePath, FileMode.Open);
                    list = (List<T>)x.Deserialize(file);
                    file.Close();
                    return list;
                }
                else
                    return new List<T>();
            }
            catch (Exception ex)
            {
                throw new DO.XMLFileLoadCreateException(filePath, $"fail to load xml file: {filePath}", ex);
            }
        }
        #endregion


    }
}
