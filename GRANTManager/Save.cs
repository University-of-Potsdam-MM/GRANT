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

        /// <summary>
        /// Speichert den gefilterten Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        public void saveFilteredTree(String filePath)
        {
            if (grantTree == null || grantTree.getFilteredTree() == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); }
            System.IO.FileStream fs = System.IO.File.Create(filePath);
            grantTree.getFilteredTree().XmlSerialize(fs);
            fs.Close();
        }
    }
}
