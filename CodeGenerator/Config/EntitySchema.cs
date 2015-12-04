using System;
using System.Configuration;

namespace CodeGenerator.Config
{
    public class EntitySchema : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public String Name
        {
            get { return (String) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("friendlyName", IsRequired = true)]
        public String FriendlyName
        {
            get { return (String) this["friendlyName"]; }
            set { this["friendlyName"] = value; }
        }

        [ConfigurationProperty("includeAttributes", IsRequired = false, DefaultValue = "all")]
        public String IncludeAttributes
        {
            get { return (String) this["includeAttributes"]; }
            set { this["includeAttributes"] = value; }
        }

        [ConfigurationProperty("attributes", IsRequired = false)]
        public AttributeSchemaCollection Attributes
        {
            get { return base["attributes"] as AttributeSchemaCollection; }
        }
    }
}