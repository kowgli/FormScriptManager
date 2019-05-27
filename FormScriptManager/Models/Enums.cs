﻿using System;

namespace FormScriptManager.Models
{
    /// <summary>
    /// Common enumerations
    /// </summary>
    public class Enums
    {
        [Flags]
        public enum FormTypes
        {
            Main = 1,
            QuickCreate = 2
        }
    }
}
