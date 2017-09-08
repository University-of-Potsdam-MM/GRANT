using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    /// <summary>
    /// Structure for saving a Grant project.
    /// </summary>
    [Serializable]
    public struct GrantProjectObject
    {
        /// <summary>
        /// the fully qualified name of the Braille Strategy type, including its namespace but not its assembly.
        /// </summary>
        public String grantBrailleStrategyFullName { get; set; }

        /// <summary>
        /// The namespace of the Braille Strategy System.Type.
        /// </summary>
        public String grantBrailleStrategyNamespace { get; set; }

        /// <summary>
        /// the fully qualified name of the Display Strategy type, including its namespace but not its assembly.
        /// </summary>
        public String grantDisplayStrategyFullName { get; set; }

        /// <summary>
        /// The namespace of the Display Strategy System.Type.
        /// </summary>
        public String grantDisplayStrategyNamespace { get; set; }

        /// <summary>
        /// the fully qualified name of the Tree Strategy type, including its namespace but not its assembly.
        /// </summary>
        public String grantTreeStrategyFullName { get; set; }

        /// <summary>
        /// The namespace of the Tree Strategy System.Type.
        /// </summary>
        public String grantTreeStrategyNamespace { get; set; }

        /// <summary>
        /// the fully qualified name of the Tree Operations Strategy type, including its namespace but not its assembly.
        /// </summary>
        public String grantTreeOperationsFullName { get; set; }

        /// <summary>
        /// The namespace of the Tree Operations Strategy System.Type.
        /// </summary>
        public String grantTreeOperationsNamespace { get; set; }

        //public String grantFilterStrategyFullName { get; set; } -> already in the filtered tree
        //public String grantFilterStrategyNamespace { get; set; } -> already in the filtered tree

        /// <summary>
        /// the fully qualified name of the Operating System Strategy type, including its namespace but not its assembly.
        /// </summary>
        public String grantOperationSystemStrategyFullName { get; set; }

        /// <summary>
        /// The namespace of the Operating System Strategy System.Type.
        /// </summary>
        public String grantOperationSystemStrategyNamespace { get; set; }

        public String grantExternalScreenreaderFullName { get; set; }
        public String grantExternalScreenreaderNamespace { get; set; }

        public String grantBrailleConverterFullName { get; set; }
        public String grantBrailleConverterNamespace { get; set; }

        public String grantEventActionFullName { get; set; }
        public String grantEventManagerFullName { get; set; }
        public String grantEventProcessorFullName { get; set; }
        public String grantEventActionNamespace { get; set; }
        public String grantEventManagerNamespace { get; set; }
        public String grantEventProcessorNamespace { get; set; }

        /// <summary>
        /// The choosen braille display
        /// </summary>
        public Device device { get; set; }

    }
}
