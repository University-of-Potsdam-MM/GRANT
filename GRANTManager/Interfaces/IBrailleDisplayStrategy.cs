using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;

namespace GRANTManager.Interfaces
{
    public interface IBrailleDisplayStrategy
    {
        /// <summary>
        /// Gibt an, ob die GUI und das Gerät initial erstellt wurde
        /// </summary>
        Boolean isInitialized();

        /// <summary>
        /// Initialisiert ein Ausgabegerät.
        /// </summary>
        void setActiveAdapter();

        /// <summary>
        /// Erstellt initial die Braille-UI
        /// </summary>
        void generatedBrailleUi();
        /// <summary>
        /// Ändert von dem angegebenen Element die Darstellung (Achtung: Momentan wird nur der Text geändert!)
        /// </summary>
        /// <param name="element">gibt das Element an, von welchem die Darstellung geändert werden soll</param>
        void updateViewContent(ref OSMElement.OSMElement element);

        /// <summary>
        /// Ermittelt zu einem Punkt den zugehörigen View-Name
        /// </summary>
        /// <param name="x">gibt die horizontale Position des Punktes auf der Stifftplatte an</param>
        /// <param name="y">gibt die vertikale Position des Punktes auf der Stifftplatte an</param>
        /// <returns>falls eine passende View gefunden wurde dessen Name; sonst <code>null</code></returns>
        String getBrailleUiElementViewNameAtPoint(int x, int y);// TODO: --> in ITreeOperation (unabhängig von BrailleIO)

        /// <summary>
        /// Gibt eine Liste mit möglichen Renderen zurück
        /// </summary>
        /// <returns>Liste der Renderer</returns>
        List<String> getUiElementRenderer();

        /// <summary>
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element an, welches für die Braille-UI beispielhaft gerendert werden soll</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        bool[,] getRendererExampleRepresentation(OSMElement.OSMElement osmElementFilteredNode);

        /// <summary>
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// Es wird eine Standardgröße genutzt
        /// </summary>
        /// <param name="uiElementType">gibt den Namen des Gui-Elements an</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        bool[,] getRendererExampleRepresentation(String uiElementType);

        /// <summary>
        /// Setzt den angegeben Screen auf Visible und alle anderen auf invisible
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an, der sichtbar sein soll</param>
        void setVisibleScreen(String screenName);

        /// <summary>
        /// Gibt den Namen des gerade sichtbaren Screens zurück;
        /// es sollte nur immer ein Screen sichtbar sein
        /// </summary>
        /// <returns>den Namen des Sichtbaren Screens oder <c>null</c></returns>
        String getVisibleScreen();

        void moveGroupViewRangHoricontal(ITreeStrategy<OSMElement.OSMElement> viewNode, int x);


        void setStrategyMgr(StrategyManager strategyMgr);
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
    }
}
