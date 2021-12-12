using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DLAPI
{
    public static class DLFactory
    {
        public static IDal GetDL()
        {
            string dlType = DLConfig.DLName;
            DLConfig.DLPackage dLPackage;
            try
            {
                dLPackage = DLConfig.DLPackages[dlType];
            }
            catch (KeyNotFoundException ex)
            {
                throw new DLConfigException($"Wrong DL type: {dlType}", ex);
            }
            string dlPackageName = dLPackage.PkgName;
            string dlNameSpace = dLPackage.NameSpace;
            string dlClass = dLPackage.ClassName;

            try
            {
                Assembly.Load(dlPackageName);

            }
            catch (KeyNotFoundException ex)
            {
                throw new DLConfigException($"Failed loading {dlPackageName}.dll", ex);
            }
            Type type = Type.GetType($"DalObject.{dlPackageName}, {dlPackageName}");
            if (type == null)
                throw new DLConfigException($"Class name is not the same as Assembly Name: {dlPackageName}");
            try
            {
                IDal dal = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null) as IDal;
                if (dal == null)
                    throw new DLConfigException($"Class {dlPackageName} instance is not initialized");
                return dal;
            }
            catch (NullReferenceException ex)
            {
                throw new DLConfigException($"Class {dlPackageName} is not a singleton", ex);
            }

        }
    }
}
