using Aurora.Disktop.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Common
{
    public class PackageManager
    {
        private Dictionary<String, ResourcePackage> keyValuePairs = new Dictionary<String, ResourcePackage>();


        public ResourcePackage? this[String packageName]
        {
            get
            {
                return keyValuePairs[packageName];
            }
        }

        public void Register(String packageName, ResourcePackage package)
        {
            keyValuePairs.Add(packageName, package);
        }






    }
}
