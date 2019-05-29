using FormScriptManager.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using static FormScriptManager.Models.Enums;

namespace FormScriptManager.Manual
{
    /// <summary>
    /// Groups together methods to work with CRM forms and metadata management
    /// </summary>
    public class CrmCustomizationsProcessor
    {
        private readonly IOrganizationService orgService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orgService">Organization service reference for the CRM to work in</param>
        public CrmCustomizationsProcessor(IOrganizationService orgService)
        {
            this.orgService = orgService ?? throw new ArgumentNullException(nameof(orgService));
        }

        /// <summary>
        /// Retrieves FormXml for the specified entity.
        /// </summary>
        /// <param name="entity">Logical name of entity.</param>
        /// <param name="formTypes">Form types to retrieve. Use bit masks to retrieve multiple.</param>
        /// <returns>An array of FormXML for the specified filters.</returns>
        public EntityForm[] GetForms(string entity, FormTypes formTypes)
        {
            return GetForms(entity, formTypes, null, null);
        }

        /// <summary>
        /// Retrieves FormXml for the specified entity.
        /// </summary>
        /// <param name="entity">Logical name of entity.</param>
        /// <param name="formTypes">Form types to retrieve. Use bit masks to retrieve multiple.</param>
        /// <param name="name">(Optional) Name of the form to retrieve. If empty gets all.</param>
        /// <returns>An array of FormXML for the specified filters.</returns>
        public EntityForm[] GetForms(string entity, FormTypes formTypes, string name)
        {
            return GetForms(entity, formTypes, name, null);
        }

        /// <summary>
        /// Retrieves FormXml for the specified entity.
        /// </summary>
        /// <param name="entity">Logical name of entity.</param>
        /// <param name="formTypes">Form types to retrieve. Use bit masks to retrieve multiple.</param>
        /// <param name="formId">(Optional) </param>
        /// <returns>An array of FormXML for the specified filters.</returns>
        public EntityForm[] GetForms(string entity, FormTypes formTypes, Guid formId)
        {
            return GetForms(entity, formTypes, null, formId);
        }

        /// <summary>
        /// Updates the Form XML of the chosen form.
        /// </summary>
        /// <param name="formId">Primary ID of the form.</param>
        /// <param name="formXml">New Form XML.</param>
        public void UpdateFormXmlOnForm(Guid formId, string formXml)
        {
            Entity systemForm = new Entity("systemform");
            systemForm["formid"] = formId;
            systemForm["formxml"] = formXml;

            orgService.Update(systemForm);
        }

        /// <summary>
        /// Publishes changes to the selected entity. Call this after updating the Form XML.
        /// </summary>
        /// <param name="entity">Logical name of the entity to publish.</param>
        public void PublishEntity(string entity)
        {
            var request = new PublishXmlRequest { ParameterXml = $"<importexportxml><entities><entity>{entity}</entity></entities></importexportxml>" };
            orgService.Execute(request);
        }
       
        private EntityForm[] GetForms(string entity, FormTypes formTypes, string name, Guid? formId)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            QueryExpression query = new QueryExpression("systemform");
            query.ColumnSet = new ColumnSet("formid", "formxml");
            query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, entity);
            query.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("formactivationstate", ConditionOperator.Equal, 1);

            FilterExpression filterTypes = new FilterExpression(LogicalOperator.Or);

            if((formTypes & FormTypes.Main) == FormTypes.Main)
            {
                filterTypes.AddCondition("type", ConditionOperator.Equal, 2);
            }
            if ((formTypes & FormTypes.QuickCreate) == FormTypes.QuickCreate)
            {
                filterTypes.AddCondition("type", ConditionOperator.Equal, 7);
            }
            query.Criteria.AddFilter(filterTypes);


            if (name != null)
            {
                query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
            }
            if (formId != null)
            {
                query.Criteria.AddCondition("formid", ConditionOperator.Equal, formId.Value);
            }

            EntityCollection forms = orgService.RetrieveMultiple(query);

            return forms.Entities.Select(f => new EntityForm
            {
                Id = f.GetAttributeValue<Guid>("formid"),
                FormXml = f.GetAttributeValue<string>("formxml")
            }).ToArray();
        }
    }
}