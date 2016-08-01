using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;

namespace GRANTManager
{
    public class Save
    {
        GeneratedGrantTrees grantTree;

        public Save(GeneratedGrantTrees grantTree) { this.grantTree = grantTree; }
        public void saveFilteredTree(String filePath)
        {
            System.IO.FileStream fs = System.IO.File.Create(filePath);
            grantTree.getFilteredTree().XmlSerialize(fs);
            fs.Close();
        }
    }
}
