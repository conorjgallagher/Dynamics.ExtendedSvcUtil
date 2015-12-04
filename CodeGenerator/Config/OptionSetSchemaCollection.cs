using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CodeGenerator.Config
{
    [ConfigurationCollection(typeof(OptionSetSchema), AddItemName = "optionSet")]
    public class OptionSetSchemaCollection : ConfigurationElementCollection, IEnumerable<OptionSetSchema>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new OptionSetSchema();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var configElement = element as OptionSetSchema;
            if (configElement != null)
                return configElement.Name;
            return null;
        }

        public OptionSetSchema this[int index]
        {
            get
            {
                return BaseGet(index) as OptionSetSchema;
            }
        }

        IEnumerator<OptionSetSchema> IEnumerable<OptionSetSchema>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, Count)
                select this[i])
                .GetEnumerator();
        }
    }
}