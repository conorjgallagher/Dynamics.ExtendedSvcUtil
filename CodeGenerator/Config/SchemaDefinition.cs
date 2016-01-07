using System;
using System.Configuration;

namespace CodeGenerator.Config
{
    public class SchemaDefinition : ConfigurationSection
    {
        [ConfigurationProperty("entitiesFolder", IsRequired = false, DefaultValue = "Entities")]
        public String EntitiesFolder
        {
            get { return (String)this["entitiesFolder"]; }
            set { this["entitiesFolder"] = value; }
        }

        [ConfigurationProperty("groupOptionSetsByEntity", IsRequired = false, DefaultValue = false)]
        public bool GroupOptionSetsByEntity
        {
            get { return (bool)this["groupOptionSetsByEntity"]; }
            set { this["groupOptionSetsByEntity"] = value; }
        }

        [ConfigurationProperty("enumsFolder", IsRequired = false, DefaultValue = "Enums")]
        public String EnumsFolder
        {
            get { return (String)this["enumsFolder"]; }
            set { this["enumsFolder"] = value; }
        }

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