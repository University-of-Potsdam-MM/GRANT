using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManager
{
    /// <summary>
    /// Enum für die Filterung im Baum;
    /// orientiert an UIA (https://msdn.microsoft.com/en-us/library/ms752331(v=vs.110).aspx)
    /// </summary>
    public enum TreeScopeEnum { Parent, Ancestors, Element, Children, Descendants, Sibling, Application }
}
