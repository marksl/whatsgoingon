using System.Configuration;

namespace run.Configuration
{
    [ConfigurationCollection(typeof(ZipFolderElement), AddItemName = "zipFolder",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ZipFolderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ZipFolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ZipFolderElement) element).Path;
        }
    }
}