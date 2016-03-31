using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree;


namespace FilterBase
{
    public class BasicFilter
    {
        public enum OperatorEnum { and, or };

        //default = UIA
        private Interfaces.IFilter specifiedFilter;
        public ITree<GeneralProperties> filtering(IntPtr hwnd){
            return specifiedFilter.filtering(hwnd);
        }

        public void setSpecifiedFilter(Interfaces.IFilter filter)
        {
            specifiedFilter = filter;
        }
        

        #region search
        /// <summary>
        /// Sucht anhand des ControlTypes alle Elemente im Baum, die diesen ControlType besitzen.
        /// </summary>
        /// <param name="tree">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="controlType">gibt den ControlType an</param>
        /// <returns>Eine Liste aus <code>INode</code>-Knoten mit den Eigenschaften <code>GeneralProperties</code> </returns>
        public List<INode<GeneralProperties>> searchControlType(ITree<GeneralProperties> tree, String controlType)
        {
            List<INode<GeneralProperties>> result = new List<INode<GeneralProperties>>();
            foreach (INode<GeneralProperties> node in tree.All.Nodes)
            {
                if (node.Data.LocalizedControlType.Equals(controlType))
                {
                    result.Add(node);
                }
            }
            printNodeList(result);
            return result;
        }

        /// <summary>
        /// Sucht anhand der angegebenen Eigenschaften alle Knoten, welche der Bedingung entsprechen (Tiefensuche). Debei werden nur Eigenschften berücksichtigt, welche angegeben wurden.
        /// </summary>
        /// <param name="tree">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="properties">gibt alle zu suchenden Eigenschaften an</param>
        /// <param name="oper">gibt an mit welchem Operator (and, or) die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste aus <code>INode</code>-Knoten mit den Eigenschaften <code>GeneralProperties</code> </returns>
        public List<INode<GeneralProperties>> searchProperties(ITree<GeneralProperties> tree, GeneralProperties properties, OperatorEnum oper)
        {
            List<INode<GeneralProperties>> result = new List<INode<GeneralProperties>>();

            foreach (INode<GeneralProperties> node in tree.All.Nodes)
            {
                Boolean propertieLocalizedControlType = properties.LocalizedControlType == null || node.Data.LocalizedControlType.Equals(properties.LocalizedControlType);
                Boolean propertieName = properties.Name == null || node.Data.Name.Equals(properties.Name);
                Boolean propertieIsEnabled = properties.IsEnabled == null || node.Data.IsEnabled == properties.IsEnabled;
                Boolean propertieBoundingRectangle = properties.BoundingRectangle == new System.Windows.Rect() || node.Data.BoundingRectangle.Equals(properties.BoundingRectangle);

                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieIsEnabled && propertieLocalizedControlType && propertieName)
                    {
                        result.Add(node);
                    }
                }
                if (OperatorEnum.Equals(oper, OperatorEnum.or))
                {
                    if ((properties.LocalizedControlType != null && propertieLocalizedControlType) ||
                        (properties.Name != null && propertieName) ||
                        (properties.IsEnabled != null && propertieIsEnabled) ||
                        (properties.BoundingRectangle != new System.Windows.Rect()) && propertieBoundingRectangle)
                    {
                        result.Add(node);
                    }
                }
            }
            printNodeList(result);
            return result;
        }
        #endregion

        #region change tree
        public void updateNode (INode<GeneralProperties> nodeOld, GeneralProperties nodeNewProperties)
        {
            nodeOld.Data = nodeNewProperties;           
        }

        public void insertChildreen(INode<GeneralProperties> node, GeneralProperties childNew)
        {//Insert = Position 0
            node.InsertChild(childNew);
        }

        public void insertSubTree(INode<GeneralProperties> node, ITree<GeneralProperties> subTree)
        {
            //node.AddChild(subTree); // Add = hinten
            node.InsertChild(subTree);
        }

        #endregion

        #region Print
        /// <summary>
        /// Gibt alle Knoten eines Baumes auf der Konsole aus.
        /// </summary>
        /// <param name="tree">gibt den Baum an</param>
        /// <param name="depth">gibt an bis in welche Tiefe die Knoten ausgegeben werden sollen</param>
        public void printTreeElements(ITree<GeneralProperties> tree, int depth)
        {
            Console.WriteLine();
            foreach (INode<GeneralProperties> node in tree.All.Nodes)
            {
                if (node.Depth <= depth || depth == -1)
                {
                    Console.Write("Node -  Anz. Kinder: {0},  Depth: {3},  Name: {1}, Type: {2},  hasNext: {4}, hasChild: {5}", node.DirectChildCount, node.Data.Name, node.Data.LocalizedControlType, node.Depth, node.HasNext, node.HasChild);
                    Console.Write(", Position - Left: {0}, Right: {1}", node.Data.BoundingRectangle.Left, node.Data.BoundingRectangle.Right);
                    
                    if (node.HasParent)
                    {
                        Console.Write(", Parent: {0}", node.Parent.Data.Name);
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Gibt eine Liste von Knoten auf der Konsole aus.
        /// </summary>
        /// <param name="nodes">gibt die Liste der auszugebenden Knoten an</param>
        private void printNodeList(List<INode<GeneralProperties>> nodes)
        {
            foreach (INode<GeneralProperties> r in nodes)
            {
                Console.Write("Node - Name: {0}, Type: {1}, Depth: {2}, hasNext: {3}, hasChild: {4}, isEnabled: {5}", r.Data.Name, r.Data.LocalizedControlType, r.Depth, r.HasNext, r.HasChild, r.Data.IsEnabled);
                if (r.HasParent)
                {
                    Console.Write(", Parent: {0}", r.Parent.Data.Name);
                }
                Console.WriteLine();
            }
        }
        #endregion

        #region export/import
        /// <summary>
        /// Exportiert einen Baum in eine XML-Datei
        /// </summary>
        /// <param name="tree">gibt den zu exportierenden Baum an</param>
        /// <param name="path">gibt den Pfad an, wo der Baum gespeichert werden soll (inkl. Dateinamen)</param>
        public void exportTreeToXmlFile(ITree<GeneralProperties> tree, String path)
        { //TODO: Fehlerbehandlung: Datei existiert schon; Pfad existiert nicht, ...
            System.IO.FileStream fs = System.IO.File.Create(path);
            tree.XmlSerialize(fs);
            fs.Close();
        }

        /// <summary>
        /// Importiert einen Baum aus einer XML-Datei.
        /// </summary>
        /// <param name="path">gibt den Pfad inkl. des Dateinamens von wo der Baum importiert werden soll an.</param>
        /// <returns>Ein objekt des Importierten Baumes</returns>
        public ITree<GeneralProperties> importTreeFromXmlFile(String path)
        { //TODO: Fehlerbehandlung: Datei existiert nicht; Pfad existiert nicht, ...
            System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read) ;

            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.XmlDeserialize(fs);
            fs.Close();
            printTreeElements(tree, -1);
            return tree;
        }
        #endregion
    }
}
