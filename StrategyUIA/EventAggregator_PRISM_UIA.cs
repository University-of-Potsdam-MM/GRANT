using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using System.Windows.Automation;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows;

using Prism.Events;


namespace StrategyUIA
{

    #region eventAggregator

    class EventAggregator_PRISM_UIA

    //todo wie kriege ich das hier öffentlich? ich muss prinzipien/konzepte der klassen/objektorientierung verstehen/wissen

    {
            //readonly IEventAggregator agg = new EventAggregator();
            public IEventAggregator agg = new EventAggregator();

            //todo was bringt token für vorteile?

            //public SubscriptionToken objSubToken;
            //public SubscriptionToken stringSubToken;

            //nur einmaliges subscriben erlauben von einer klasse aus, nicht mehrmalig für dasselbe event anmelden!
            public void aggSubscribe()
            {
                agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM);
                ////agg.GetEvent<stringOSMEvent>().Unsubscribe(generateOSM_2);


                agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM);
                Console.WriteLine("Für Button Event in Prism subscribed");

                //agg.GetEvent<stringOSMEvent>().Subscribe(generateOSM, ThreadOption.UIThread);
            }

        //generateosm verarbeitet das event generateosm2 verarbeitet dasselbe event auch noch anders.
            public void generateOSM(string osm)
            {
                Console.WriteLine("event verarbeitet, derzeit in EventAggregator_Prism, mit übergabe folgenden strings aus der publish-methode: " + osm);
                //osm = "werhers";
            }

        //todo???
        //problem unsubscribe fehlt in auslösen und anmeldung für das event in uia, 
        // es wird dann mehrfach geworfen, da es bei jdem auslösen des buttonevent ein erneutes subscriben und erstellen des prismevent gibt?

            //event publisher
            public void eventOsmChangedHandler()
            {
                //todo
                //dieses event auslösen und testen indem dieses eevent ausgelöst wird nach dem event von OnUIAutomationEvent aus 
                //public class UIAEventMonitor

                //dazu verstehen, wie methoden in anderen klassen aufgerfuen werden und wie dies in grant läuft! mit strategy
                agg.GetEvent<stringOSMEvent>().Publish("Wurf aus EventAggregator_PRISM.cs");

                Console.WriteLine("event gepublished in EventAggregator_Prism ");
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

