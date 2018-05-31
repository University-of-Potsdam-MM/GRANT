using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/aa359456(v=vs.85).aspx
    public enum TextUnit
    {
        TextUnit_Character  = 0,
        TextUnit_Format     = 1,
        TextUnit_Word       = 2,
        TextUnit_Line       = 3,
        TextUnit_Paragraph  = 4,
        TextUnit_Page       = 5,
        TextUnit_Document   = 6
    }
}