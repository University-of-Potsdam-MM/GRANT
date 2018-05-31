using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee684092(v=vs.85).aspx
    public enum WindowVisualState
    {
        /// <summary>The window is normal (restored).</summary>
        WindowVisualState_Normal        = 0,

        /// <summary>The window is maximized.</summary>
        WindowVisualState_Maximized     = 1,

        /// <summary>The window is minimized.</summary>
        WindowVisualState_Minimized     = 2,
    }
}