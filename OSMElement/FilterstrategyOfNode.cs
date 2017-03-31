using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Filter strategy  of node
    /// </summary>
    /// <typeparam name="T">generated id of a filtered node</typeparam>
    /// <typeparam name="U">name of the filter strategy e.g. 'StrategyUIA.FilterStrategyUIA' </typeparam>
    /// <typeparam name="V">DLL name of the filter strategy e.g. 'StrategyUIA'</typeparam>
    [Serializable]
    public class FilterstrategyOfNode<T, U, V>
    {
        public FilterstrategyOfNode() { }

        /// <summary>
        /// Capsules the filter strategy of a node
        /// </summary>
        /// <param name="idGenerated">generated id of a filtered node</param>
        /// <param name="strategyName">name of the filter strategy e.g. 'StrategyUIA.FilterStrategyUIA'</param>
        /// <param name="strategyDll">DLL name of the filter strategy e.g. 'StrategyUIA'</param>
        public FilterstrategyOfNode(T idGenerated, U strategyName, V strategyDll)
        {
            this.IdGenerated = idGenerated;
            this.FilterstrategyFullName = strategyName;
            this.FilterstrategyDll = strategyDll;
        }

        public T IdGenerated { get; set; }

        public U FilterstrategyFullName { get; set; }

        public V FilterstrategyDll { get; set; }
    }
}
