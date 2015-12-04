using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CodeGenerator.Config
{
    [ConfigurationCollection(typeof(AttributeSchema), AddItemName = "attribute")]
    public class AttributeSchemaCollection : ConfigurationElementCollection, IEnumerable<AttributeSchema>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new AttributeSchema();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var configElement = element as AttributeSchema;
            if (configElement != null)
                return configElement.Name;
            return null;
        }

        public AttributeSchema this[int index]
        {
            get
            {
                return BaseGet(index) as AttributeSchema;
            }
        }

        IEnumerator<AttributeSchema> IEnumerable<AttributeSchema>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, Count)
                select this[i])
                .GetEnumerator();
        }
    }
}