using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CodeGenerator.Config;
using Microsoft.Crm.Services.Utility;
using Microsoft.CSharp;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator
{
    public sealed class CodeCustomizationService : ICustomizeCodeDomService
    {
        /// <summary>
        /// Remove the unnecessary classes that we generated for entities. 
        /// </summary>
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {

            // Iterate over all of the namespaces that were generated.
            Directory.CreateDirectory(Schema.EnumsFolder);
            Directory.CreateDirectory(Schema.EntitiesFolder);


            var optionSets = new Dictionary<string, List<CodeTypeDeclaration>>();

            for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
            {
                var types = codeUnit.Namespaces[i].Types;

                // Export the classes first so that we know what optionsets we need
                for (var j = 0; j < types.Count; j++)
                {
                    var type = types[j];
                    if (type.IsClass)
                    {
                        EntitySchema entity = GetSchemaEntity(type.Name);
                        if (entity == null) continue;
                        foreach (CodeTypeMember member in type.Members)
                        {
                            if (member is CodeMemberProperty)
                            {
                                AttributeMetadata attributeMetadata = entity.AttributeMetadata.ContainsKey(member.Name) ? entity.AttributeMetadata[member.Name] : null;
                                TransformOptionSets(member, entity, attributeMetadata);

                            }
                        }
                        ExportClassCodeFile(type, codeUnit.Namespaces[i].Name);
                    }
                }

                // Now that we know what optionsets need to be included create the adequate files for them
                for (var j = 0; j < types.Count; j++)
                {
                    var type = types[j];

                    EnumAttributeMetadata optionSet = GetIncludedOptionSet(type.Name);
                    if (type.IsEnum && optionSet != null)
                    {
                        EntitySchema entity = GetSchemaEntity(optionSet.EntityLogicalName);
                        var entityGroup = entity.FriendlyName;
                        if (optionSet.OptionSet.IsGlobal.Value)
                        {
                            entityGroup = "Global";
                        }
                        if (!optionSets.ContainsKey(entityGroup))
                        {
                            optionSets[entityGroup] = new List<CodeTypeDeclaration>();
                        }
                        optionSets[entityGroup].Add(type);
                    }
                }
                foreach (var entityType in optionSets.Keys)
                {
                    if (Schema.GroupOptionSetsByEntity)
                    {
                        ExportOptionSetCodeFile(optionSets[entityType], codeUnit.Namespaces[i].Name,
                            string.Format("{0}.Enums.cs", entityType));
                    }
                    else
                    {
                        foreach (var type in optionSets[entityType])
                        {
                            ExportOptionSetCodeFile(new List<CodeTypeDeclaration> { type }, codeUnit.Namespaces[i].Name, string.Format("{0}.cs", type.Name));
                        }
                    }
                }
            }
        }

        private EnumAttributeMetadata GetIncludedOptionSet(string name)
        {
            if (Schema.IncludedOptionSets.ContainsKey(name.ToLower()))
            {
                return Schema.IncludedOptionSets[name.ToLower()];
            }
            if (Schema.OptionSets.Any(o => o.Value.FriendlyName == name))
            {
                var optionSetSchema = Schema.OptionSets.First(o => o.Value.FriendlyName == name);
                if (Schema.IncludedOptionSets.ContainsKey(optionSetSchema.Value.Name.ToLower()))
                {
                    return Schema.IncludedOptionSets[optionSetSchema.Value.Name.ToLower()];
                }
            }
            return null;
        }

        private void ExportOptionSetCodeFile(List<CodeTypeDeclaration> types, string namespaceName, string filename = null)
        {
            var csharp = new CSharpCodeProvider();
            var namespaces = new CodeNamespace() { Name = namespaceName };
            namespaces.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            foreach (var type in types)
            {
                namespaces.Types.Add(type);
            }
            using (TextWriter output = new StreamWriter(string.Format("{0}\\{1}", Schema.EnumsFolder, filename), false))
            {
                csharp.GenerateCodeFromNamespace(namespaces, output, new CodeGeneratorOptions());
            }
        }

        private void ExportClassCodeFile(CodeTypeDeclaration type, string namespaceName)
        {
            var csharp = new CSharpCodeProvider();
            var namespaces = new CodeNamespace() { Name = namespaceName };
            using (TextWriter output = new StreamWriter(string.Format("{0}\\{1}.cs", Schema.EntitiesFolder, type.Name), false))
            {
                namespaces.Types.Add(type);
                csharp.GenerateCodeFromNamespace(namespaces, output, new CodeGeneratorOptions());
            }
        }

        private static EntitySchema GetSchemaEntity(string name)
        {
            EntitySchema entity = null;
            if (Schema.Entities.ContainsKey(name.ToLower()))
            {
                entity = Schema.Entities[name.ToLower()];
            }
            else if(Schema.Entities.Values.FirstOrDefault(e => e.FriendlyName == name) != null)
            {
                return Schema.Entities.Values.FirstOrDefault(e => e.FriendlyName == name);
            }
            return entity;
        }


        private static void TransformOptionSets(CodeTypeMember member, EntitySchema entity, AttributeMetadata attributeMetadata)
        {
            var codeProperty = (CodeMemberProperty)member;
            if ((codeProperty.Type.BaseType != "Microsoft.Xrm.Sdk.OptionSetValue")) return;

            EnumAttributeMetadata picklistAttribute = null;
            OptionSetSchema optionSet = null;

            if (entity != null && attributeMetadata != null)
            {
                picklistAttribute = attributeMetadata as EnumAttributeMetadata;
                if (picklistAttribute != null)
                {
                    Schema.IncludedOptionSets[picklistAttribute.OptionSet.Name.ToLower()] = picklistAttribute;
                    optionSet = Schema.OptionSets[picklistAttribute.OptionSet.Name.ToLower()];
                }
            }


            if (optionSet != null)
            {
                FixEnums(codeProperty, picklistAttribute, optionSet);
            }
            else
            {
                codeProperty.Type = new CodeTypeReference("int?");
            }
        }

        private static void FixEnums(CodeMemberProperty codeProperty, EnumAttributeMetadata listAttribute, OptionSetSchema optionSet)
        {
            codeProperty.Type = new CodeTypeReference(optionSet.FriendlyName + "?");
            if (codeProperty.HasSet)
            {
                if (codeProperty.SetStatements[1].GetType() == typeof(CodeConditionStatement))
                {
                    ((CodeConditionStatement)codeProperty.SetStatements[1]).FalseStatements[0] = new CodeSnippetStatement
                    {
                        Value =
                            String.Format(
                                "\t\t\t\tthis.SetAttributeValue(\"{0}\", (value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value) : null));",
                                listAttribute.LogicalName)
                    };
                }
                else
                {
                    codeProperty.SetStatements[1] =
                        new CodeSnippetStatement(
                            String.Format(
                                "\t\t\t\t\tthis.SetAttributeValue(\"{0}\", (value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value) : null));",
                                listAttribute.LogicalName));
                }
            }

            if (codeProperty.HasGet)
            {
                codeProperty.GetStatements.Clear();
                codeProperty.GetStatements.Add(new CodeSnippetExpression(
                    string.Format(
                        "var ret = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(\"{1}\");" + Environment.NewLine +
                        "\t\t\t\treturn (ret!=null ? ({0}?)ret.Value : ({0}?)null);",
                        optionSet.Name, listAttribute.LogicalName)
                    ));
            }
        }

    }

}