# Dynamics.ExtendedSvcUtil

Extension of the CrmSvcUtil to allow more custom creation of early bound types

## Synopsis

After reviewing many other tools all lacked a few of the custom requirements we had in a project. We had the following technical requirements:

* Individual cs files for entities and option set
* Ability to choose what entities you want to export
* Ability to choose what option sets you want to export
* Apply friendly names to both entites and attributes
* Run ad-hoc and include in a build process. 

Unfortunately no projects out there met all these requirements so this project was born. Utilising the CrmSvcUtil looked like a good option so this is a requirement of this project. I could not locate CrmSvcUtil on nuget so I have included the CRM 2013 version in this project.

## How to use

I will provided binaries as soon as I am happy we have a stable version. For now you must take a copy of the source locally and build it.

To configure to suit your specific needs within the application config you will see a sample schemaDefinition section. This has 2 sub sections for entities and option sets that allow you to specify the configuration for each. For example:

```
  <schemaDefinition>
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

* If an entity/optionset appears in the list it will be exported to it's own cs file.
* If it doesn't exist in the list then it will still be exported to the 1 big overall file (specified in the out parameter of CrmSvcUtil).
* Use friendlyName to rename your entity/attribute/optionset

A sample command line use is:

`CrmSvcUtil /url:http://[url]/[org]/XrmServices/2011/Organization.svc /out:sdk.cs  /namespace:Namespace.Crm.Models /codewriterfilter:CodeGenerator.CodeWriterFilter,CodeGenerator  /namingservice:CodeGenerator.NamingService,CodeGenerator /codecustomization:CodeGenerator.CodeCustomizationService,CodeGenerator`

## License

MIT Licence

