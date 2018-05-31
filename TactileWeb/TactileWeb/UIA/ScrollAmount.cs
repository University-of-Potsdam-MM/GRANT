using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671606(v=vs.85).aspx
    public enum ScrollAmount
    {
        ScrollAmount_LargeDecrement     = 0,
        ScrollAmount_SmallDecrement     = 1,
        ScrollAmount_NoAmount           = 2,
        ScrollAmount_LargeIncrement     = 3,
        ScrollAmount_SmallIncrement     = 4
    }
}