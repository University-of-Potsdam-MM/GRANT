using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;
using GRANTManager.TreeOperations;

namespace GRANTManager.Interfaces
{
    public interface IBrailleDisplayStrategy
    {
        /// <summary>
        /// Specifies whether a braille display was initirized
        /// </summary>
        Boolean isInitialized();

        /// <summary>
        /// Initialized a braille display.
        /// </summary>
        void setActiveAdapter();

        /// <summary>
        /// Removes the active adapter
        /// </summary>
        void removeActiveAdapter();

        /// <summary>
        /// generated the tactile user interface of a braille device
        /// </summary>
        void generatedBrailleUi();

        /// <summary>
        /// Updated the content of a specific element
        /// </summary>
        /// <param name="element">element which content should be updated</param>
        void updateViewContent(ref OSMElement.OSMElement element);

        /// <summary>
        /// Seeks to a point the associated view name.
        /// </summary>
        /// <param name="x">x coordinat of the point on the braille display</param>
        /// <param name="y">y coordinat of the point on the braille display</param>
        /// <param name="offsetX">x offste of the view</param>
        /// <param name="offsetY">y offste of the view</param>
        /// <returns>the name of the view or <code>null</code></returns>
        String getBrailleUiElementViewNameAtPoint(int x, int y, out int offsetX, out int offsetY);// TODO: --> in ITreeOperation (unabhängig von BrailleIO)

        /// <summary>
        /// Gives a list of all possible renderer depending on the choosen device
        /// </summary>
        /// <returns>list of all possible renderer</returns>
        List<String> getUiElementRenderer();

        /// <summary>
        /// list of ALL possible renderer
        /// </summary>
        /// <returns>list of all possible renderer</returns>
        List<String> getAllUiElementRenderer();

        /// <summary>
        /// Gives a example tactile representation to a given tree object (node)
        /// </summary>
        /// <param name="osmElementFilteredNode">a node</param>
        /// <returns>Boolean matrix where <code>true</code> represents a shown pin</returns>
        bool[,] getRendererExampleRepresentation(OSMElement.OSMElement osmElementFilteredNode);

        /// <summary>
        /// Gives a example tactile representation to a controll type
        /// </summary>
        /// <param name="uiElementType">controll type of the UI element</param>
        /// <returns>Boolean matrix where <code>true</code> represents a shown pin</returns>
        bool[,] getRendererExampleRepresentation(String uiElementType);

        /// <summary>
        /// Sets the given screen on "visible" and all others of "invisible".
        /// </summary>
        /// <param name="screenName">the name of the screen which should be visible</param>
        void setVisibleScreen(String screenName);

        /// <summary>
        /// Returns the name of the visible screen
        /// only one screen (per time) should by visible
        /// </summary>
        /// <returns>name of the visible screen or <c>null</c></returns>
        String getVisibleScreen();

        /// <summary>
        /// moves a (group of) view(s) horizontal
        /// </summary>
        /// <param name="viewNode">the name of the view</param>
        /// <param name="steps">steps to move right</param>
        void moveViewRangHoricontal(Object viewNode, int steps);

        /// <summary>
        /// moves a (group of) view(s) vertical
        /// </summary>
        /// <param name="viewNode">the name of the view</param>
        /// <param name="steps">steps to move left</param>
        void moveViewRangVertical(Object viewNode, int steps);


        void setStrategyMgr(StrategyManager strategyMgr);
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperation(TreeOperation treeOperation);

        /// <summary>
        /// Gives position and offset of a node
        /// </summary>
        /// <param name="brailleNode">node of the braille (output) tree</param>
        /// <returns>position and offset of a node</returns>
        TactileNodeInfos getTactileNodeInfos(Object brailleNode);

        Type getActiveAdapter();
        void removeAllViews();
    }
    /// <summary>
    /// Structure for position and offset of a view
    /// </summary>
    public struct TactileNodeInfos
    {
        public int contentHeight { get; set; }
        public int contentWidth { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
    }
}
