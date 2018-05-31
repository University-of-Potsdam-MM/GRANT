using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671226(v=vs.85).aspx
    public enum ExpandCollapseState
    {
        /// <summary>No children are visible.</summary>
        ExpandCollapseState_Collapsed         = 0,

        /// <summary>All children are visible.</summary>
        ExpandCollapseState_Expanded          = 1,

        /// <summary>Some, but not all, children are visible.</summary>
        ExpandCollapseState_PartiallyExpanded = 2,

        /// <summary>The element does not expand or collapse.</summary>
        ExpandCollapseState_LeafNode          = 3,
    }
}