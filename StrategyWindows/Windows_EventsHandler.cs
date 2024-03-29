﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using GRANTManager.Interfaces;

using Prism.Events;

using System.Threading;

//using System.Drawing;
////using GRANTApplication;
//using OSMElement;
////using GRANTManager;
////using GRANTManager.Interfaces;
////using TemplatesUi;
//using GRANTManager.TreeOperations;

//todo: test ob hier, in keymouseklasse oder das prism-pakcage istalliert/eingebunden sein muss

//todo: unsubscribe für das uia-event in klasse setzen, da es sonst bei jedem neuen erstellen einer instanz neu subscribed wird, oder reicht das unsubscribe in der Windowsvenetsmonitor.cs
//Frage: es wird dann mehrfach geworfen, da es bei jdem auslösen des buttonevent ein erneutes subscriben und erstellen des prismevent gibt?


namespace StrategyWindows
{
    public class Windows_EventsHandler //: IEventManagerStrategy
    {
        #region windowsEventsHandler

        public IEventAggregator prismEventAggregatorClass;
        public StrategyManager strategyMgr;

        public Windows_EventsHandler(StrategyManager manager)
        {
            strategyMgr = manager;
            prismEventAggregatorClass = strategyMgr.getSpecifiedEventStrategy().getSpecifiedEventManagerClass();
        }

        /// <summary>
        /// Methode der Eventverarbeitung beim Wurf eines Key-Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e != null)
            {
                String HWNDString = "Null";
                IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().getForegroundWindow();
                if (hwnd == IntPtr.Zero) { return; }
                else
                {
                    //Thread.Sleep(1000);

                    System.Diagnostics.Debug.WriteLine("hwnd " + hwnd.ToString());
                    //applicationName = strategyMgr.getSpecifiedOperationSystem().getProcessNameOfApplication((int)hwnd);
                    HWNDString = hwnd.ToString();

                    IntPtr test;
                    test = strategyMgr.getSpecifiedOperationSystem().getHWNDByInt32String(HWNDString);

                    //HWNDString += "-";
                    //HWNDString += test.ToString();
                    //HWNDString += "-";



                    //Thread.Sleep(1000);
                }

                Console.WriteLine("KeyUp (Info aus WindowsKlasse)  \t\t {0}\n", e.KeyCode, HWNDString ,DateTime.Now.ToString());
                mouseKeyHookEventHandler("Keyboard", "KeyUp", e.KeyCode.ToString(), HWNDString, DateTime.Now.ToString());
            }
        }

        //gibt Infos des Eventwurfs an PRISM weiter, Publish    
        /// <summary>
        /// sonderzeichen bei Stringübergabe in prims ist "_"
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="mouseKeyEventType"></param>
        /// <param name="mouseKeyEventValue"></param>
        /// <param name="HWNDString"></param>
        /// <param name="dateTimeNow"></param>
        public void mouseKeyHookEventHandler(string eventType, string mouseKeyEventType, string mouseKeyEventValue, string HWNDString, string dateTimeNow)
        {
            //Console.WriteLine("(Info aus WindowsKlasse) Publish für Prismklasse erfolgt jetzt " + mouseKeyEventType + mouseKeyEventValue + dateTimeNow);

            prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Publish(eventType + "_" + mouseKeyEventType + "_" + mouseKeyEventValue + "_" + HWNDString + "_" + dateTimeNow);
        }
    }
    #endregion

    #region oldEventsHandler
    //public IEventAggregator prismEventAggregator = new EventAggregator();


    //    //public EventAggregatorPRISM_GRANTManager ea = new EventAggregatorPRISM_GRANTManager();

    //    public Windows_EventsHandler(StrategyManager manager)
    //    {
    //        strategyMgr = manager;
            
    //        //prismEventAggregator = strategyMgr.getSpecifiedEventManager().getSpecifiedEventManagerClass();
    //    }



    //    //todo was bringt token für vorteile? wie wird es genutzt?
    //    //public SubscriptionToken objSubToken;
    //    //public SubscriptionToken stringSubToken;

    //    public StrategyManager strategyMgr;

    //    //todo unklar ob das hier gebraucht wird...
    //    //public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }

    //    public void onKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
    //    {
    //        //Console.WriteLine("KeyUp  \t\t {0}\n", e.KeyValue);
    //        if (e != null)
    //        {
    //            Console.WriteLine("KeyUp (Info aus WindowsKlasse)  \t\t {0}\n", e.KeyCode, DateTime.Now.ToString());
    //            //return e.KeyValue.ToString();
    //            //der wurf soll hier nicht erfolgen???, es erfolgt ein aufruf einer methode in rprimsklasse und die parameter werde von hier mit übergeben, in prism klasse erfolgt der publish
    //            //diese methode hat als parameter einfach alle infos zu dem event und kann immer dieselbe bezeichnung haben
    //            //cea.agg.GetEvent<stringOSMEvent>().Publish("Wurf aus windowskeyEventsMonitorKlasse")

    //            //prismEventAggregator = strategyMgr.getSpecifiedEventManager().getSpecifiedEventManagerClass();

    //            mouseKeyHookEventHandler("KeyUp", e.KeyCode.ToString(), DateTime.Now.ToString());


    //            ////todo kann alles weg?
    //            //////test ab hier für erhalt der initialisierten klasse, der instanz                
    //            //strategyMgr = new StrategyManager();
    //            //List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
    //            //String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
    //            //strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));

    //            //string g = "qw";
    //            //Console.WriteLine("hier hier hier: " + g);

    //            //g = strategyMgr.getSpecifiedOperationSystem().deliverPRISMEventClassString();
    //            //Console.WriteLine("hier hier hier: " + g);

    //            ////// bis hier
    //        }
    //    }

    //    //IEventAggregator IEventManagerStrategy.getSpecifiedEventManagerClass()
    //    //{
    //    //    //strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
    //    //    return prismEventAggregator;
    //    //}


    //    //gibt info des eventwurf an prism weiter , es passiert hier das publish    
    //    public void mouseKeyHookEventHandler(string mouseKeyEventType, string mouseKeyEventValue, string dateTimeNow)
    //    {
    //        Console.WriteLine("(Info aus WindowsKlasse) Publish für Prismklasse erfolgt jetzt " + mouseKeyEventType + mouseKeyEventValue + dateTimeNow);

    //        //liefert prismeventaggregator-klasse über interface zurück, nicht die prism_handler-Class, mit dem umbenannten event
    //        prismEventAggregator = strategyMgr.getSpecifiedEventManager().getSpecifiedEventManagerClass();


    //        //prismEventAggregator.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin


    //        //Publish
    //        //aufruf mittels übergebenem prismeventaggregator
    //        //prismEventAggregator.GetEvent<stringOSMEventTest>().Publish(mouseKeyEventType + mouseKeyEventValue);

    //        ///todo hier ist klasse des event aus prism direkt aus grantmanager, globale klasse, genutzt, ohne instanzbildung
    //        ///nächster test subscribe des event dieser klasse in ganz anderem teilprojekt!
    //        prismEventAggregator.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Publish(mouseKeyEventType + mouseKeyEventValue);

    //        //generateOSMmwxamlTest("asd");

    //////////Aufruf direkt von globalem prismeventaggregator
    ////////prismEventAggregator.GetEvent<stringOSMEvent>().Publish(mouseKeyEventType);

    //        //ea.prismEventAggregatorClass.GetEvent<>().Publish(dateTimeNow);
    //        //ea = this.ea;
    //        //ea.prismEventAggregatorClass.GetEvent<>().
    //    }

    //    public void generateOSMmwxamlTest(string osm)
    //    {
    //        Console.WriteLine("winevent verarbeitet in wineventhandler" + osm);
    //        //osm = "werhers";
    //    }


    //        public void prismeventhandler() 
    //        {
    //            //Console.WriteLine("Prismklasse" + stringOSMEvent);
    //        }

    //        //nur einmaliges subscriben erlauben von einer klasse aus, nicht mehrmalig für dasselbe event anmelden!
    //        public void aggSubscribe()
    //        {
    //            //prismEventAggregator.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM);
    //            //////agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM_2);


    //            //prismEventAggregator.GetEvent<stringOSMEvent>().Subscribe(generateOSM);

    //            //Console.WriteLine("Für Button Event in Prism subscribed");

    //            ////agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM, ThreadOption.UIThread);
    //        }

    //    //generateosm verarbeitet das event generateosm2 verarbeitet dasselbe event auch noch anders.
    //        public void generateOSM(string osm)
    //        {
    //            Console.WriteLine("winevent verarbeitet" + osm);
    //            //osm = "werhers";
    //        }

    //    //todo???
    //    //problem unsubscribe fehlt in auslösen und anmeldung für das event in uia, 
    //    // es wird dann mehrfach geworfen, da es bei jdem auslösen des buttonevent ein erneutes subscriben und erstellen des prismevent gibt?

    //        //event publisher
    //        public void eventOsmChangedHandler()
    //        {
    //            ////todo
    //            ////dieses event auslösen und testen indem dieses eevent ausgelöst wird nach dem event von OnUIAutomationEvent aus 
    //            ////public class UIAEventMonitor

    //            ////dazu verstehen, wie methoden in anderen klassen aufgerfuen werden und wie dies in grant läuft! mit strategy

    //            ////der Wurf dieses event könte auch direkt aus uiaeventsmonitor.cs erfolgen dazu müsste allerdings agg von dort aus zugreifbar sein.
    //            //prismEventAggregator.GetEvent<stringOSMEvent>().Publish("Wurf aus windowseventmonitor.cs");

    //            //Console.WriteLine("event gepublished in EventAggregator_Prism ");
    //        }

    //        //der publisher ist der button der normalen anwendung, dieser wirft ein eneus event 
    //        //und der subscriber von diesem neuen event
    //        //behandelt dieses event dann

    //        //public void Dispose()
    //        //{
    //        //    //unsub
    //        //    prismEventAggregator.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM);
    //        //}

    //}

    ////Kreierung des events
    //public class stringOSMEventTest : PubSubEvent<string> { }

    ////timerEvent
    //public class stringTimeEvent : PubSubEvent<string> { }

    #endregion
}

