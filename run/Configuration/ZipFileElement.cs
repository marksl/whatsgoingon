using System.Configuration;

namespace run.Configuration
{
    public class ZipFileElement : ConfigurationElement
    {
        [ConfigurationProperty("file", IsKey = true, IsRequired = true)]
        public string File
        {
            get { return (string)base["file"]; }
            set { base["file"] = value; }
        }


        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ZipFolderCollection Folders
        {
            get { return (ZipFolderCollection)this[""]; }
            set { this[""] = value; }
        }
    }
}