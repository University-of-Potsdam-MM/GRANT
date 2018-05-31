using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee684088(v=vs.85).aspx
    public enum WindowInteractionState 
    {

        /// <summary>The window is running. This does not guarantee that the window is ready for user interaction or is responding.</summary>
        WindowInteractionState_Running                  = 0,

        /// <summary>The window is closing.</summary>
        WindowInteractionState_Closing                  = 1,

        /// <summary>The window is ready for user interaction.</summary>
        WindowInteractionState_ReadyForUserInteraction  = 2,

        /// <summary>The window is blocked by a modal window.</summary>
        WindowInteractionState_BlockedByModalWindow     = 3,

        /// <summary>The window is not responding.</summary>
        WindowInteractionState_NotResponding            = 4
    }
}