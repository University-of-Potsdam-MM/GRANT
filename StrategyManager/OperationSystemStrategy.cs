using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using StrategyManager.Interfaces;

namespace StrategyManager
{
    public class OperationSystemStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;

        public void setSpecifiedOperationSystem(String operationSystemName)
        {
            Type type = Type.GetType(operationSystemName);
            specifiedOperationSystem = (IOperationSystemStrategy)Activator.CreateInstance(type);

        }

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }

    }
}
