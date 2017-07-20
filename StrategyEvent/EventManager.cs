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

    }
}
