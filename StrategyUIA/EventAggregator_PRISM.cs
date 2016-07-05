using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StrategyManager;
using System.Windows.Automation;
using System.Diagnostics;
using StrategyManager.Interfaces;
using OSMElement;
using System.Windows;

using Prism.Events;


namespace StrategyUIA
{

    #region eventAggregator

    class EventAggregator_PRISM

    //todo wie kriege ich das hier öffentlich? ich muss prinzipien/konzepte der klassen/objektorientierung verstehen/wissen

    {
            //readonly IEventAggregator agg = new EventAggregator();
            public IEventAggregator agg = new EventAggregator();

            //todo was bringt token für vorteile?

            //public SubscriptionToken objSubToken;
            //public SubscriptionToken stringSubToken;

            public void aggSubscribe()
            {
                agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM);
                agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM_2);


                agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM);
                //agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM, ThreadOption.UIThread);
                agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM_2);
            }

        //generateosm verarbeitet das event generateosm2 verarbeitet dasselbe event auch noch anders.
            public void generateOSM(string osm)
            {
                Console.WriteLine("event verarbeitet" + osm);
                osm = "werhers";
            }

            public void generateOSM_2(string osm)
            {
                Console.WriteLine("event anders verarbeitet" + osm);
                osm = "werhers";
            }


        //todo
        //problem unsubscribe fehlt in auslösen des event in uia, 
        // es wird dann mehrfach geworfen, da es bei jdem auslösen des buttonevent ein erneutes subscriben und erstellen des prismevent gibt?

            //event publisher
            public void eventOsmChangedHandler()
            {
                //todo
                //dieses event auslösen und testen indem dieses eevent ausgelöst wird nach dem event von OnUIAutomationEvent aus 
                //public class UIAEventMonitor

                //dazu verstehen, wie methoden in anderen klassen aufgerfuen werden und wie dies in grant läuft! mit strategy
                agg.GetEvent<stringOSMEvent>().Publish("tada");

                agg.GetEvent<stringOSMEvent>().Publish("zweiteevent werfen nach dem eigentlichen event vom button aus uia");

                Console.WriteLine("event geworfen");
            }

            //der publisher ist der button der normalen anwendung, dieser wirft ein eneus event 
            //und der subscriber von diesem neuen event
            //behandelt dieses event dann

            public void Dispose()
            {
                //unsub
                agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM);
            }
        }

        //Kreierung des events
        public class stringOSMEvent : PubSubEvent<string> { }

        //timerEvent
        public class stringTimeEvent : PubSubEvent<string> { }

        #endregion
    }

