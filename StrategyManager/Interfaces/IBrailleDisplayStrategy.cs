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

        /// <summary>
        /// Ermittelt zu einem Punkt die Id des zugehörige Braille-UI-Elements
        /// </summary>
        /// <param name="x">gibt die horizontale Position des Punktes auf der Stifftplatte an</param>
        /// <param name="y">gibt die vertikale Position des Punktes auf der Stifftplatte an</param>
        /// <returns>falls ein passender Knoten gefunden wurde dessen generierte Id; sonst <code>null</code></returns>
        String getBrailleUiElementIdAtPoint(int x, int y);// TODO: --> in ITreeOperation (unabhängig von BrailleIO)

        /// <summary>
        /// Gibt eine Liste mit möglichen Renderen zurück
        /// </summary>
        /// <returns>Liste der Renderer</returns>
        List<String> getUiElementRenderer();

        /// <summary>
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element an, welches für die Braille-UI beispielhaft gerendert werden soll</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        bool[,] getRendererExampleRepresentation(OSMElement.OSMElement osmElement);


        void setStrategyMgr(StrategyMgr strategyMgr);
        StrategyMgr getStrategyMgr();




    }
}
