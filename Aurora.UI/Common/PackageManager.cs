

namespace Aurora.UI.Common
{
    public class PackageManager
    {
        private Dictionary<String, ResourcePackage> keyValuePairs = new Dictionary<String, ResourcePackage>();


        public ResourcePackage this[String packageName]
        {
            get
            {
                if (keyValuePairs.ContainsKey(packageName))
                {
                    return keyValuePairs[packageName];
                }
                return null;
            }
        }

        public void Register(String packageName, ResourcePackage package)
        {
            keyValuePairs.Add(packageName, package);
        }
    }
}
