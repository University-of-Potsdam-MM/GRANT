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

using System.Collections.ObjectModel;

//todo: notwendig für erhalt der prismeventklasse, sollte nicht sein!
using StrategyWindows;

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


        public IEventAggregator prismEventAggregatorClass = new EventAggregator();

        //public EventAggregatorPRISM() 
        //{
        //    prismMouseKeyHookEventHandler_Subscribe();
        //}

        /// <summary>
        /// Test
        /// </summary>
        //public string g = "wer";
        IEventAggregator IEventManagerStrategy.getSpecifiedEventManagerClass()
        {
            //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            return prismEventAggregatorClass;
        }

        //#region publisher
        public void prismMouseKeyHookEventHandler_Subscribe()
        {
            Console.WriteLine("(Info aus WindowsKlasse) Publish für Prismklasse folgt");
            //Publish
            //aufruf mittels übergebenem prismeventaggregator
            prismEventAggregatorClass.GetEvent<stringOSMEventTest>().Subscribe(generateOSM);

            //object pd = new updateOSMEvent();

            ////prismEventAggregatorClass.GetType().
            ////prismEventAggregatorClass.GetEvent<pd>().Subscribe(generateOSM);

            /////hier weitermachen: wie die klasse updateosmevent weitergeben für nutzung, bzw. verfügbar machen?
            //prismEventAggregatorClass.GetEvent<updateOSMEvent>().Subscribe(generateOSM);

            PRISMHandler_Class p = new PRISMHandler_Class();

            //prismEventAggregator.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Publish(mouseKeyEventType + mouseKeyEventValue);
            //prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Publish(mouseKeyEventType + mouseKeyEventValue);
            prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Subscribe(p.generateOSM_PRISMHandler_Class); ///hier muss ein subscribe hin

            prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Subscribe(generateOSM); ///hier muss ein subscribe hin


        }


        //#endregion

        //#region subscriber
        //#endregion

        //#region handler
        public void generateOSM(string osm)
        {
            Console.WriteLine("winevent verarbeitet" + osm);
            //osm = "werhers";
        }

        //#endregion
    }

    ////Kreierung des event
    // dies evtl. in eigene klasse, es ist der verbinder zwischen dem subscriber und dem publisher

    //uklare bezeichnung dieser klasse, sollte sie so bezeichnet sein, als ob sie beschreibt, was die folge des events ist, oder sollte sie so bezeichnet sein
    //public class stringOSMEvent : PubSubEvent<string> { }

    ////???typ, ist dieser auch über grantStrategymanager nutzbar???
    //public class updateOSMEvent : PubSubEvent<string> 
    //{    
    //    //void get()
    //    //{
    //    //    updateOSMEvent.
    //    //}
    //}


    //updateOSMEvent IEventManagerStrategy.getUpdateOSMEventClass()
    //{
    //    //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
    //    return updateOSMEvent;
    //}

    //todo: Rückgabe der klassen, als liste geht nicht
    ////List<PubSubEvent> list = new List<PubSubEvent>();
    ////Collection<IEventAggregator> col = new Collection<EventAggregator>();
    //public void setList()
    //{
    //    //list.Add(stringOSMEvent);
    //    //col.Add(EventAggregatorPRISM.updateOSMEvent);
    //}

    ////    IEventManagerStrategy.getSpecifiedEventManagerClass()
    ////{
    ////    //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
    ////    return prismEventAggregatorClass;
    ////}


    ////timerEvent
    //public class stringTimeEvent : PubSubEvent<string> { }

}
