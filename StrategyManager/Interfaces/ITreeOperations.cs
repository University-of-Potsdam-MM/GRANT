using System;
namespace StrategyManager.Interfaces
{
    public interface ITreeOperations<T>
    {
        /// <summary>
        /// Ändert die Eigenschaften eines Elementes der Braille-UI --> Momentan wird nur der anzuzeigende Text geändert!
        /// </summary>
        /// <param name="element">gibt das zuändernde Element an</param>
        void updateNodeOfBrailleUi(ref OSMElement.OSMElement element);

        /// <summary>
        /// Ändert die Eigenschaften eines Knotens des gefilterten Baumes.
        /// </summary>
        /// <param name="properties">Gibt die neuen Eigenschaften an.</param>
        void changePropertiesOfFilteredNode(OSMElement.GeneralProperties properties);

        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum einen zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder ein zugehöriger Knoten ermittelt werden soll</param>
        /// <param name="tree">gibt den Baum an, in welchem ein zugehöriger Knoten ermittelt werden soll</param>
        /// <returns>Gibt einen Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
       // StrategyManager.Interfaces.ITreeStrategy<T> getAssociatedNode(string idGenerated, StrategyManager.Interfaces.ITreeStrategy<T> tree);

        OSMElement.OSMElement getFilteredTreeOsmElementById(String idGenerated);
        OSMElement.OSMElement getBrailleTreeOsmElementById(String idGenerated);

        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum alle zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder die zugehörigen Knoten ermittelt werden sollen</param>
        /// <param name="tree">gibt den Baum an, in welchem die zugehörigen Knoten ermittelt werden sollen </param>
        /// <returns>Gibt eine Liste mit den Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
        System.Collections.Generic.List<StrategyManager.Interfaces.ITreeStrategy<T>> getAssociatedNodeList(string idGenereted, StrategyManager.Interfaces.ITreeStrategy<T> tree);

        /// <summary>
        /// Gibt die (einige) <code>GeneralProperties</code> des angegebenen Baumes aus
        /// </summary>
        /// <param name="tree">Gibt den auszugebenen Baum an</param>
        /// <param name="depth">Gibt die Tiefe der Ausgabe an; Wenn der gesamte Baum ausgegeben werden soll, so muss <value>-1</value> angegeben werden.</param>
        void printTreeElements(StrategyManager.Interfaces.ITreeStrategy<T> tree, int depth);

        /// <summary>
        /// Sucht anhand der angegebenen <code>GeneralProperties</code> alle Knoten die diesen Eigenschaften entsprechen
        /// </summary>
        /// <param name="tree">Gibt den Baum an, in welchem gesucht werden soll</param>
        /// <param name="properties">gibt die zusuchenden Eigenschaften an</param>
        /// <param name="oper">gibt an wie die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste mit allen Knoten auf den die Eigenschaften zutreffen</returns>
        System.Collections.Generic.List<StrategyManager.Interfaces.ITreeStrategy<T>> searchProperties(StrategyManager.Interfaces.ITreeStrategy<T> tree, OSMElement.GeneralProperties properties, StrategyManager.OperatorEnum oper);

        /// <summary>
        /// Fügt einen Knoten dem Baum der  Braille-Darstellung hinzu;
        /// Falls ein Knoten mit der 'IdGenerated' schon vorhanden sein sollte, wird dieser aktualisiert
        /// </summary>
        /// <param name="brailleNode">gibt die Darstellung des Knotens an</param>
        void addNodeInBrailleTree(OSMElement.OSMElement brailleNode);

        /// <summary>
        /// entfernt einen Knoten vom Baum der Braille-Darstellung
        /// </summary>
        /// <param name="brailleNode">gibt das OSM-element des Knotens der entfernt werden soll an</param>
        void removeNodeInBrailleTree(OSMElement.OSMElement brailleNode);

        /// <summary>
        /// Ermittelt zu einer View den zugehörigen Knoten
        /// </summary>
        /// <param name="viewName">gibt den Namen der View an</param>
        /// <returns></returns>
        OSMElement.OSMElement getNodeOfView(String viewName);

        /// <summary>
        /// Ermittelt zu einer View die Id des zugehörigen Knotens
        /// </summary>
        /// <param name="viewName">gibt den Namen der View an</param>
        /// <returns>falls der Knoten gefunden wurde, die generierte Id des Knotens; sonst <code>null</code> </returns>
        String getIdOfView(String viewName);


        void setStrategyMgr(StrategyManager.StrategyMgr mamager);
        StrategyManager.StrategyMgr getStrategyMgr();
    }
}
