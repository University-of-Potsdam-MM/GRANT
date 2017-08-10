using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager
{
    /// <summary>
    /// Enum for filtering;
    /// orientiert an UIA ( https://msdn.microsoft.com/en-us/library/ms752331(v=vs.110).aspx, https://docs.microsoft.com/en-us/dotnet/api/system.windows.automation.treescope?view=netframework-4.7)
    /// </summary>
    public enum TreeScopeEnum {
        /// <summary>
        /// Specifies that the filtering include the element's ancestors, including the parent.
        /// </summary>
        Ancestors,
        /// <summary>
        /// Specifies that the filtering include the element itself.
        /// </summary>
        Element,
        /// <summary>
        /// Specifies that the filtering include the element's immediate children.
        /// </summary>
        Children,
        /// <summary>
        /// Specifies that the filtering include the root of the search and all descendants.
        /// </summary>
        Subtree,
        /// <summary>
        /// Specifies that the filtering include the element's descendants, including children.
        /// </summary>
        Descendants,
        /// <summary>
        /// Specifies that the filtering include the siblings (but not the element itself).
        /// </summary>
        Sibling,
        /// <summary>
        /// Specifies that the whole application will be filtered.
        /// </summary>
        Application
    }
}
