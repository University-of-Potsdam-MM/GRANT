using GRANTManager;
using GRANTManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyEvent
{
    
    public class EventProcessor : IEventProcessor
    {
        StrategyManager strategyManager;

        public EventProcessor(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
        }
    }
}
