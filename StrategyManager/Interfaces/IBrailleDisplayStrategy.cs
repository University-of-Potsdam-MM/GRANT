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
        /// <summary>
        /// Erstellt ein Simulator für die Braille-Ausgabe
        /// </summary>
        void initializedSimulator();

        /// <summary>
        /// Initialisiert ein "reales" Ausgabegerät.
        /// </summary>
        void initializedBrailleDisplay();

        /// <summary>
        /// Erstellt initial die Braille-UI
        /// </summary>
        void generatedBrailleUi();
        /// <summary>
        /// Ändert von dem angegebenen Element die Darstellung (Achtung: Momentan wird nur der Text geändert!)
        /// </summary>
        /// <param name="element">gibt das Element an, von welchem die Darstellung geändert werden soll</param>
        void updateViewContent(OSMElement.OSMElement element);


        void setStrategyMgr(StrategyMgr strategyMgr);
        StrategyMgr getStrategyMgr();




    }
}
