using GRANTManager;
using GRANTManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Prism.Events;


namespace StrategyEvent
{
    
    public class EventProcessor : IEventProcessor
    {
        StrategyManager strategyManager;

        public EventProcessor(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;

            eventTest();
        }

        //todo inti der prismeventaggreagtorclass in grantapplication einbauen
        public IEventAggregator prismEventAggregatorClass;// = new EventAggregator();

        public void eventTest()
        {
            //erhalt des prismaggregator über interface
            prismEventAggregatorClass = strategyManager.getSpecifiedEventStrategy().getSpecifiedEventManagerClass();

            //prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent_PRISMHandler_GrantManager>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin
            prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Subscribe(eventValueParsing); ///hier muss ein subscribe hin

            //Console.WriteLine("test winevent verarbeitet in mainwindowxaml_");


        }

        //Aufschlüsselung der Informationen aus dem Event-String in ein OSMEvent-Format für die weitere Verarbeitung
        //liefert den string, die eventid
        public void eventValueParsing(string todo)
        {
            Debug.WriteLine("winevent verarbeitet in eventprocessor" + todo);
            //osm = "werhers";

            //Aufruf eienr Methode aus EventAction über startegymanager
            //strategyManager.getSpecifiedEventAction().filterOSM();

            //strategyManager.getSpecifiedEventManager().deliverActionListForEvent();


            //strategyManager.getSpecifiedEventManager()
            //id des events ermittelt
        }

        public void eventManagerActionList()
        {

        }


    }
}
