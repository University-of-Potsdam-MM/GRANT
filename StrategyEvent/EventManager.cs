using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;

namespace StrategyEvent
{
    public class EventManager : IEventManager
    {
        private StrategyManager strategyManager;

        public EventManager(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
        }

        //erhalt der subscriptions auf ein event
        //https://msdn.microsoft.com/en-us/library/microsoft.practices.prism.pubsubevents.eventbase.subscriptions(v=pandp.50).aspx
        //https://stackoverflow.com/questions/45410678/eventaggregator-get-list-of-subscribers

        public void deliverActionListForEvent(string eventID)
        {

        }


    }
}
