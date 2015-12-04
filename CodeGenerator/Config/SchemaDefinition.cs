using System.Configuration;

namespace CodeGenerator.Config
{
    public class SchemaDefinition : ConfigurationSection
    {
        [ConfigurationProperty("entities")]
        public EntitySchemaCollection Entities
        {
            get { return base["entities"] as EntitySchemaCollection; }
        }

        [ConfigurationProperty("optionSets")]
        public OptionSetSchemaCollection OptionSets
        {
            get { return base["optionSets"] as OptionSetSchemaCollection; }
        }
    }
}