using System.Configuration;

namespace Neo4jRestNet.Configuration
{
	
	[ConfigurationCollection(typeof(ConnectionElement))]
	public class ConnectionCollection : ConfigurationElementCollection
{
    protected override ConfigurationElement CreateNewElement()
    {
		return new ConnectionElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
		return ((ConnectionElement)element).Name;
    }
}
}