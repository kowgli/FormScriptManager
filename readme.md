# Dynamics CRM / CE Form Script Manager

.NET library that allows registering client side form scripts using server side code (C#).

Using it you can add on-load and on-save scripts on specified forms.

It has two modes of operation: **simplified** (which has a set of common sense default values) and **manual** where you can setup everything yourself.

## Installing via NuGet

```
Install-Package FormScriptManager
```

[NuGet link](https://www.nuget.org/packages/FormScriptManager/)

## Simplified mode

Just register an on-load or on-save script on all entities Main or Quick Create forms. Publish entity after it's done.

Use the static method:

```FormScriptManager.Processor.AddFormScript(IOrganizationService orgService, string entity, FormTypes formTypes, string libraryName, string functionName, EventTypes eventType)```

__Example:__

```Processor.AddFormScript(orgService, "lead", FormTypes.Main | FormTypes.QuickCreate, "new_testscript.js", "test_onload", EventTypes.OnLoad);```

For the lead entity, on both Main and Quick Create forms adds the function ```test_onload``` from the ```new_testscript.js``` library as an on-load event handler.

## Manual mode

Look into the ```FormScriptManager.Manual``` namespace. This document will be expanded with more information.





