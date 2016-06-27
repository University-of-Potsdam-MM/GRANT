using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using StrategyManager.Interfaces;



namespace GApplication
{
    public class Settings
    {
        private static String readAppSettings(String name)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (appSettings == null) { return "ERROR"; }
            String value = appSettings.Get(name);

            return value;
        }

        private static List<String> getPossibleStrategyClasses(String name)
        {
            String filters = readAppSettings(name);
            return splitNames(filters); 
        }

        private static List<String> splitNames(String filterName)
        {
            if (filterName == null || filterName.Equals("ERROR") ) { return null; }
            return filterName.Split(new Char[] { ',' }).Select(s => s.Trim()).Where(s => s != String.Empty).ToList();
        }

        /// <summary>
        /// Ordnet anhand der Filter.config einen Filter-Anzeigenamen  einen Filter-Klassennamen zu
        /// </summary>
        /// <param name="strategyUserName">gibt den Anzeigenamen an</param>
        /// <returns>Den Klassenname der Strategy-Klasse als String</returns>
        public String strategyUserNameToClassName(String strategyUserName)
        {
            String classStrategyName = readAppSettings(strategyUserName);
            return classStrategyName;
        }

        public List<Strategy> getPossibleFilters()
        {
            List<Strategy> filter = new List<Strategy>();
            List<String> filterNames = getPossibleStrategyClasses("PossibleFilters");
            if (filterNames == null) { return filter; }
            Strategy f = new Strategy();
            foreach (String fName in filterNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                filter.Add(f);
            }
            return filter;
        }
          
        public List<Strategy> getPossibleOperationSystems()
        {
            List<Strategy> operationSystems = new List<Strategy>();
            List<String> operationSystemNames = getPossibleStrategyClasses("PossibleOperationSystems");
            if (operationSystemNames == null) { return operationSystems; }
            Strategy f = new Strategy();
            foreach (String osName in operationSystemNames)
            {
                f.userName = osName;
                f.className = strategyUserNameToClassName(osName);
                operationSystems.Add(f);
            }
            return operationSystems;
        }

        public List<Strategy> getPossibleTrees()
        {
            List<Strategy> trees = new List<Strategy>();
            List<String> treeNames = getPossibleStrategyClasses("PossibleTrees");
            if (treeNames == null) { return trees; }
            Strategy t = new Strategy();
            foreach (String tName in treeNames)
            {
                t.userName = tName;
                t.className = strategyUserNameToClassName(tName);
                trees.Add(t);
            }
            return trees;
        }

        public List<Strategy> getPossibleBrailleDisplays()
        {
            List<Strategy> filter = new List<Strategy>();
            List<String> filterNames = getPossibleStrategyClasses("PossibleBrailleDisplays");
            if (filterNames == null) { return filter; }
            Strategy f = new Strategy();
            foreach (String fName in filterNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                filter.Add(f);
            }
            return filter;
        }

        public List<Strategy> getPossibleTreeOperations()
        {
            List<Strategy> trees = new List<Strategy>();
            List<String> treeNames = getPossibleStrategyClasses("PossibleTreeOperations");
            if (treeNames == null) { return trees; }
            Strategy t = new Strategy();
            foreach (String tName in treeNames)
            {
                t.userName = tName;
                t.className = strategyUserNameToClassName(tName);
                trees.Add(t);
            }
            return trees;
        }


    }
}
