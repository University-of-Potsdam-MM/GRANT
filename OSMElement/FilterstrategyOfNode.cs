using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    [Serializable]
    public class FilterstrategyOfNode<T, U, V>
    {
        public FilterstrategyOfNode() { }

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
