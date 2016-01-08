# Dynamics.ExtendedSvcUtil

Extension of the CrmSvcUtil to allow more custom creation of early bound types

## Synopsis

This tool extends the CrmSvcUtil to provide the following features:

* Individual class files for entities
* Individual class files for option sets or group option set files by associated entity 
* Ability to choose what entities you want to export
* Only creates option sets for the entities you are exporting
* Apply friendly names to entites, attributes and option sets
* Run ad-hoc and include in a build process 

It utilises the CrmSvcUtil to achieve this so this is a requirement of this project. The version of the CrmSvcUtil used at time of writing this was the version included with CRM 2016. But the DLL can be rebuilt to work with older versions if required.

## How to use

I will provided binaries as soon as I am happy we have a stable version. For now you must take a copy of the source locally and build it.

To configure to suit your specific needs within the application config you will see a sample schemaDefinition section. This has 2 sub sections for entities and option sets that allow you to specify the configuration for each. For example:

```
  <schemaDefinition entitiesFolder="..\EntityFiles" enumsFolder="..\EntityFiles" groupOptionSetsByEntity="true">
    <entities>
      <!-- Attributes="all/specified" -->
      <entity name="adx_conference" friendlyName="Conference">
        <attributes>
          <attribute name="adx_name" friendlyName="Name" />
        </attributes>
      </entity>
      <entity name="adx_event" friendlyName="Meeting">
        <attributes>
          <attribute name="icm_arrangedby" friendlyName="ArrangedBy" />
        </attributes>
      </entity>
    </entities>
    <optionSets>
      <optionSet name="adx_event_adx_eventtype" friendlyName="Meeting_MeetingType" entity="adx_event" />
      <optionSet name="icm_adx_event_icm_arrangedby" friendlyName="Meeting_ArrangedBy" entity="adx_event" />
    </optionSets>
  </schemaDefinition>
  ```

The main points to note:

* If an entity appears in the list it will be exported to a cs file.
* All option sets used by an entity in the list will be exported
* If you choose to utilise groupOptionSetsByEntity all option sets will be grouped into an entity file called "Entity.Enums.cs"
* If you exclude groupOptionSetsByEntity then all option sets will be exported to individual cs files 
* Use the friendlyName attribute to rename your entity/attribute/optionset
* 1 large overall file is still exported as specified in the out parameter of CrmSvcUtil. 

A sample command line use is:

`CrmSvcUtil /url:http://[url]/[org]/XrmServices/2011/Organization.svc /out:sdk.cs  /namespace:Namespace.Crm.Models /codewriterfilter:CodeGenerator.CodeWriterFilter,CodeGenerator  /namingservice:CodeGenerator.NamingService,CodeGenerator /codecustomization:CodeGenerator.CodeCustomizationService,CodeGenerator`

## License

MIT Licence

