using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;

namespace StrategyManager.Interfaces
{
    public interface IBrailleDisplayStrategy
    {
        void initializedSimulator();
        void initializedBrailleDisplay();
        void generatedBrailleUi(ITreeStrategy<OSMElement.OSMElement> osm);
        void updateViewContent(OSMElement.OSMElement element);
        void setStrategyMgr(StrategyMgr strategyMgr);
        StrategyMgr getStrategyMgr();

        void updateNodeOfBrailleUi(OSMElement.OSMElement element, String filteredTreeGeneratedId);

    }
}
