﻿using System;
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
            if (!Schema.OptionSets.ContainsKey(optionSetMetadata.Name.ToLower()))
            {
                Schema.OptionSets[optionSetMetadata.Name.ToLower()] = new OptionSetSchema
                {
                    Name = optionSetMetadata.Name,
                    FriendlyName = optionSetMetadata.Name
                };
            }
            return true;
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateEntity(entityMetadata, services);
        }

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
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
            return false;
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return _defaultService.GenerateServiceContext(services);
        }

    }
}
