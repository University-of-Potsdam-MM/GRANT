using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GRANTManager
{
    public class ScreenReaderFunctions
    {
        public Dictionary<String, String> screenreaders;
        StrategyManager strategyMgr;
        //Thread activeApp;

        public ScreenReaderFunctions(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            screenreaders = new Dictionary<string, string>();
           // activeApp = new Thread(delegate () { loadScreeanReaderActiveApp(); });
            loadScreenReaderHash();
        }

        private void loadScreeanReaderActiveApp()
        {
            Thread.Sleep(3000);
            IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().getForegroundWindow();
            String name = strategyMgr.getSpecifiedOperationSystem().gerProcessNameOfApplication((int)hwnd);

            String file = getScreenReaderFile(name);
        }

        private void loadScreenReaderHash()
        {
            String directory = Settings.getScreenReaderDirectory();
            if (directory == null || !Directory.Exists(@directory))
            {
                Debug.Print("Directory can't be found!");
                return;
            }
            string[] srFiles = Directory.GetFiles(@directory, "*.grant");
            foreach (string sr in srFiles)
            {
                String projectDirectory = Path.GetDirectoryName(@sr) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@sr);

                String processName = getProcessName(@projectDirectory + Path.DirectorySeparatorChar + Settings.getFilteredTreeSavedName());
                if (processName != null && !processName.Equals(""))
                {
                    if (!screenreaders.ContainsKey(processName))
                    {
                        screenreaders.Add(processName, sr);
                    }
                }
            }
        }

        private String getProcessName(String filePath)
        {
            if (!File.Exists(@filePath))
            {
                Debug.WriteLine("The file don't exist!");
                return null;
            }
            using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                Object loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
                return strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(loadedTree)).properties.processName;
            }
        }

        private String getScreenReaderFile(String appName)
        {
            if (appName != null && screenreaders.ContainsKey(appName))
            {
                return screenreaders[appName];
            }
            return null;
        }

        /// <summary>
        /// Unchecked and enabled all siblings
        /// </summary>
        /// <param name="menuItem"><code>System.Windows.Controls.MenuItem</code> which was checked</param>
        public void uncheckedMenuItem(System.Windows.Controls.MenuItem menuItem)
        {
            if(menuItem.Parent != null)
            {
                foreach(System.Windows.Controls.MenuItem siblingMenuitem in ((System.Windows.Controls.MenuItem)menuItem.Parent).Items)
                {
                    if (!siblingMenuitem.Equals(menuItem))
                    {
                        siblingMenuitem.IsChecked = false;
                        siblingMenuitem.IsEnabled = true;
                    }
                }
            }
        }

    }


}
