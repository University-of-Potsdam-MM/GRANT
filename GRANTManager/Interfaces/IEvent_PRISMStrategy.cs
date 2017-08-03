using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using Prism.Events;

namespace GRANTManager.Interfaces
{
    /// <summary>
    /// Dieses Interface ist festgelegt auf PRISM für die IEventmanagerStrategy. Es sind nur die Methoden, welche PRISM sowieso bereitstellt benutzbar
    /// </summary>
    public interface IEvent_PRISMStrategy
    {
        // Die Methode wird nur benötigt, wenn in der Umsetzung des Interface auch weitere Methoden aus dem StratgyManager genutzt werden.
        void setStrategyMgr(StrategyManager manager);

        IEventAggregator getSpecifiedEventManagerClass(); 
    }
}
