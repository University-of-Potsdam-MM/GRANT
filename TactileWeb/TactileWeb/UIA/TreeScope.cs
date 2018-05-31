using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/ms727434(v=vs.85).aspx
    public enum TreeScope
    {
        /// <summary>The scope includes the element itself.</summary>
        TreeScope_Element       = 0x1,

        TreeScope_Children      = 0x2,
        TreeScope_Descendants   = 0x4,
        TreeScope_Parent        = 0x8,
        TreeScope_Ancestors     = 0x10,
        TreeScope_Subtree       = ( ( TreeScope_Element | TreeScope_Children )  | TreeScope_Descendants ) 
    }
}