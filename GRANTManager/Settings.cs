using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using GRANTManager.Interfaces;



namespace GRANTManager
{
    /// <summary>
    /// Reads Strings from the Strategy.config
    /// </summary>
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
        /// Gives the appropriate filter-class-name to a filter-showing-name (depending on the Strategy.config)
        /// </summary>
        /// <param name="strategyUserName">filter-showing-name</param>
        /// <returns>the filter-class name</returns>
        public static String strategyUserNameToClassName(String strategyUserName)
        {
            String classStrategyName = readAppSettings(strategyUserName);
            return classStrategyName;
        }

        public List<Strategy> getPossibleBrailleConverter()
        {
            List<Strategy> brailleConverter = new List<Strategy>();
            List<String> brailleConverterNames = getPossibleStrategyClasses("BrailleConverter");
            if (brailleConverterNames == null) { return brailleConverter; }
            Strategy f = new Strategy();
            foreach (String osName in brailleConverterNames)
            {
                f.userName = osName;
                f.className = strategyUserNameToClassName(osName);
                brailleConverter.Add(f);
            }
            return brailleConverter;
        }


        public List<Strategy> getPossibleExternalScreenreaders()
        {
            List<Strategy> externalScreenreader = new List<Strategy>();
            List<String> externalScreenreaderNames = getPossibleStrategyClasses("ExternalScreenreader");
            if (externalScreenreaderNames == null) { return externalScreenreader; }
            Strategy f = new Strategy();
            foreach (String osName in externalScreenreaderNames)
            {
                f.userName = osName;
                f.className = strategyUserNameToClassName(osName);
                externalScreenreader.Add(f);
            }
            return externalScreenreader;
        }

        public static String filterStrategyTypeToUserName(Type strategyType)
        {
            List<Strategy> filterStrategies = getPossibleFilters();
            Strategy filterFind = filterStrategies.Find(f => f.className.Equals(strategyType.FullName+", "+strategyType.Namespace));
            return filterFind.Equals(new Strategy()) ? strategyType.Name : filterFind.userName;

        }

        public static List<Strategy> getPossibleFilters()
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

        public List<Strategy> getPosibleDisplayStrategies()
        {
            List<Strategy> displayStrategy = new List<Strategy>();
            List<String> filterNames = getPossibleStrategyClasses("PossibleDisplayStrategies");
            if (filterNames == null) { return displayStrategy; }
            Strategy f = new Strategy();
            foreach (String fName in filterNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                displayStrategy.Add(f);
            }
            return displayStrategy;
        }

        public List<Strategy> getPossibleEventManager()
        {
            List<Strategy> eventManager = new List<Strategy>();
            List<String> eventManagerNames = getPossibleStrategyClasses("PossibleEventManager");
            if (eventManagerNames == null) { return eventManager; }
            Strategy f = new Strategy();
            foreach (String fName in eventManagerNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                eventManager.Add(f);
            }
            return eventManager;
        }

        public List<Strategy> getPossibleEventManager2()
        {
            List<Strategy> eventManager = new List<Strategy>();
            List<String> eventManagerNames = getPossibleStrategyClasses("PossibleEventManager2");
            if (eventManagerNames == null) { return eventManager; }
            Strategy f = new Strategy();
            foreach (String fName in eventManagerNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                eventManager.Add(f);
            }
            return eventManager;
        }

        public List<Strategy> getPossibleEventAction()
        {
            List<Strategy> eventAction = new List<Strategy>();
            List<String> eventActionNames = getPossibleStrategyClasses("PossibleEventAction");
            if (eventActionNames == null) { return eventAction; }
            Strategy f = new Strategy();
            foreach (String fName in eventActionNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                eventAction.Add(f);
            }
            return eventAction;
        }

        public List<Strategy> getPossibleEventProcessor()
        {
            List<Strategy> eventProcessor = new List<Strategy>();
            List<String> eventProcessorNames = getPossibleStrategyClasses("PossibleEventProcessor");
            if (eventProcessorNames == null) { return eventProcessor; }
            Strategy f = new Strategy();
            foreach (String fName in eventProcessorNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                eventProcessor.Add(f);
            }
            return eventProcessor;
        }

        public List<Strategy> getPossibleUiTemplateStrategies()
        {
            List<Strategy> templates = new List<Strategy>();
            List<String> templatesNames = getPossibleStrategyClasses("PossibleUiTemplateStrategies");
            if (templatesNames == null) { return templates; }
            Strategy f = new Strategy();
            foreach (String fName in templatesNames)
            {
                f.userName = fName;
                f.className = strategyUserNameToClassName(fName);
                templates.Add(f);
            }
            return templates;
        }


        public static List<String> getPossibleTypesOfViews()
        {
            String viewCategories = readAppSettings("ListOfTypesOfViews");
            return splitNames(viewCategories);
        }

        /// <summary>
        /// Reads the name of the directory from the config where are the screen readers are saved
        /// </summary>
        /// <returns></returns>
        public static String getScreenReaderDirectory()
        {
            return readAppSettings("savedScreenReaderDirectory");
        }

        public static String getFilteredTreeSavedName()
        {
            return readAppSettings("filteredTreeSavedName");
        }

        public static String getBrailleTreeSavedName()
        {
            return readAppSettings("brailleTreeSavedName");
        }

        public static String getOsmTreeConectorName()
        {
            return readAppSettings("osmTreeConectorName");
        }

        public static String getNavigationbarSubstring()
        {
            return readAppSettings("navigationbarSubstring");
        }
    }
}
