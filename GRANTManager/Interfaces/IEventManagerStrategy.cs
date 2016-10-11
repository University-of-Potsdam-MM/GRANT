using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;

namespace GRANTManager.Interfaces
{
    public interface IEventManagerStrategy
    {
        void setStrategyMgr(StrategyManager manager);
        //void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        //StrategyManager getStrategyMgr();

        string deliverString(); 
    }
}
