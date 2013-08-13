using System.Configuration;

namespace run.Configuration
{
    [ConfigurationCollection(typeof(ZipFileElement), AddItemName = "zipFile", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ZipFileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ZipFileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ZipFileElement)element).File;
        }
    }
}