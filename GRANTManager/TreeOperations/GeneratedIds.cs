using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;
using System.Security.Cryptography;
using System.Diagnostics;

namespace GRANTManager.TreeOperations
{
    public class GeneratedIds
    {
        StrategyManager strategyMgr;
        public GeneratedIds(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
        }

        /// <summary>
        /// Generiert für den kompletten Baum die Ids
        /// </summary>
        /// <param name="parentNode">gibt eine referenz zu dem Baum an</param>
        public void generatedIdsOfFilteredTree(ref Object tree)
        {
            Console.WriteLine("===== DEBUG BEGINN - HASH ======");
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.IdGenerated = generatedIdFilteredNode(node);
                    osm.properties = properties;
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                    if (properties.IdGenerated.Trim().Equals(""))
                    {
                        Debug.WriteLine("");
                    }
                }
            }
            Console.WriteLine("===== DEBUG ENDE - HASH ======");
        }

        /// <summary>
        /// Ermittelt und setzt die Ids in einem Teilbaum
        /// </summary>
        /// <param name="parentNode">gibt den Baum inkl. des Teilbaums ohne Ids an</param>
        /// <param name="idOfParent">gibt die Id des ersten Knotens des Teilbaums ohne Ids an</param>
      /*  public void generatedIdsOfFilteredSubtree(ref Object tree, String idOfParent)
        {
            //getFilteredTreeOsmElementById(idOfParent);
            Object subtree = getAssociatedNode(idOfParent, tree);
            if (subtree == null) { return; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.IdGenerated = generatedIdFilteredNode(node);
                    osm.properties = properties;
                    strategyMgr.getSpecifiedTree().SetData(node,osm);
                }
            }
            tree = strategyMgr.getSpecifiedTree().Root(subtree);
        }*/

        /// <summary>
        /// Ermittelt und setzt die Ids in einem Teilbaum
        /// </summary>
        /// <param name="subtree">gibt den Teilbaum an</param>
        /// <returns>Baum mit den veränderten Ids</returns>
        public Object generatedIdsOfFilteredSubtree(Object subtree)
        {
            //getFilteredTreeOsmElementById(idOfParent);
            //Object subtree = getAssociatedNode(idOfParent, tree);
            if (subtree == null) { return null ; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.IdGenerated = generatedIdFilteredNode(node);
                    osm.properties = properties;
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                }
            }
            return strategyMgr.getSpecifiedTree().Root(subtree);
        }

        /// <summary>
        /// Erstellt eine Id für ein Knoten des gefilterten Baums
        /// </summary>
        /// <param name="node">gibt den Knoten des gefilterten Baums an</param>
        /// <returns>die generierte Id</returns>
        private String generatedIdFilteredNode(Object node)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
            String result =
                properties.controlTypeFiltered +
                properties.itemTypeFiltered +
                properties.accessKeyFiltered +
                properties.acceleratorKeyFiltered +
                properties.frameWorkIdFiltered +
                properties.isContentElementFiltered +
                properties.isControlElementFiltered +
                properties.isKeyboardFocusableFiltered +
                properties.isPasswordFiltered +
                properties.isRequiredForFormFiltered +
                properties.itemStatusFiltered +
                properties.labeledbyFiltered +
                // node.BranchCount +
                strategyMgr.getSpecifiedTree().BranchIndex(node) +
                strategyMgr.getSpecifiedTree().Depth(node);

            if (strategyMgr.getSpecifiedTree().HasParent(node)) { result += strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Parent(node)).properties.IdGenerated; }
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(result));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            String tmpHash = String.Join(" : ", hash.Select(p => p.ToString()).ToArray());
            // Console.WriteLine("Node: name = {0}, \t controltype = {1}, \t hash[] = {2}, \t hashString = {3}, \t resultString = {4}", node.Data.properties.nameFiltered, node.Data.properties.controlTypeFiltered, tmpHash, sb.ToString(), result);
            return sb.ToString();
        }

        /// <summary>
        /// Erstellt die Id für einen Knoten des Braille-Baums
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public String generatedIdBrailleNode(OSMElement.OSMElement osmElement)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = osmElement.properties;
            BrailleRepresentation braille = osmElement.brailleRepresentation;
            String result = properties.controlTypeFiltered +
                braille.fromGuiElement +
                braille.screenName +
                braille.viewName +
                properties.boundingRectangleFiltered.ToString() +
                properties.controlTypeFiltered+
                (braille.screenCategory == null? "":braille.screenCategory) +
                (braille.uiElementSpecialContent == null? "": braille.uiElementSpecialContent.ToString());
          //  Debug.WriteLine("in generatedIdBrailleNode String für Hash = ", result);
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(result));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }


    }
}
