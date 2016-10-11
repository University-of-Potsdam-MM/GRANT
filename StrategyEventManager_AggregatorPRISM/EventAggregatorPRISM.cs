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

//using Microsoft.Practices.Prism;
//using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Events;
//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md
//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample

namespace StrategyEventManager_AggregatorPRISM
{
    public class EventAggregatorPRISM : IEventManagerStrategy
    {
        //Todo
        //Instanz des strategyManager abfragen, aus Inti in GrantApplication in MainWindows.Xaml.cs für Arbeit damit in dieser Klasse
        private StrategyManager strategyMgr;
        //Instanz der GrantTrees abfragen für Arbeit damit in dieser Klasse
        //private GeneratedGrantTrees grantTrees;
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        //public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }

        //getstrategymgr ruft nur die lokale gesetzte variable strategymgr ab
        //public StrategyManager getStrategyMgr() { return strategyMgr; }

        /// <summary>
        /// Test
        /// </summary>
        public string g = "wer";
        string IEventManagerStrategy.deliverString()
        {
            //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            return g;
        }

        public IEventAggregator prismEventAggregatorClass = new EventAggregator();

        ////Kreierung des events
        //public class stringOSMEvent : PubSubEvent<string> { }

        ////timerEvent
        //public class stringTimeEvent : PubSubEvent<string> { }

        //#region publisher

        //#endregion

        //#region subscriber
        //#endregion

        //#region handler
        //#endregion

    }
}
