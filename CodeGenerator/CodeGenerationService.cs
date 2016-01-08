using System;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace CodeGenerator
{
    //public class CodeGenerationService : ICodeGenerationService
    //{
    //    private readonly ICodeGenerationService _defaultCodeGenerationService;

    //    public CodeGenerationService(ICodeGenerationService codeGenerationService)
    //    {
    //        _defaultCodeGenerationService = codeGenerationService;
    //    }

    //    public void Write(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
    //    {
    //        _defaultCodeGenerationService.Write(organizationMetadata, language, outputFile, targetNamespace, services);
    //        //// Hack to get the service exporting to separate file. Loop through when writing 
    //        //// and set a current type. This field will be checked and only that allowed through
    //        //foreach (var entitySchema in Schema.Entities.Values)
    //        //{
    //        //    Schema.CurrentEntity = entitySchema.Name;
    //        //    Schema.ExportType = ExportTypeEnum.Entity;

    //        //    _defaultCodeGenerationService.Write(organizationMetadata, language, entitySchema.FriendlyName + ".cs", targetNamespace,
    //        //        services);
    //        //}

    //        //foreach (var optionSetSchema in Schema.OptionSets.Values)
    //        //{
    //        //    Schema.CurrentEntity = optionSetSchema.Entity;
    //        //    Schema.CurrentOptionSet = optionSetSchema.Name;
    //        //    Schema.ExportType = ExportTypeEnum.OptionSet;
    //        //    _defaultCodeGenerationService.Write(organizationMetadata, language, optionSetSchema.FriendlyName + ".cs", targetNamespace,
    //        //        services);
    //        //}
    //    }

    //    public CodeGenerationType GetTypeForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata,
    //        IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForOptionSet(entityMetadata, optionSetMetadata, services);
    //    }

    //    public CodeGenerationType GetTypeForOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata,
    //        IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForOption(optionSetMetadata, optionMetadata, services);
    //    }

    //    public CodeGenerationType GetTypeForEntity(EntityMetadata entityMetadata, IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForEntity(entityMetadata, services);
    //    }

    //    public CodeGenerationType GetTypeForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata,
    //        IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForAttribute(entityMetadata, attributeMetadata, services);
    //    }

    //    public CodeGenerationType GetTypeForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForMessagePair(messagePair, services);
    //    }

    //    public CodeGenerationType GetTypeForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField,
    //        IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForRequestField(request, requestField, services);
    //    }

    //    public CodeGenerationType GetTypeForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField,
    //        IServiceProvider services)
    //    {
    //        return _defaultCodeGenerationService.GetTypeForResponseField(response, responseField, services);
    //    }

    //}
}