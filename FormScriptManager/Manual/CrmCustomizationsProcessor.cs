using FormScriptManager.Models;
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