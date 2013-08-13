using System.Configuration;

namespace run.Configuration
{
    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("dll", IsKey = true, IsRequired = true)]
        public string Dll
        {
            get { return (string)base["dll"]; }
            set { base["dll"] = value; }
        }

        [ConfigurationProperty("interface", IsRequired = true)]
        public string Interface
        {
            get { return (string)base["interface"]; }
            set { base["interface"] = value; }
        }

        [ConfigurationProperty("category", IsRequired = true)]
        public string Category
        {
            get { return (string)base["category"]; }
            set { base["category"] = value; }
        }
    }
}