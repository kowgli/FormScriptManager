using System;
using System.Collections.Generic;
using System.Xml;

namespace FormScriptManager
{
    public static class FormXmlProcessor
    {
        public enum EventTypes
        {
            OnLoad,
            OnSave
        }

        /// <summary>
        /// Adds the specified form library to the FormXml, if not already there.
        /// </summary>
        /// <param name="formXml">The FormXml the form library should be added to</param>
        /// <param name="libraryName">The name of the library to be added</param>
        /// <returns>FormXml with the added library</returns>
        public static string UpsertLibrary(string formXml, string libraryName)
        {
            if (String.IsNullOrWhiteSpace(formXml)) { throw new ArgumentException("formXml needs to have a non-empty value", nameof(formXml)); }
            if (String.IsNullOrWhiteSpace(libraryName)) { throw new ArgumentException("libraryName needs to have a non-empty value", nameof(libraryName)); }

            var formDoc = new XmlDocument();
            formDoc.LoadXml(formXml);

            XmlNode form = formDoc.SelectSingleNode("form") ?? throw new Exception("Expected node \"form\" was not found");

            XmlNode formLibraries = XmlUtilities.EnsureChildNode(form, "formLibraries");

            XmlUtilities.EnsureChildNode(formLibraries, $"Library[@name = '{libraryName}']", "Library",
                new Dictionary<string, string>
                {
                    { "name", libraryName },
                    { "libraryUniqueId", Guid.NewGuid().ToString("B") }
                }
            );

            return formDoc.OuterXml;
        }

        /// <summary>
        /// Add the specified form library (if not already there) and event handler (if not already there) to the FormXml.
        /// </summary>
        /// <param name="formXml">The FormXml the form library and event handler should be added to</param>
        /// <param name="eventType">Type of event - onLoad or onSave</param>
        /// <param name="libraryName">The name of the library to be added</param>
        /// <param name="functionName">The function to be called in the event handler</param>
        /// <param name="parameters">Optional parameters for the event handler</param>
        /// <param name="passExecutionContext">Should the execution context be passed to the function</param>
        /// <param name="enabled">Should the event handler be enabled</param>
        /// <returns>FormXml with specified library and event handler added.</returns>
        public static string UpsertEventHandler(string formXml, EventTypes eventType, string libraryName, string functionName, string parameters, bool passExecutionContext, bool enabled)
        {
            formXml = UpsertLibrary(formXml, libraryName);

            var formDoc = new XmlDocument();
            formDoc.LoadXml(formXml);

            string eventName = (eventType == EventTypes.OnLoad ? "onload" : "onsave");

            XmlNode form = formDoc.SelectSingleNode("form") ?? throw new Exception("Expected node \"form\" was not found");

            XmlNode events = XmlUtilities.EnsureChildNode(form, "events");

            XmlNode @event = XmlUtilities.EnsureChildNode(events, $"event[@name = '{eventName}']", "event",
                new Dictionary<string, string>
                {
                    { "name", eventName },
                    { "application", "false" },
                    { "active", "false" }
                }
            );

            XmlNode handlers = XmlUtilities.EnsureChildNode(@event, "Handlers");

            XmlUtilities.EnsureChildNode(handlers, $"Handler[@functionName='{functionName}' and @libraryName='{libraryName}']", "Handler",
                new Dictionary<string, string>
                {
                    { "functionName", functionName },
                    { "libraryName", libraryName },
                    { "handlerUniqueId", Guid.NewGuid().ToString("B") },
                    { "enabled", enabled ? "true" : "false" },
                    { "parameters", parameters },
                    { "passExecutionContext", passExecutionContext ? "true" : "false" }
                }
            );

            return formDoc.OuterXml;
        }
    }
}