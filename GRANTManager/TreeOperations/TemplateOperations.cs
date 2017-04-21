using OSMElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.TreeOperations
{
    public class TemplateOperations
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;

        public TemplateOperations(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        /// <summary>
        /// Ermittelt, sofern vorhanden, für einen Controlltype die anzuzeigenden Eigenschaften sowie deren Reihenfolge
        /// </summary>
        /// <param name="controltype">gibt den Controlltype an</param>
        /// <returns>ein <c>SpecialOrder</c>-Objekt mit der Reihenfolge der anzuzeigenden Eigenschaften  oder <c>null</c></returns>
        public SpecialOrder textViewspecialOrderContainsControltype(String controltype)
        {
            if (grantTrees.TextviewObject.orders.specialOrders != null)
            {
                foreach (SpecialOrder so in grantTrees.TextviewObject.orders.specialOrders)
                {
                    if (so.controltypeName.Equals(controltype))
                    {
                        return so;
                    }
                }
            }
            return new SpecialOrder();
        }
    }
}
