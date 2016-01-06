using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using CodeGenerator.Config;

namespace CodeGenerator
{
    public static class Schema
    {
        private static IDictionary<string, EntitySchema> _validEntities;
        private static IDictionary<string, OptionSetSchema> _validOptionSets;
        private static IList<string> _includedOptionSets;

        public static IDictionary<string, EntitySchema> Entities
        {
            get
            {
                if (_validEntities == null)
                {
                    LoadConfig();
                }
                return _validEntities;
            }
        }
        public static IDictionary<string, OptionSetSchema> OptionSets
        {
            get
            {
                if (_validOptionSets == null)
                {
                    LoadConfig();
                }
                return _validOptionSets;
            }
        }

        public static IList<string> IncludedOptionSets
        {
            get { return _includedOptionSets ?? (_includedOptionSets = new List<string>()); }
        }

        public static string CurrentOptionSet { get; set; }
        public static string CurrentEntity { get; set; }
        public static ExportTypeEnum ExportType { get; set; }

        private static void LoadConfig()
        {
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = "CodeGenerator.dll.config"
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var schemaDefinition = (SchemaDefinition)config.GetSection("schemaDefinition");

            _validEntities = new Dictionary<string, EntitySchema>();
            foreach (EntitySchema entity in schemaDefinition.Entities)
            {
                _validEntities.Add(entity.Name, entity);
            }

            _validOptionSets = new ConcurrentDictionary<string, OptionSetSchema>();
            foreach (OptionSetSchema optionSet in schemaDefinition.OptionSets)
            {
                _validOptionSets.Add(optionSet.Name, optionSet);
            }
        }
    }

    public enum ExportTypeEnum
    {
        Entity,
        OptionSet
    }
}