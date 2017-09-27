using GRANTManager.TreeOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    public interface IEventProcessor
    {
        void setGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperations(TreeOperation treeOperations);

    }
}
