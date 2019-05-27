using System;

namespace FormScriptManager.Models
{
    /// <summary>
    /// An instance of an entity form
    /// </summary>
    public class EntityForm
    {
        public Guid Id { get; set; }        
        public string FormXml { get; set; }
    }
}