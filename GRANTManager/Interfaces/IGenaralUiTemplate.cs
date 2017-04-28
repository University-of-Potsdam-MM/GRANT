using System;
using System.Collections.Generic;
using GRANTManager.TreeOperations;
using System.Windows;

namespace GRANTManager.Interfaces
{
    public interface IGenaralUiTemplate
    {
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperation(TreeOperation treeOperation);

        /// <summary>
        /// Adds a navigatiobar for the screen
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <param name="screenName">name of the screen on wich the navigation bar should be added</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bar should be added</param>
        void addNavigationbarForScreen(string pathToXml, String screenName, String typeOfView);

        /// <summary>
        /// Creates navigation bars for every screen in this tpye of view
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bars should be added</param>
        void createNavigationbar(string pathToXml, String typeOfView);

        /// <summary>
        /// Generates Ui elements depending on a template file
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        void generatedUiFromTemplate(string pathToXml);

        /// <summary>
        /// Generates Ui elements for the layout view depending on a template file
        /// </summary>
        /// <param name="pathToTemplate">path of the used template (XML)</param>
        void generatedLayoutView(String pathToTemplate);

        /// <summary>
        /// Generates Ui elements for the symbol view depending on a template file
        /// </summary>
        /// <param name="pathToTemplate">path of the used template (XML)</param>
        void generatedSymbolView(String pathToTemplate);

        /// <summary>
        /// Updates all navigation bars in this type of view
        /// </summary>
        /// <param name="pathToXml">path of the used template(XML)</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bar should be updated</param>
        void updateNavigationbarScreens(string pathToXml, String typeOfView);

        /// <summary>
        /// Adds all elements (of the subtree) as Sysmbols to the braille tree
        /// </summary>
        /// <param name="subtree">subtree to add as Symbols</param>
        /// <param name="lastRect">position of the last UI element which was added</param>
        /// <param name="idToIgnore">a list of all (ids of) elements which should NOT be added as symbol</param>
        void allElementsAsSymbols(Object subtree, ref Rect lastRect, String[] idToIgnore = null);

        /// <summary>
        /// Creates all UI elements for the Braille tree which are specified in the template
        /// </summary>
        /// <param name="filteredSubtree">the filtered (sub-)tree</param>
        /// <param name="templateObject">the template object for the group to created</param>
        /// <param name="brailleNodeId">Id of the parent element of the group</param>
        void createUiElementFromTemplate(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null);
    }
    public struct TemplateUiObject
    {
        public OSMElement.OSMElement osm { get; set; }
        public String groupImplementedClassTypeFullName { get; set; } //nötig?
        public String groupImplementedClassTypeDllName { get; set; } //nötig?
        /// <summary>
        /// A List of screen names (different to <see cref="OSMElement.BrailleRepresentation"/>)
        /// </summary>
        public List<String> Screens { get; set; } //-> neu, da es nicht mit Screen in OSM (BR) zusammenpasst

        /// <summary>
        /// The name of the view
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Determines whether all elements of this type should be added to the Braille tree; if this value <c>false</c> only the first occurrence will be used
        /// </summary>
        public Boolean allElementsOfType { get; set; }

        /// <summary>
        /// Specifies the orientation of the element
        /// </summary>
        public OSMElement.UiElements.Orientation orientation { get; set; }

        /// <summary>
        /// Specifies the elements which build a group; e.g. the tabs in the menu bar are a group; The number of elements can be changed during runtime
        /// </summary>
        public List<OSMElement.OSMElement> groupElements { get; set; }
    }

    public struct TemplateScreenshotObject
    {
        public OSMElement.OSMElement osm { get; set; }
        public List<String> Screens { get; set; }
        public String viewName { get; set; }
        /// <summary>
        /// Specifies the control type of the filtered node object from which to create a screenshot
        /// </summary>
        public String connectedFilteredNodeControltype { get; set; } 
    }

}
