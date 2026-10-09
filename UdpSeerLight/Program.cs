using Hspi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HSPI_UdpSeerLight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetAssemblyNameAndVersion());
            Console.WriteLine();
            Connector.Connect<HSPI>(args);
        }

        private static string GetAssemblyNameAndVersion()
        {
            Assembly exeAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemName = exeAssembly.GetName();
            var ver = assemName.Version;
            return assemName.Name + ", Version " + ver.ToString();
        }
    }
}
