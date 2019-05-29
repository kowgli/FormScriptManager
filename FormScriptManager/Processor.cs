using FormScriptManager.Manual;
using Microsoft.Xrm.Sdk;
using System;
using static FormScriptManager.Models.Enums;

namespace FormScriptManager
{
    /// <summary>
    /// This is a wrapper over the more detailed methods of script registration. It uses common sense defaults and should cover most scenarios.
    /// If you need more control use the classes inside the FormScriptManager.Manual namespace.
    /// </summary>
    public static class Processor
    {
        /// <summary>
        /// Registers a form script with common sense defaults. This will not create duplicates if the script is already there.
        /// By default the functions will have no parameters and the execution context passed.
        /// JS doesn't care about additional function arguments.
        /// If you need more control use the methods from the FormScriptManager.Manual namespace. 
        /// </summary>
        /// <param name="orgService">Reference to the CRM organization service.</param>
        /// <param name="entity">Logical name of the entity the script should be registered on.</param>
        /// <param name="formTypes">Type of forms the script shoult be registered on: Main / Quick Create. Use bit masks for both.</param>
        /// <param name="libraryName">Name of the library to register.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="eventTypes">Which event should the script be registered on: OnLoad / OnSave.</param>
        public static void AddFormScript(IOrganizationService orgService, string entity, FormTypes formTypes, string libraryName, string functionName, EventTypes eventType)
        {
            orgService = orgService ?? throw new ArgumentNullException(nameof(orgService));
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            libraryName = libraryName ?? throw new ArgumentNullException(nameof(libraryName));
            functionName = functionName ?? throw new ArgumentNullException(nameof(functionName));

            var customizationsProcessor = new CrmCustomizationsProcessor(orgService);

            Models.EntityForm[] forms = customizationsProcessor.GetForms(entity, formTypes);

            foreach(var form in forms)
            {
                string newFormXml = FormXmlProcessor.UpsertEventHandler(form.FormXml, eventType, libraryName, functionName, parameters: "", passExecutionContext: true, enabled: true);

                customizationsProcessor.UpdateFormXmlOnForm(form.Id, newFormXml);
            }

            customizationsProcessor.PublishEntity(entity);
        }
    }
}
