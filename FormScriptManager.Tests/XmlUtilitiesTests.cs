using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace FormScriptManager.Tests
{
    [TestClass]
    public class XmlUtilitiesTests
    {
        private readonly string sourceXml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>        <root>
            <children>
                <child name=""name1"" role=""role1"" />
                <child name=""name1"" role=""role2"" attr1=""test1"" attr2=""test2"" />
                <child name=""name2"" />                
            </children>
        </root>
        ";

        [TestMethod]
        public void DoesNotCreateWhenExists()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceXml);

            XmlNode childrenNode = xmlDocument.ChildNodes[1].ChildNodes[0];

            var attributes = new Dictionary<string, string>
            {
                { "name", "name1" },
                { "role", "role2" },
                { "attr1", "test1" },
                { "attr2", "test2" },
            };

            XmlUtilities.EnsureChildNode(childrenNode, "child[@name='name1' and @role='role2']", "child", attributes);

            AssertXmlEqual(xmlDocument.OuterXml, sourceXml);
        }

        [TestMethod]
        public void DoesCreateSimpleWhenDoesntExist()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceXml);

            XmlNode childrenNode = xmlDocument.ChildNodes[1].ChildNodes[0];

            var attributes = new Dictionary<string, string>
            {
                { "name", "name1" },
                { "role", "role3" }
            };

            XmlUtilities.EnsureChildNode(childrenNode, "child2[@name='name1' and @role='role3']", "child", attributes);

            string expectedXml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>            <root>
                <children>
                    <child name=""name1"" role=""role1"" />
                    <child name=""name1"" role=""role2"" attr1=""test1"" attr2=""test2"" />
                    <child name=""name2"" />                    
                    <child name=""name1"" role=""role3"" />
                </children>
            </root>
            ";

            AssertXmlEqual(xmlDocument.OuterXml, expectedXml);
        }

        [TestMethod]
        public void QueryIsUsedAsDefaultName()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceXml);

            XmlNode childrenNode = xmlDocument.ChildNodes[1].ChildNodes[0];

          
            XmlUtilities.EnsureChildNode(childrenNode, "new_node");

            string expectedXml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>            <root>
                <children>
                    <child name=""name1"" role=""role1"" />
                    <child name=""name1"" role=""role2"" attr1=""test1"" attr2=""test2"" />
                    <child name=""name2"" />                   
                    <new_node />
                </children>
            </root>
            ";

            AssertXmlEqual(xmlDocument.OuterXml, expectedXml);
        }

        [TestMethod]
        public void CanUpdateAttributes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceXml);

            XmlNode childrenNode = xmlDocument.ChildNodes[1].ChildNodes[0];

            var attributes = new Dictionary<string, string>
            {
                { "name", "name1" },
                { "role", "role2" },
                { "attr1", "test1_new" },
                { "attr2", "test2_new" },
            };

            XmlUtilities.EnsureChildNode(childrenNode, "child[@name='name1' and @role='role2']", "child", attributes);

            string expectedXml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>            <root>
                <children>
                    <child name=""name1"" role=""role1"" />
                    <child name=""name1"" role=""role2"" attr1=""test1_new"" attr2=""test2_new"" />
                    <child name=""name2"" />  
                </children>
            </root>
            ";

            AssertXmlEqual(xmlDocument.OuterXml, expectedXml);
        }

        private void AssertXmlEqual(string xml1, string xml2)
        {
            Diff d = DiffBuilder.Compare(Input.FromString(xml1))
                                .WithTest(Input.FromString(xml2))
                                .Build();
            Assert.IsFalse(d.HasDifferences(), "Xml documents were expected to be the same, but are different.");
        }
    }
}
