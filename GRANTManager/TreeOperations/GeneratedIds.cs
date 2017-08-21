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
        private StrategyManager strategyMgr;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedIds"/> class.
        /// </summary>
        /// <param name="strategyMgr"></param>
        public GeneratedIds(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
        }
        #endregion
       
        /// <summary>
        /// Generates ids for the whole filtered tree.
        /// </summary>
        /// <param name="parentNode">reference to the filtered tree object</param>
        public void generatedIdsOfFilteredTree(ref Object tree)
        {
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                     osm.properties.IdGenerated = generatedIdFilteredNode(node);
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                }
            }
        }

        /// <summary>
        /// Calculates and sets the ids into a subtree of the filtered tree.
        /// </summary>
        /// <param name="subtree">subtree object</param>
        /// <returns>tree object with new ids</returns>
        public Object generatedIdsOfFilteredSubtree(Object subtree)
        {
            //getFilteredTreeOsmElementById(idOfParent);
            //Object subtree = getNode(idOfParent, tree);
            if (subtree == null) { return null ; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    osm.properties.IdGenerated = generatedIdFilteredNode(node);
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                }
            }
            return strategyMgr.getSpecifiedTree().Root(subtree);
        }

        /// <summary>
        /// Generates an id for one node of the filtered tree.
        /// </summary>
        /// <param name="node">one filtered node</param>
        /// <returns>the generated id</returns>
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
                properties.labeledByFiltered +
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
            return sb.ToString();
        }


        internal String generatedIdFilteredNode(object node, int depth, int branchIndex, String parentId)
        {
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
                properties.labeledByFiltered +
                // node.BranchCount +
                branchIndex +
                depth;

            if (parentId != null) { result += parentId; }
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
            return sb.ToString();
        }

        /// <summary>
        /// Generates an id for one node of the braille (output) tree.
        /// </summary>
        /// <param name="node">one node of the braille tree</param>
        /// <returns>the generated id</returns>
        public String generatedIdBrailleNode(OSMElement.OSMElement osmElement)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = osmElement.properties;
            BrailleRepresentation braille = osmElement.brailleRepresentation;
            String result = properties.controlTypeFiltered +
                braille.displayedGuiElementType +
                braille.screenName +
                braille.viewName +
                properties.boundingRectangleFiltered.ToString() +
                properties.controlTypeFiltered+
                (braille.typeOfView == null? "":braille.typeOfView) +
                (braille.uiElementSpecialContent == null? "": braille.uiElementSpecialContent.ToString());
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

        /// <summary>
        /// Generates an id for an <c>OSMEvent</c> object
        /// </summary>
        /// <param name="osmEvent">an <c>OSMEvent</c> object</param>
        /// <returns>the generated id</returns>
        public String generatedIdOsmEvent(OSMEvent osmEvent)
        {
            String result = osmEvent.Name + osmEvent.Type.ToString();
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
            return sb.ToString();
        }

        /// <summary>
        /// Generates an id for an <c>OSMAction</c> object
        /// </summary>
        /// <param name="osmAction">an <c>OSMAction</c> object</param>
        /// <returns>the generated id</returns>
        public String generatedIdOsmAction(OSMAction osmAction)
        {
            String result = osmAction.Name + osmAction.Type.ToString();
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
            return sb.ToString();
        }
    }
}
