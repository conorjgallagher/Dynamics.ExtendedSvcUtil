using System;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;
using CodeGenerator.Config;

namespace CodeGenerator
{
    public class CodeWriterFilter : ICodeWriterFilterService
    {
        private Dictionary<String, bool> GeneratedOptionSets { get; set; }

        //reference to the default service.
        private readonly ICodeWriterFilterService _defaultService;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="defaultService">default implementation</param>
        public CodeWriterFilter(ICodeWriterFilterService defaultService)
        {
            _defaultService = defaultService;
            GeneratedOptionSets = new Dictionary<String, bool>();
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            Schema.OptionSets[optionSetMetadata.Name.ToLower()] = new OptionSetSchema
            {
                Name = optionSetMetadata.Name,
                FriendlyName = optionSetMetadata.Name
            };

            //if (!optionSetMetadata.IsGlobal.HasValue || !optionSetMetadata.IsGlobal.Value) return true;
            //if (GeneratedOptionSets.ContainsKey(optionSetMetadata.Name)) return false;
            //GeneratedOptionSets[optionSetMetadata.Name] = true;

            //if (Schema.OptionSets.ContainsKey(optionSetMetadata.Name.ToLower()))
            //{
                //Schema.OptionSets[optionSetMetadata.Name.ToLower()] = new OptionSetSchema
                //{
                //    Name = optionSetMetadata.Name,
                //    FriendlyName = optionSetMetadata.Name
                //};
            //}
            return true;
            //return _defaultService.GenerateOptionSet(optionSetMetadata, services);
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            // Hack to get the service exporting an entity at a time. Loop through when writing 
            // and set a current entity. Check for the entity here and only allow that one through
            //if (Schema.ExportType == ExportTypeEnum.OptionSet)
            //{
            //    return true;
            //}
            //return Schema.CurrentEntity == entityMetadata.LogicalName;
            return _defaultService.GenerateEntity(entityMetadata, services);
        }

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            /* 
             * Bug in CrmSvcUtil?
             * At present this code seems to cause problems with how the code is generated
             * Not sure if it's an issue with CrmSvcUtil. Just export all attributes for now 
            EntitySchema entity = Schema.Entities[attributeMetadata.EntityLogicalName];
            if (entity.IncludeAttributes.ToLower() == "all")
                return true;
            return entity.Attributes.FirstOrDefault(a => a.Name == attributeMetadata.LogicalName) != null;
             */
            if (Schema.Entities.ContainsKey(attributeMetadata.EntityLogicalName))
            {
                EntitySchema entity = Schema.Entities[attributeMetadata.EntityLogicalName];
                entity.AttributeMetadata[attributeMetadata.SchemaName] = attributeMetadata;
            }
            return _defaultService.GenerateAttribute(attributeMetadata, services);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata,
            EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            //return _defaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
            return false;
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return _defaultService.GenerateServiceContext(services);
            //return false;
        }

    }
}
