using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator.Config
{
    public class EntitySchema : ConfigurationElement
    {
        private IDictionary<string, AttributeMetadata> _attributeMetadata;
        private IDictionary<string, string> _attributeNamesExport;

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

        public IDictionary<string, AttributeMetadata> AttributeMetadata
        {
            get { return _attributeMetadata ?? (_attributeMetadata = new Dictionary<string, AttributeMetadata>()); }
            set { _attributeMetadata = value; }
        }

        public IDictionary<string, string> AttributeNamesExport
        {
            get { return _attributeNamesExport ?? (_attributeNamesExport = new Dictionary<string, string>()); }
        }
    }
}