using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using GRANTManager.Interfaces;

//using System.Windows.Automation;
using System.Diagnostics;
//using OSMElement;
using System.Windows;

using Prism.Events;

namespace StrategyEventManager_AggregatorPRISM
{
    class EventAggregatorPRISM : IEventManagerStrategy
    {
        /// <summary>
        /// Test
        /// </summary>
        public string g = "wer";
        string IEventManagerStrategy.deliverString()
        {
            //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            return g;
        }
    }
}
