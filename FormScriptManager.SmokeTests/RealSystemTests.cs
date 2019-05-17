using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using static FormScriptManager.CrmCustomizationsProcessor;

namespace FormScriptManager.SmokeTests
{
    [TestClass]
    public class RealSystemTests
    {
        IOrganizationService OrgService => new CrmServiceClient(System.IO.File.ReadAllText("__ConnectionString.txt"));

        [TestMethod]
        public void CanGetLeadMainForms()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main);

            Assert.IsTrue(forms.Length == 2);
        }

        [TestMethod]
        public void CanGetQuickCreateForm()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.QuickCreate);

            Assert.IsTrue(forms.Length == 1);
        }

        [TestMethod]
        public void CanGetBothTypesOfForm()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main | FormTypes.QuickCreate);

            Assert.IsTrue(forms.Length == 3);
        }

        [TestMethod]
        public void CanGetByName()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main, "Lead");

            Assert.IsTrue(forms.Length == 1);
        }

        [TestMethod]
        public void ReturnsEmptyArrayIfNotFoundByName()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main, "LeadXYZ");

            Assert.IsTrue(forms.Length == 0);
        }

        [TestMethod]
        public void CanGetById()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main, null, new Guid("E3B6DDB7-8DF0-4410-AC7B-FD32E5053D38"));

            Assert.IsTrue(forms.Length == 1);
        }

        [TestMethod]
        public void CanGetByAllFiltersTogether()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main, "Lead", new Guid("E3B6DDB7-8DF0-4410-AC7B-FD32E5053D38"));

            Assert.IsTrue(forms.Length == 1);
        }

        [TestMethod]
        public void ReturnsEmptyArrayIfNotFoundById()
        {
            if (!Debugger.IsAttached) { return; }

            CrmCustomizationsProcessor crmCustomizationsProcessor = new CrmCustomizationsProcessor(OrgService);

            var forms = crmCustomizationsProcessor.GetForms("lead", FormTypes.Main, null, Guid.NewGuid());

            Assert.IsTrue(forms.Length == 0);
        }
    }
}
