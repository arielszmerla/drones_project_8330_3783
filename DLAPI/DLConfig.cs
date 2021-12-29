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
        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;
        static DLConfig()
        {
            XElement dalConfig = XElement.Load(@"config.xml");
            DalName = dalConfig.Element("dl").Value;
            DalPackages = (from pkg in dalConfig.Element("dl-packages").Elements()
                           select pkg
            ).ToDictionary(p => "" + p.Name, p => p.Value);
        }
    }
      
}

/*
static class DLConfig
{

    internal static string DLName;/*
        public string PkgName;
        public string NameSpace;
        public string ClassName;



    internal static string DLName;*/
/*
    internal static Dictionary<string, string> DLPackages;
    static DLConfig()
    {
        XElement dlConfig = XElement.Load(@"config.xml");
        DLName = dlConfig.Element("dl").Value;
        DLPackages = (from pkg in dlConfig.Element("dl-packages").Elements()
                      select pkg).ToDictionary(p => p.Name.ToString(), p => p.Value);

        /*
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
        }).ToDictionary(p => "" + p.Name, p => p);*/




