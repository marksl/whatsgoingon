using System.Configuration;

namespace run.Configuration
{
    public class ArtifactsSection : ConfigurationSection
    {
        [ConfigurationProperty("dir", IsRequired = true)]
        public string Directory
        {
            get
            {
                return (string)this["dir"];
            }
            set
            {
                this["dir"] = value;
            }
        }

        [ConfigurationProperty("zipFiles", IsRequired = true, IsDefaultCollection = true)]
        public ZipFileCollection ZipFiles
        {
            get { return (ZipFileCollection)this["zipFiles"]; }
            set { this["zipFiles"] = value; }
        }

        [ConfigurationProperty("zipFile", IsRequired = false)]
        public string ZipFile
        {
            get
            {
                return (string)this["zipFile"];
            }
            set
            {
                this["zipFile"] = value;
            }
        }

        [ConfigurationProperty("dllInZipFile", IsRequired = false)]
        public string DllInZipFile
        {
            get
            {
                return (string)this["dllInZipFile"];
            }
            set
            {
                this["dllInZipFile"] = value;
            }
        }
    }
}