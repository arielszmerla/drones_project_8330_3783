using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DLAPI
{
    static class DLConfig
    {
        public class DLPackage
        {
            public string Name;
            public string PkgName;
            public string NameSpace;
            public string ClassName;
        }
        internal static string DLName;
        internal static Dictionary<string, DLPackage> DLPackages;
        static DLConfig()
        {
            XElement dlConfig = XElement.Load(@"config.xml");
            DLName = dlConfig.Element("dl").Value;
            DLPackages = (from pkg in dlConfig.Element("dl-packages").Elements()
                          let tmp1 = pkg.Attribute("namespace")
                          let nameSpace = tmp1 == null ? "DL" : tmp1.Value
                          let tmp2 = pkg.Attribute("class")
                          let className = tmp2 == null ? pkg.Value : tmp2.Value
                          select new DLPackage()
                          {
                              Name = "" + pkg.Name,
                              PkgName = pkg.Value,
                              NameSpace = nameSpace,
                              ClassName = className
                          }).ToDictionary(p => "" + p.Name, p => p);
        }

    }
}
