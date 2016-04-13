using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager.Interfaces;
using StrategyGenericTree;

namespace StrategyManager
{
    public class TreeStrategy
    {
        private ITreeStrategy<GeneralProperties> specifiedTree;

        public void setSpecifiedTree(String treeName)
        {
            //TODO;
        }

        public ITreeStrategy<GeneralProperties> getSpecifiedTree()
        {
            return specifiedTree;
        }
    }
}
