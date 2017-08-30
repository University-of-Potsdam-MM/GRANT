﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager.TreeOperations;


namespace GRANTManager.Interfaces
{
    public interface IEventManager
    {
        void deliverActionListForEvent(string eventID);

        void setGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperations(TreeOperation treeOperations);

        void EventExample();

    }
}
