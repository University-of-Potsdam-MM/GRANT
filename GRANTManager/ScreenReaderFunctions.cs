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
        /// Determines whether a screen reader is already exists
        /// </summary>
        /// <param name="projectDirectory">the the directory of the screen reader</param>
        /// <param name="screenReaderProcessName">the name of the process of the screen reader</param>
        /// <returns><c>true</c> if the screenreader exist; otherwiese <c>false</c></returns>
        public Boolean existScreenReader(String projectDirectory, out KeyValuePair<String,String> screenReader)
        {
            screenReader = new KeyValuePair<string, string>();
            String screenReaderProcessName = getProcessName(@projectDirectory + Path.DirectorySeparatorChar + Settings.getFilteredTreeSavedName());
            if (screenReaderProcessName == null || screenReaderProcessName.Equals(""))
            {
                return false;
            }
            if (screenreaders.ContainsKey(screenReaderProcessName))
            {
                screenReader = screenreaders.First(p => p.Key.Equals(screenReaderProcessName));
                return true;
            }
            screenReader = new KeyValuePair<string, string>(screenReaderProcessName, null);
            return false;
        }


        /// <summary>
        /// Delete an existing screen reader
        /// </summary>
        /// <param name="screenReaderName">name of the screen reader (with file extention '.grant' and without the path)</param>
        /// <returns><c>true</c> if the screen reader was deleted; otherwiese <c>false</c></returns>
        public bool deleteScreenReader(String screenReaderName)
        {
            String fileExtention = ".grant";
            if (!Path.GetExtension(screenReaderName).Equals(fileExtention)) { return false; }
            bool result = false;
            if(File.Exists(Path.Combine(Settings.getScreenReaderDirectory(), screenReaderName)))
            {
                try
                {
                    File.Delete(Path.Combine(Settings.getScreenReaderDirectory(), screenReaderName));
                    result = true;
                }
                #region catch: IOException, UnauthorizedAccessException, Exception
                catch (IOException e)
                {
                    Debug.WriteLine("IOException in deleteScreenReader:\n" + e);
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine("UnauthorizedAccessException in deleteScreenReader:\n" + e);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in deleteScreenReader:\n" + e);
                }
                #endregion
                
            }else { result = false; }
            
            String directoryDel = Path.Combine(Settings.getScreenReaderDirectory(), Path.GetFileNameWithoutExtension(screenReaderName));
            if (Directory.Exists(directoryDel))
            {
                try
                {
                    Directory.Delete(directoryDel, true);
                    result = true;
                }
                #region catch: IOException, UnauthorizedAccessException, Exception
                catch (IOException e) {
                    Debug.WriteLine("IOException in deleteScreenReader:\n"+e);
                }
                catch (UnauthorizedAccessException e) {
                    Debug.WriteLine("UnauthorizedAccessException in deleteScreenReader:\n" + e);

                }
                catch (Exception e) {
                    Debug.WriteLine("Exception in deleteScreenReader:\n" + e);
                }
                #endregion
                
            }
            //else { result = result || false; }
            return result;
        }

        public bool addScreenReader(String screenReaderPath)
        { 
            String fileExtention = ".grant";
            #region check whether all files exist
            String projectDirectory = Path.GetDirectoryName(@screenReaderPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@screenReaderPath);
            Boolean existFiles = true;
            existFiles = File.Exists(projectDirectory + Path.DirectorySeparatorChar + Settings.getBrailleTreeSavedName());
            existFiles = existFiles && File.Exists(projectDirectory + Path.DirectorySeparatorChar + Settings.getFilteredTreeSavedName());
            existFiles = existFiles && File.Exists(projectDirectory + Path.DirectorySeparatorChar + Settings.getFilterstrategyFileName());
            existFiles = existFiles && File.Exists(projectDirectory + Path.DirectorySeparatorChar + Settings.getOsmConectorName());
            if (existFiles == false) { System.Windows.Forms.MessageBox.Show("The chosen screen reader doesn't exist!", "GRANT exception"); return false; }
            #endregion
            #region check whether the screen reader exist in the used screen reader directory
            KeyValuePair<String, String> screenReader;
            Boolean isExistScreenReader = existScreenReader(@projectDirectory, out screenReader);
            System.Windows.Forms.DialogResult screenReaderOverride = new System.Windows.Forms.DialogResult();
            if (isExistScreenReader)
            {
                if (screenReader.Key != null)
                {
                    screenReaderOverride = System.Windows.Forms.MessageBox.Show("A screen reader for this application ('" + screenReader + "') is already exist! \nDo you want to replace the existing screen reader?", "GRANT notification", System.Windows.Forms.MessageBoxButtons.YesNo);
                }
                else { screenReaderOverride = System.Windows.Forms.MessageBox.Show("A screen reader for this application is already exist! \nDo you want to replace the existing screen reader?", "GRANT notification", System.Windows.Forms.MessageBoxButtons.YesNo); }
            }
            #endregion
            if (!isExistScreenReader || (isExistScreenReader && screenReaderOverride == System.Windows.Forms.DialogResult.Yes))
            {
                if (screenReaderOverride == System.Windows.Forms.DialogResult.Yes && screenReader.Value != null)
                {
                    deleteScreenReader(Path.GetFileName(screenReader.Value));
                }
                try
                {
                    File.Copy(screenReaderPath, Settings.getScreenReaderDirectory() + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@screenReaderPath) + fileExtention, true);
                }
                #region catch: IOException, UnauthorizedAccessException, Exception
                catch (IOException e)
                {
                    Debug.WriteLine("IOException in addScreenReader:\n" + e);
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine("UnauthorizedAccessException in addScreenReader:\n" + e);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in addScreenReader:\n" + e);
                }
                #endregion
                CloneDirectory(projectDirectory, Settings.getScreenReaderDirectory() + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@screenReaderPath));
                return true;
            }
            return false;
        }

        //https://stackoverflow.com/questions/36484009/how-to-copy-and-paste-a-whole-directory-to-a-new-path-recursively
        private static void CloneDirectory(string source, string dest)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }
            foreach (var file in Directory.GetFiles(source))
            {
                try
                {
                    File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
                }
                #region catch: IOException, UnauthorizedAccessException, Exception
                catch (IOException e)
                {
                    Debug.WriteLine("IOException in CloneDirectory:\n" + e);
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine("UnauthorizedAccessException in CloneDirectory:\n" + e);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in CloneDirectory:\n" + e);
                }
                #endregion
            }
        }

    }


}
