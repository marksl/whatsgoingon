using System.Configuration;

namespace run.Configuration
{
    public class JiraGitSection :ConfigurationSection
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

        [ConfigurationProperty("prefix", IsRequired = true)]
        public string Prefix
        {
            get
            {
                return (string)this["prefix"];
            }
            set
            {
                this["prefix"] = value;
            }
        }
    }
}