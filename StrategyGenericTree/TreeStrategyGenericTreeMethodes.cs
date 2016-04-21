using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;

namespace StrategyGenericTree
{
    public class TreeStrategyGenericTreeMethodes
    {
        
        public enum OperatorEnum { and, or };

        #region search
        /// <summary>
        /// Sucht anhand der angegebenen Eigenschaften alle Knoten, welche der Bedingung entsprechen (Tiefensuche). Debei werden nur Eigenschften berücksichtigt, welche angegeben wurden.
        /// </summary>
        /// <param name="tree">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="properties">gibt alle zu suchenden Eigenschaften an</param>
        /// <param name="oper">gibt an mit welchem Operator (and, or) die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste aus <code>INodeTree</code>-Knoten mit den Eigenschaften <code>GeneralProperties</code> </returns>
        public List<ITreeStrategy<GeneralProperties>> searchProperties(ITreeStrategy<GeneralProperties> tree, GeneralProperties properties, OperatorEnum oper)
        {//TODO: hier fehlen noch viele Eigenschaften
            List<INode<GeneralProperties>> result = new List<INode<GeneralProperties>>();

            foreach (INode<GeneralProperties> node in ((ITree<GeneralProperties>)tree).All.Nodes)
            {
                Boolean propertieLocalizedControlType = properties.localizedControlTypeFiltered == null || node.Data.localizedControlTypeFiltered.Equals(properties.localizedControlTypeFiltered);
                Boolean propertieName = properties.nameFiltered == null || node.Data.nameFiltered.Equals(properties.nameFiltered);
                Boolean propertieIsEnabled = properties.isEnabledFiltered == null || node.Data.isEnabledFiltered == properties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = properties.boundingRectangleFiltered == new System.Windows.Rect() || node.Data.boundingRectangleFiltered.Equals(properties.boundingRectangleFiltered);

                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieIsEnabled && propertieLocalizedControlType && propertieName)
                    {
                        result.Add(node);
                    }
                }
                if (OperatorEnum.Equals(oper, OperatorEnum.or))
                {
                    if ((properties.localizedControlTypeFiltered != null && propertieLocalizedControlType) ||
                        (properties.nameFiltered != null && propertieName) ||
                        (properties.isEnabledFiltered != null && propertieIsEnabled) ||
                        (properties.boundingRectangleFiltered != new System.Windows.Rect()) && propertieBoundingRectangle)
                    {
                        result.Add(node);
                    }
                }
            }
            List<ITreeStrategy<GeneralProperties>> result2 = ListINodeToListINodeTree(result);
            printNodeList(result2);
            return result2;
        }

        #endregion

        private List<ITreeStrategy<GeneralProperties>> ListINodeToListINodeTree(List<INode<GeneralProperties>> list)
        {
            List<ITreeStrategy<GeneralProperties>> result = new List<ITreeStrategy<GeneralProperties>>();
            foreach (INode<GeneralProperties> node in list)
            {
                result.Add((ITreeStrategy<GeneralProperties>)node);
            }

            return result;
        }


        #region Print
        /// <summary>
        /// Gibt alle Knoten eines Baumes auf der Konsole aus.
        /// </summary>
        /// <param name="tree">gibt den Baum an</param>
        /// <param name="depth">gibt an bis in welche Tiefe die Knoten ausgegeben werden sollen</param>
        public static void printTreeElements(ITreeStrategy<GeneralProperties> tree, int depth)
        {
           foreach (ITreeStrategy<GeneralProperties> node in ((ITree<GeneralProperties>) tree).All.Nodes)
            {
                if (node.Depth <= depth || depth == -1)
                {
                    Console.Write("Node -  Anz. Kinder: {0},  Depth: {3},  Name: {1}, Type: {2},  hasNext: {4}, hasChild: {5}", node.DirectChildCount, node.Data.nameFiltered, node.Data.controlTypeFiltered, node.Depth, node.HasNext, node.HasChild);
                    Console.Write(", Position - Left: {0}, Right: {1}", node.Data.boundingRectangleFiltered.Left, node.Data.boundingRectangleFiltered.Right);

                    if (node.HasParent)
                    {
                        Console.Write(", Parent: {0}", node.Parent.Data.nameFiltered);
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
        private static void printNodeList(List<ITreeStrategy<GeneralProperties>> nodes)
        {
            foreach (ITreeStrategy<GeneralProperties> r in nodes)
            {
                Console.Write("Node - Name: {0}, Type: {1}, Depth: {2}, hasNext: {3}, hasChild: {4}, isEnabled: {5}", r.Data.nameFiltered, r.Data.controlTypeFiltered, r.Depth, r.HasNext, r.HasChild, !r.Data.isEnabledFiltered);
                if (r.HasParent)
                {
                    Console.Write(", Parent: {0}", r.Parent.Data.nameFiltered);
                }
                Console.WriteLine();
            }
        }
        #endregion

    }
}
