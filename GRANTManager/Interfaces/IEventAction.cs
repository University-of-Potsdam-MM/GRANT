using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    public interface IEventAction
    {
        //Übergabe aller OSM-Bäume und -Verbindungen: OSM-Original, OSM-Braille - Verbindung OSMTreeConnection; OSM-Event OSM-Action und Verbindung zwischen diesen und OSM-Original/Braille: OSMNodeEventActionConnector
        void setGrantTrees(GeneratedGrantTrees trees);
        void refreshBrailleView(String viewId);
        void refreshBrailleScreen(String screenId);
        void changeBrailleScreen(String screenName);
        void changeWholeOSMElement();
    }
}
