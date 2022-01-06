using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DLAPI
{

    public static class DLFactory
    {
        public static IDal GetDL(string str)
        {
            switch (str)
            {
                case "1":
                    {
                        
                        string dlType = DLConfig.DalName;
                        string dlPkg = DLConfig.DalPackages[dlType];
                        if (dlPkg == null)
                            throw new DLConfigException($"Wrong DL type: {dlType}");
                        try
                        {
                            Assembly.Load(dlPkg);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw new DLConfigException($"Failed loading {dlPkg}.dll", ex);
                        }
                        Type type = Type.GetType($"DalObject.{dlPkg}, {dlPkg}");
                        if (type == null) throw new DLConfigException($"Class name is not the same as Assembly Name: {dlPkg}");
                        IDal dal = (IDal)type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                        if (dal == null) throw new DLConfigException($"Class {dlPkg} is not a singleton");
                        return dal;
                    }
                    break;
               case "2":
                    {

                        string dlType = DLConfig.DalName;
                        string dlPkg = DLConfig.DalPackages["xml"];
                       
                        if (dlPkg == null)
                            throw new DLConfigException($"Wrong DL type: {dlType}");
                        try
                        {
                            Assembly.Load(dlPkg);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw new DLConfigException($"Failed loading {dlPkg}.dll", ex);
                        }
                        Type type = Type.GetType($"DalXML.{dlPkg}, {dlPkg}");
                        if (type == null) throw new DLConfigException($"Class name is not the same as Assembly Name: {dlPkg}");
                        IDal dal = (IDal)type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                        if (dal == null) throw new DLConfigException($"Class {dlPkg} is not a singleton");
                        return dal;
                    }
                    break;
                default:
                    throw new DLConfigException($"didn't send a parameter,{str} is a wrong dl type ");


            }
        }
    }
}









