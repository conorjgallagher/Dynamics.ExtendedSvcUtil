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
                                AttributeSchema attribute = GetSchemaAttribute(entity, member.Name);
                                AttributeMetadata attributeMetadata = entity.AttributeMetadata.ContainsKey(member.Name) ? entity.AttributeMetadata[member.Name] : null;
                                //Check for whitelisted CRM atributes to attach the c# attributes
                                //AttachAttributes(member, attribute);
                                //Add attribute desc to the property summary for docs
                                //AttachComments(member, attribute);
                                //Check for option sets
                                TransformOptionSets(member, entity, attribute, attributeMetadata);
                                //if (!optionSets.ContainsKey(type.Name.ToLower()))
                                //{
                                //    optionSets[type.Name.ToLower()] = new List<CodeTypeDeclarationCollection>();
                                //}
                            }
                        }
                        ExportClassCodeFile(type, codeUnit.Namespaces[i].Name);
                    }
                    if (type.IsEnum)
                    {

                        //if (!Schema.IncludedOptionSets.Contains(type.Name)) continue;
                        // Should we check the parent entity to see if we should export this? Or just let all through?
                        //if (GetSchemaOptionSet(type.Name) == null) continue;
                    }
                    //if (type.IsEnum)
                    //{
                    //    OptionSetSchema optionSet = GetSchemaOptionSet(type.Name);
                    //    foreach (CodeTypeMember member in type.Members)
                    //    {
                    //        SchemaOption option = GetSchemaOption(optionSet, member.Name);
                    //        if (member is CodeMemberField)
                    //        {
                    //            AttachAttributes(member, option);
                    //        }
                    //    }

                    //}


                    //Export the code file
                    //ExportCodeFile(type, codeUnit.Namespaces[i].Name);

                }

                // Now that we know what optionsets need to be included create the adequate files for them
                for (var j = 0; j < types.Count; j++)
                {
                    var type = types[j];

                    if (type.IsEnum && Schema.IncludedOptionSets.ContainsKey(type.Name))
                    {
                        var optionSet = Schema.IncludedOptionSets[type.Name];
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

            //ExportSchemas();
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

        //private static void ExportSchemas()
        //{
        //    //Output the Entities schema
        //    using (TextWriter output = new StreamWriter(string.Format("CRM Entities.schema"), false))
        //    {
        //        foreach (EntitySchema entity in Schema.Entities.Values)
        //        {
        //            output.WriteLine("----------------------");
        //            output.WriteLine(entity);
        //            output.WriteLine("----------------------");
        //            foreach (AttributeSchema attribute in entity.Attributes)
        //            {
        //                output.WriteLine(attribute);
        //            }
        //        }
        //    }

        //    //Output the Option Set schema
        //    using (TextWriter output = new StreamWriter(string.Format("CRM OptionSets.schema"), false))
        //    {
        //        foreach (var optionSet in Schema.OptionSets.Values)
        //        {
        //            output.WriteLine("----------------------");
        //            output.WriteLine(optionSet);
        //            output.WriteLine("----------------------");
        //            foreach (var option in optionSet.)
        //            {
        //                output.WriteLine(option);
        //            }
        //        }
        //    }
        //}



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
            else
            {
                //Console.Error.WriteLine("SchemaStorage: No entity named '{0}'", name.ToLower());
            }
            return entity;
        }
        private static AttributeSchema GetSchemaAttribute(EntitySchema entity, string att)
        {
            AttributeSchema attribute = null;
            if (entity != null && entity.Attributes.FirstOrDefault(a => a.Name == att.ToLower()) != null)
            {
                attribute = entity.Attributes.FirstOrDefault(a => a.Name == att.ToLower());
            }
            else if (entity != null && entity.Attributes.FirstOrDefault(a => a.FriendlyName == att.ToLower()) != null)
            {
                attribute = entity.Attributes.FirstOrDefault(a => a.FriendlyName == att.ToLower());
            }
            else
            {
                //Console.Error.WriteLine("\tNo attribute named '{0}.{1}'", (entity != null ? entity.Name : ""), att.ToLower());
            }
            return attribute;
        }

        private static OptionSetSchema GetSchemaOptionSet(string name)
        {
            OptionSetSchema entity = null;
            if (Schema.OptionSets.ContainsKey(name.ToLower()))
            {
                entity = Schema.OptionSets[name.ToLower()];
            }
            else if (Schema.OptionSets.Values.FirstOrDefault(e => e.FriendlyName == name) != null)
            {
                return Schema.OptionSets.Values.FirstOrDefault(e => e.FriendlyName == name);
            }
            else
            {
                //Console.Error.WriteLine("SchemaStorage: No option set named '{0}'", name);
            }
            return entity;
        }

        //private static SchemaOption GetSchemaOption(SchemaOptionSet otionSet, string opt)
        //{
        //    SchemaOption option = null;
        //    if (otionSet != null && otionSet.Options.ContainsKey(opt))
        //    {
        //        option = otionSet.Options[opt];
        //    }
        //    else
        //    {
        //        Console.Error.WriteLine("\tNo opion named '{0}.{1}'", (otionSet != null ? otionSet.Name : ""), opt);
        //    }
        //    return option;
        //}

        private static void TransformOptionSets(CodeTypeMember member, EntitySchema entity, AttributeSchema attribute, AttributeMetadata attributeMetadata)
        {
            var codeProperty = (CodeMemberProperty)member;
            if ((codeProperty.Type.BaseType != "Microsoft.Xrm.Sdk.OptionSetValue")) return;

            EnumAttributeMetadata picklistAttribute = null;
            OptionSetSchema optionSet = null;

            if (entity != null && (attribute != null || attributeMetadata != null))
            {
                //listAttribute = attribute as SchemaPickListAttribute;
                picklistAttribute = attributeMetadata as EnumAttributeMetadata;
                if (picklistAttribute != null) // && Schema.OptionSets.ContainsKey(picklistAttribute.OptionSet.Name.ToLower()))
                {
                    Schema.IncludedOptionSets[picklistAttribute.OptionSet.Name.ToLower()] = picklistAttribute;
                    optionSet = Schema.OptionSets[picklistAttribute.OptionSet.Name.ToLower()];
                }
                else
                {
                    //Console.Error.WriteLine("\tOption Set '{0}' could not be found for attribute '{1}.{2}'", member.Name, entity.Name, attribute.Name);
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

        //private static void AttachAttributes(CodeTypeMember member, AttributeSchema attribute)
        //{
        //    if (attribute == null) return;
        //    //if (attribute.WhiteList == null) return;

        //    //foreach (var property in attribute.WhiteList.Properties)
        //    //{
        //    //    member.CustomAttributes.Add(property); //TODO: Probably construct the attribute here rather than in the Schema structure load...?
        //    //}
        //}

        //private static void AttachAttributes(CodeTypeMember member, SchemaOption option)
        //{
        //    if (option == null) return;
        //    if (option.WhiteList == null)
        //    {
        //        //Add default CRM name to the enum
        //        var attribute = new CodeAttributeDeclaration { Name = "Description" };
        //        attribute.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(@"""" + option.CrmName + @"""")));
        //        member.CustomAttributes.Add(attribute);
        //        return;
        //    }

        //    foreach (var property in option.WhiteList.Properties)
        //    {
        //        member.CustomAttributes.Add(property); //TODO: Probably construct the attribute here rather than in the Schema structure load...?
        //    }

        //}

//        private static void AttachComments(CodeTypeMember member, AttributeSchema attribute)
//        {
//            if (attribute == null) return;
//            //if (attribute.WhiteList == null) return;

//            var comment = !string.IsNullOrWhiteSpace(attribute.Desc) ? attribute.Desc :
//                string.Format("No description for '{0}'", attribute);
//            if (member.Comments.Count > 0)
//            {
//                member.Comments.Clear();
//            }
//            member.Comments.Add(new CodeCommentStatement(string.Format(@"
//        <summary>
//        {0}
//        </summary>", comment)));

//        }

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
                    //codeProperty.SetStatements.Insert(0, new CodeSnippetStatement(
                    //    "\t\t\t\tif (!value.HasValue) return;"));
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

    //public sealed class CodeCustomizationService : ICustomizeCodeDomService
    //{
    //    /// <summary>
    //    /// Remove the unnecessary classes that we generated for entities. 
    //    /// </summary>
    //    public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
    //    {
    //        Trace.TraceInformation("Entering ICustomizeCodeDomService.CustomizeCodeDom");
    //        Trace.TraceInformation(
    //            "Number of Namespaces generated: {0}", codeUnit.Namespaces.Count);
    //        if (Schema.ExportType == ExportTypeEnum.OptionSet)
    //        {
    //            foreach (CodeAttributeDeclaration attribute in codeUnit.AssemblyCustomAttributes)
    //            {
    //                Trace.TraceInformation("Attribute BaseType is {0}",
    //                    attribute.AttributeType.BaseType);
    //                if (attribute.AttributeType.BaseType ==
    //                    "Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute")
    //                {
    //                    codeUnit.AssemblyCustomAttributes.Remove(attribute);
    //                    break;
    //                }
    //            }
    //        }

    //        // Iterate over all of the namespaces that were generated.
    //        for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
    //        {
    //            var types = codeUnit.Namespaces[i].Types;
    //            Trace.TraceInformation("Number of types in Namespace {0}: {1}",
    //                codeUnit.Namespaces[i].Name, types.Count);
    //            // Iterate over all of the types that were created in the namespace.
    //            for (var j = 0; j < types.Count;)
    //            {
    //                // Remove the type if it is not an enum (all OptionSets are enums) or has no members.
    //                if (!types[j].IsEnum || types[j].Members.Count == 0)
    //                {
    //                    if (Schema.ExportType == ExportTypeEnum.OptionSet)
    //                    {
    //                        types.RemoveAt(j);
    //                    }
    //                    else
    //                    {
    //                        j += 1;
    //                    }
    //                }
    //                else
    //                {
    //                    if (Schema.ExportType == ExportTypeEnum.Entity)
    //                    {
    //                        types.RemoveAt(j);
    //                    }
    //                    else
    //                    {
    //                        j += 1;
    //                    }
    //                }
    //            }
    //        }

    //        Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
    //    }
    //}
}