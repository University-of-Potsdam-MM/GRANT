using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using StrategyManager.Interfaces;



namespace GApplication
{
    class Settings
    {
        private static String readAppSettings(String name)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (appSettings == null) { return "ERROR"; }
            String value = appSettings.Get(name);

            return value;
        }

        private static List<String> getPosibleFilterClasses()
        {
            String filters = readAppSettings("PossibleFilters");
            return splitFilterNames(filters); 
        }

        private static List<String> splitFilterNames(String filterName)
        {
            if (filterName == null || filterName.Equals("ERROR") ) { return null; }
            return filterName.Split(new Char[] { ',' }).Select(s => s.Trim()).Where(s => s != String.Empty).ToList();
        }

        /// <summary>
        /// Ordnet anhand der Filter.config einen Filter-Anzeigenamen  einen Filter-Klassennamen zu
        /// </summary>
        /// <param name="filterUserName">gibt den Filter-Anzeigenamen an</param>
        /// <returns>Den Klassenname der Filter-Klasse als String</returns>
        public String filterUserNameToClassName(String filterUserName)
        {
            String classFilterName = readAppSettings(filterUserName);
            return classFilterName;
        }

        public List<Filter> getPosibleFilters()
        {
            List<Filter> filter = new List<Filter>();
            List<String> filterNames = getPosibleFilterClasses();
            if (filterNames == null) { return filter; }
            Filter f = new Filter();
            foreach (String fName in filterNames)
            {
                f.userName = fName;
                f.className = filterUserNameToClassName(fName);
                filter.Add(f);
            }
            return filter;
        }

        /// <summary>
        /// Ermittelt das Objekt einer FilterStrategy.
        /// </summary>
        /// <param name="filterUserName">gibt den Anzeigenamen der FilterStrategy an</param>
        /// <returns><code>IFilterStrategy</code>-Objekt des genutzten Filters</returns>
        /*public IFilterStrategy getFilterObjectName(String filterUserName)
        {
            Type type = Type.GetType(filterUserNameToClassName(filterUserName));
            return (IFilterStrategy)Activator.CreateInstance(type);
        }*/
    }
}
