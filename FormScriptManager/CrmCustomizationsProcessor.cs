using FormScriptManager.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace FormScriptManager
{
    public class CrmCustomizationsProcessor
    {
        [Flags]
        public enum FormTypes
        {
            Main = 2,
            QuickCreate = 7
        }

        private readonly IOrganizationService orgService;

        public CrmCustomizationsProcessor(IOrganizationService orgService)
        {
            this.orgService = orgService ?? throw new ArgumentNullException(nameof(orgService));
        }

        /// <summary>
        /// Retrieves FormXml for the specified entity.
        /// </summary>
        /// <param name="entity">Logical name of entity.</param>
        /// <param name="formTypes">Form types to retrieve. Use bit masks to retrieve multiple.</param>
        /// <param name="name">(Optional) Name of the form to retrieve. If empty gets all.</param>
        /// <param name="formId">(Optional) </param>
        /// <returns>An array of FormXML for the specified filters.</returns>
        public EntityForm[] GetForms(string entity, FormTypes formTypes, string name = null, Guid? formId = null)
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
                filterTypes.AddCondition("type", ConditionOperator.Equal, (int)FormTypes.Main);
            }
            if ((formTypes & FormTypes.QuickCreate) == FormTypes.QuickCreate)
            {
                filterTypes.AddCondition("type", ConditionOperator.Equal, (int)FormTypes.QuickCreate);
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