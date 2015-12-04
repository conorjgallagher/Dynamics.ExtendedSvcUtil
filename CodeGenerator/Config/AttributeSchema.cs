using System;
using System.Configuration;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator.Config
{
    public class AttributeSchema : ConfigurationElement
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

        public AttributeMetadata Metadata { get; set; }
    }
}