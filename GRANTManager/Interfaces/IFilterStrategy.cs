using System;
using GRANTManager;
using OSMElement;
using System.Windows;
using System.Windows.Automation;
using GRANTManager.TreeOperations;

namespace GRANTManager.Interfaces
{
    /// <summary>
    /// An Interface for filtering application data. 
    /// </summary>
    public interface IFilterStrategy
    {
        #region filtering
        /// <summary>
        /// filters an application depending on a given point (<paramref name="pointX"/>, <paramref name="pointY"/>) and the choosen <paramref name="treeScope"/>
        /// </summary>
        /// <param name="pointX">x coordinate of the element to filtering</param>
        /// <param name="pointY">y coordinate of the element to filtering</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Parent', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <returns>the filtered (sub-)tree</returns>
        Object filtering(int pointX, int pointY, GRANTManager.TreeScopeEnum treeScope, int depth = -1);

        /// <summary>
        /// filters an application depending on a given hwnd
        /// </summary>
        /// <param name="hwnd">the process handle of applicatio/element</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Parent', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <returns>the filtered (sub-)tree</returns>
        Object filtering(IntPtr hwnd, TreeScopeEnum treeScope = TreeScopeEnum.Application, int depth = -1);

        /// <summary>
        /// filters an application depending on a given OSM element
        /// </summary>
        /// <param name="osmElementOfFirstNodeOfSubtree">osm element of the to filtered application</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <returns>the filtered (sub-)tree</returns>
        Object filtering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScope = TreeScopeEnum.Subtree);

        Object filtering(String generatedNodeId, TreeScopeEnum treeScope);
        #endregion

        /// <summary>
        /// Seeks the process id of a given handle
        /// </summary>
        /// <param name="hwnd">the process handle of an element of a application</param>
        /// <returns>the process id</returns>
        int deliverElementID(IntPtr hwnd);

        /// <summary>
        /// Seeks an OSM element to a given point
        /// </summary>
        /// <param name="pointX">x coordinate of the element to filtering</param>
        /// <param name="pointY">y coordinate of the element to filtering</param>
        /// <returns>the OSM element of the point</returns>
        OSMElement.OSMElement getOSMElement(int pointX, int pointY);

        /// <summary>
        /// Seeks new data for a node
        /// </summary>
        /// <param name="osmElementFilteredNode">OSM element to update</param>
        /// <returns>new properties for a node</returns>
        GeneralProperties updateNodeContent(OSMElement.OSMElement osmElementFilteredNode);

        void setStrategyMgr(StrategyManager manager);
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperation(TreeOperation treeOperation);
        StrategyManager getStrategyMgr();
        
    };
}
