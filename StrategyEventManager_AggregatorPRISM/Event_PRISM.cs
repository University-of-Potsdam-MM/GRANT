using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Collections.ObjectModel;

using GRANTManager;
using GRANTManager.Interfaces;

using Prism.Events;
//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md
//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample

namespace StrategyEvent_PRISM
{
    public class Event_PRISM : IEvent_PRISMStrategy
    {
        //Erhallt der Instanz des StrategyManager für Nutzung
        //Im Interface IEventManagerStrategy wird es gefordert, aber hier in der Klasse wird der StrategyManager nicht benötigt: Er könnte also auch gelöscht werden
        private StrategyManager strategyMgr;
        public void setStrategyMgr(StrategyManager manager)
        {
            strategyMgr = manager;
        }

        //Instanziieren der PRISM-Klasse für Nutzung in allen anderen Klassen und Projekten
        public IEventAggregator prismEventAggregatorClass = new EventAggregator();

        //Abruf der PRISM.Instanz, diese muss in den anderen Klassen nicht instanziiert werden
        IEventAggregator IEvent_PRISMStrategy.getSpecifiedEventManagerClass()
        {
            return prismEventAggregatorClass;
        }
    }

    //todo: Klassennamen der PRISM-Event-Klassen sollten so bezeichnet sein, das sie direkt beschreiben, auf welches Ereignis sie reagieren und auf Sie folgt
    /// <summary>
    /// Klasse des PRISM-Event für Nutzung in anderen Projekten mit .Publish und .Subscribe
    /// </summary>
    public class updateOSMEvent : PubSubEvent<string>
    {
    }

    //timerOSMEvent noch ungenutzt
    public class timerOSMEvent : PubSubEvent<string>
    {
    }

}
