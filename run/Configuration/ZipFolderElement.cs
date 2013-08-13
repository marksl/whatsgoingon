using System.Configuration;

namespace run.Configuration
{
    public class ZipFolderElement :ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public AssemblyElementCollection Assemblies
        {
            get { return (AssemblyElementCollection)this[""]; }
            set { this[""] = value; }
        }
    }
}