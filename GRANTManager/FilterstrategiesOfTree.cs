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
        public static FilterstrategyOfNode<String, String, String> getMainFilterstrategyOfTree(Object filteredTree, List<FilterstrategyOfNode<String, String, String>> filterstrategies, ITreeStrategy<OSMElement.OSMElement> treeStrategy)
        {
            if (filteredTree == null || !treeStrategy.HasChild(filteredTree))
            {
                Debug.WriteLine("Angaben im Baum fehlen!");
                return null;
            }
            return getFilterstrategyOfNode(treeStrategy.GetData(treeStrategy.Child(filteredTree)).properties.IdGenerated, filterstrategies);
        }

        /// <summary>
        /// Fügt für diesen Knoten eine Zuordnung der Filterstrategie hinzu
        /// </summary>
        /// <param name="idGeneratedOfNode">gibt die ID des Knotens an</param>
        /// <param name="filterstrategyType">gibt die für den Knoten zu nutzende Filterstrategie an</param>
        /// <param name="filterstrategies">gibt eine Liste mit der Zuordnung der Filterstrategien an</param>
        /// <returns><c>true</c> falls die Filterstrategie für den Knoten hinzugefügt wurde; sonst <c>false</c></returns>
        public static bool addFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            //TODO: prüfen, ob der Standardfilter genutzt wird
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

        /// <summary>
        /// Entfernt für diesen Knoten die Filterstrategie
        /// </summary>
        /// <param name="idGeneratedOfNode">gibt die ID des Knotens an</param>
        /// <param name="filterstrategyType">gibt die für den Knoten zu nutzende Filterstrategie an</param>
        /// <param name="filterstrategies">gibt eine Liste mit der Zuordnung der Filterstrategien an</param>
        /// <returns><c>true</c> falls die Filterstrategie entfernt wurde; sonst <c>false</c></returns>
        public static bool removeFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            FilterstrategyOfNode<String, String, String> filterstrategyOld = getFilterstrategyOfNode(idGeneratedOfNode, filterstrategies);
            if (filterstrategyOld != null)
            {
                filterstrategies.Remove(filterstrategyOld);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gibt die filterstrategie von einen Knoten an
        /// </summary>
        /// <param name="idGeneratedOfNode">gibt die ID des Knotens an</param>
        /// <param name="filterstrategies">gibt eine Liste mit der Zuordnung der Filterstrategien an</param>
        /// <returns>ein<c>FilterstrategyOfNode</c>-objekt mit der für den Knoten verwendeten Filterstrategie</returns>
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
