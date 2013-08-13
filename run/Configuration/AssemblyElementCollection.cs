using System.Configuration;

namespace run.Configuration
{
    [ConfigurationCollection(typeof(AssemblyElement), AddItemName = "assembly",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class AssemblyElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyElement)element).Dll;
        }
    }
}