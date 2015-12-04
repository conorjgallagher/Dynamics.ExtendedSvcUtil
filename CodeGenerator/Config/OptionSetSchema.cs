using System;
using System.Configuration;

namespace CodeGenerator.Config
{
    public class OptionSetSchema : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("friendlyName", IsRequired = true)]
        public String FriendlyName
        {
            get { return (String)this["friendlyName"]; }
            set { this["friendlyName"] = value; }
        }


        [ConfigurationProperty("entity", IsRequired = true)]
        public String Entity
        {
            get { return (String)this["entity"]; }
            set { this["entity"] = value; }
        } 
    }
}