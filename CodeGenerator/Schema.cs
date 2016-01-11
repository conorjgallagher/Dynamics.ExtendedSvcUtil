using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using CodeGenerator.Config;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator
{
    public static class Schema
    {
        private static IDictionary<string, EntitySchema> _validEntities;
        private static IDictionary<string, OptionSetSchema> _transformOptionSets;
        private static Dictionary<string, EnumAttributeMetadata> _includedOptionSets;

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
                if (_transformOptionSets == null)
                {
                    LoadConfig();
                }
                return _transformOptionSets;
            }
        }

        public static IDictionary<string, EnumAttributeMetadata> IncludedOptionSets
        {
            get { return _includedOptionSets ?? (_includedOptionSets = new Dictionary<string, EnumAttributeMetadata>()); }
        }
        public static string EnumsFolder { get; set; }
        public static string EntitiesFolder { get; set; }
        public static string CurrentOptionSet { get; set; }
        public static string CurrentEntity { get; set; }
        public static ExportTypeEnum ExportType { get; set; }
        public static bool GroupOptionSetsByEntity { get; set; }
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

            _transformOptionSets = new ConcurrentDictionary<string, OptionSetSchema>();
            foreach (OptionSetSchema optionSet in schemaDefinition.OptionSets)
            {
                _transformOptionSets.Add(optionSet.Name, optionSet);
            }

            EntitiesFolder = schemaDefinition.EntitiesFolder;
            EnumsFolder = schemaDefinition.EnumsFolder;
            GroupOptionSetsByEntity = schemaDefinition.GroupOptionSetsByEntity;
        }

    }

    public enum ExportTypeEnum
    {
        Entity,
        OptionSet
    }
}