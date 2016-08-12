using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElement;
using GRANTManager.Interfaces;

namespace GRANTManager
{
    public class FilterstrategiesOfTree
    {
        public static FilterstrategyOfNode<String, String, String> getMainFilterstrategyOfTree(ITreeStrategy<OSMElement.OSMElement> filteredTree, List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            if (filteredTree == null || !filteredTree.HasChild)
            {
                Debug.WriteLine("Angaben im Baum fehlen!");
                return null;
            }
            return getFilterstrategyOfNode(filteredTree.Child.Data.properties.IdGenerated, filterstrategies);
        }

        public static bool addFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            if (idGeneratedOfNode == null || idGeneratedOfNode.Equals("")) { Debug.WriteLine("Keine Id vorhanden - Strategy konnte nicht gesetzt werden!"); return false; }
            FilterstrategyOfNode<String, String, String> filterstrategyNew = new FilterstrategyOfNode<string, string, string>();
            filterstrategyNew.IdGenerated = idGeneratedOfNode;
            filterstrategyNew.FilterstrategyFullName = filterstrategyType.FullName;
            filterstrategyNew.FilterstrategyDll = filterstrategyType.Namespace;
            if (!filterstrategies.Contains(filterstrategyNew))
            {
                //Prüfen, ob für diesen Knoten eine andere Beziehung gesetzt wurde und ggf. löschen
                 FilterstrategyOfNode<String, String, String> filterstrategyOld = getFilterstrategyOfNode(idGeneratedOfNode, filterstrategies);
                 if (filterstrategyOld != null)
                 {
                     filterstrategies.Remove(filterstrategyOld);
                 }
                 filterstrategies.Add(filterstrategyNew);
                 return true;
            }
            Debug.WriteLine("Die Beziehung existiert so schon!");
            return false;
        }


        public static void removeFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            FilterstrategyOfNode<String, String, String> filterstrategyOld = getFilterstrategyOfNode(idGeneratedOfNode, filterstrategies);
            if (filterstrategyOld != null)
            {
                filterstrategies.Remove(filterstrategyOld);
            }
        }

        public static FilterstrategyOfNode<String, String, String> getFilterstrategyOfNode(String idGeneratedOfNode, List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            if (filterstrategies == null || filterstrategies.Equals(new List<FilterstrategyOfNode<String, String, String>>()) || filterstrategies.Count == 0 || idGeneratedOfNode.Equals("")) { return null; }

            FilterstrategyOfNode<String, String, String> filterstrategyFound = filterstrategies.Find(r => r.IdGenerated.Equals(idGeneratedOfNode));
            if (!(filterstrategyFound == null || filterstrategyFound.Equals(new FilterstrategyOfNode<String, String, String>())))
            {
                return filterstrategyFound;
            }

            return null;
        }


    }
}
