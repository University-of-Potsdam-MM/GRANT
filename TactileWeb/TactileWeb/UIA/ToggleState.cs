using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671606(v=vs.85).aspx
    public enum ToggleState
    {
        /// <summary>The UI Automation element is not selected, checked, marked or otherwise activated</summary>
        ToggleState_Off                 = 0,

        /// <summary>The UI Automation element is selected, checked, marked or otherwise activated.</summary>
        ToggleState_On                  = 1,

        /// <summary>The UI Automation element is in an indeterminate state.</summary>
        ToggleState_Indeterminate       = 2,
    }
}