using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CodeGenerator.Config;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator
{
    /// <summary>
    /// Implement this class if you want to provide your own logic for building names that
    /// will appear in the generated code.
    /// </summary>
    public sealed class NamingService : INamingService
    {
        private readonly INamingService _defaultNamingService;

        /// <summary>
        /// This field keeps track of the number of times that options with the same
        /// name would have been defined.
        /// </summary>
        private readonly Dictionary<OptionSetMetadataBase, Dictionary<String, int>> _optionNames;

        public NamingService(INamingService namingService)
        {
            _defaultNamingService = namingService;
            _optionNames = new Dictionary<OptionSetMetadataBase,
                Dictionary<String, int>>();
        }

        public string GetNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata,
            IServiceProvider services)
        {
            var optionSetName = optionSetMetadata.Name.ToLower();
            if (Schema.OptionSets.ContainsKey(optionSetName))
            {
                return Schema.OptionSets[optionSetName].FriendlyName;
            }

            return _defaultNamingService.GetNameForOptionSet(
                entityMetadata, optionSetMetadata, services);
        }

        public string GetNameForOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata,
            IServiceProvider services)
        {
            var name = _defaultNamingService.GetNameForOption(optionSetMetadata, optionMetadata, services);
            name = EnsureValidIdentifier(name);
            name = EnsureUniqueOptionName(optionSetMetadata, name);
            return name;
        }

        public string GetNameForEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            if (Schema.Entities.ContainsKey(entityMetadata.LogicalName))
            {
                return Schema.Entities[entityMetadata.LogicalName].FriendlyName;
            }
            return _defaultNamingService.GetNameForEntity(entityMetadata, services);
        }

        public string GetNameForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            if (Schema.Entities.ContainsKey(entityMetadata.LogicalName))
            {
                var attribute =
                    Schema.Entities[entityMetadata.LogicalName].Attributes.FirstOrDefault(
                        a => a.Name == attributeMetadata.LogicalName);
                if (attribute != null)
                {
                    attribute.Metadata = attributeMetadata;
                    return attribute.FriendlyName;
                }
            }
            return _defaultNamingService.GetNameForAttribute(entityMetadata, attributeMetadata, services);
        }

        public string GetNameForRelationship(EntityMetadata entityMetadata, RelationshipMetadataBase relationshipMetadata,
            EntityRole? reflexiveRole, IServiceProvider services)
        {
            return _defaultNamingService.GetNameForRelationship(entityMetadata, relationshipMetadata, reflexiveRole,
                services);
        }

        public string GetNameForServiceContext(IServiceProvider services)
        {
            return _defaultNamingService.GetNameForServiceContext(services);
        }

        public string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return GetNameForEntity(entityMetadata, services) + "Set";
        }

        public string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
        {
            return _defaultNamingService.GetNameForMessagePair(messagePair, services);
        }

        public string GetNameForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            return _defaultNamingService.GetNameForRequestField(request, requestField, services);
        }

        public string GetNameForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField,
            IServiceProvider services)
        {
            return _defaultNamingService.GetNameForResponseField(response, responseField, services);
        }


        /// <summary>
        /// Checks to make sure that the name begins with a valid character. If the name
        /// does not begin with a valid character, then add an underscore to the
        /// beginning of the name.
        /// </summary>
        private static String EnsureValidIdentifier(String name)
        {
            // Check to make sure that the option set begins with a word character
            // or underscore.
            const string pattern = @"^[A-Za-z_][A-Za-z0-9_]*$";
            if (!Regex.IsMatch(name, pattern))
            {
                // Prepend an underscore to the name if it is not valid.
                name = String.Format("_{0}", name);
                Trace.TraceInformation("Name of the option changed to {0}", name);
            }
            return name;
        }

        /// <summary>
        /// Checks to make sure that the name does not already exist for the OptionSet
        /// to be generated.
        /// </summary>
        private String EnsureUniqueOptionName(OptionSetMetadataBase metadata, String name)
        {
            if (_optionNames.ContainsKey(metadata))
            {
                if (_optionNames[metadata].ContainsKey(name))
                {
                    // Increment the number of times that an option with this name has
                    // been found.
                    ++_optionNames[metadata][name];

                    // Append the number to the name to create a new, unique name.
                    var newName = String.Format("{0}_{1}",
                        name, _optionNames[metadata][name]);

                    Trace.TraceInformation("The {0} OptionSet already contained a definition for {1}. Changed to {2}", metadata.Name, name, newName);

                    // Call this function again to make sure that our new name is unique.
                    return EnsureUniqueOptionName(metadata, newName);
                }
            }
            else
            {
                // This is the first time this OptionSet has been encountered. Add it to
                // the dictionary.
                _optionNames[metadata] = new Dictionary<string, int>();
            }

            // This is the first time this name has been encountered. Begin keeping track
            // of the times we've run across it.
            _optionNames[metadata][name] = 1;

            return name;
        }

    }
}