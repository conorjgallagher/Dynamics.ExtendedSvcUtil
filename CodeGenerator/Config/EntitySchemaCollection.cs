using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CodeGenerator.Config
{
    [ConfigurationCollection(typeof(EntitySchema), AddItemName = "entity")]
    public class EntitySchemaCollection : ConfigurationElementCollection, IEnumerable<EntitySchema>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EntitySchema();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var configElement = element as EntitySchema;
            if (configElement != null)
                return configElement.Name;
            return null;
        }

        public EntitySchema this[int index]
        {
            get
            {
                return BaseGet(index) as EntitySchema;
            }
        }

        IEnumerator<EntitySchema> IEnumerable<EntitySchema>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, Count)
                select this[i])
                .GetEnumerator();
        }
    }
}