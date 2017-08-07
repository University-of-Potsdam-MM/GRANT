using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager
{
    /// <summary>
    /// Enum für die Filterung im Baum;
    /// orientiert an UIA ( https://msdn.microsoft.com/en-us/library/ms752331(v=vs.110).aspx, https://docs.microsoft.com/en-us/dotnet/api/system.windows.automation.treescope?view=netframework-4.7)
    /// </summary>
    public enum TreeScopeEnum {  Ancestors, Element, Children, Subtree, Descendants, Sibling, Application }
}
