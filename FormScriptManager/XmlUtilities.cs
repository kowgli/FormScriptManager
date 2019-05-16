using System.Collections.Generic;
using System.Xml;

namespace FormScriptManager
{
    internal static class XmlUtilities
    {
        /// <summary>
        /// Ensures a child node of a given node exists. Creates it if it doesn't.
        /// </summary>
        /// <param name="parentNode">The parent node to which append the child node to</param>
        /// <param name="childNodeQuery">XPath query for finding the child node</param>
        /// <param name="childNodeName">(Optional) The name of the child node if needs to be created. Defaults to the XPath query if not specified. Makes sense if the query is just the name of the node.</param>
        /// <param name="attributes">(Optional) Key-value collection of attributes to add to the node if it is created</param>
        /// <returns>The existing or created child node</returns>
        public static XmlNode EnsureChildNode(XmlNode parentNode, string childNodeQuery, string childNodeName = null, Dictionary<string, string> attributes = null)
        {
            childNodeName = childNodeName ?? childNodeQuery;
            attributes = attributes ?? new Dictionary<string, string>();

            XmlNode childNode = parentNode.SelectSingleNode(childNodeQuery);
            if (childNode == null)
            {
                childNode = parentNode.OwnerDocument.CreateElement(childNodeName);

                foreach (string key in attributes.Keys)
                {
                    AddAttribute(childNode, key, attributes[key]);
                }

                parentNode.AppendChild(childNode);
            }

            return childNode;
        }

        private static void AddAttribute(XmlNode node, string name, string value)
        {
            var attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.Value = value;

            node.Attributes.Append(attribute);
        }
    }
}