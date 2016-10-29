using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Interop
{
    public static class Resources
    {

        /// <summary>
        /// Lists all embeded resources with name that matches de filter
        /// </summary>
        /// <param name="a">Assembly to look for resources</param>
        /// <param name="Filter">Resource name filter</param>
        /// <returns>List os resourcer names that matched the given filter</returns>
        public static IEnumerable<string> GetResourcesList(Assembly a, string Filter)
        {
            IEnumerable<string> resourceNames;

            try
            {
                resourceNames = a.GetManifestResourceNames().Where(x => x.IndexOf(Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1).OrderBy(x => x);
            }
            catch (Exception)
            {
                throw;
            }

            return (resourceNames);
        }

        /// <summary>
        /// Read data from embeded text resources (text files)
        /// </summary>
        /// <remarks>remarks</remarks>
        /// <param name="a">Base assembly</param>
        /// <param name="ResourceName">Resource name to read</param>
        /// <returns>Text readed from resource file</returns>
        public static string LoadTextResourceData(Assembly a, string ResourceName) {

            Stream st=null;
            StreamReader reader=null;
            try
            {
                st = a.GetManifestResourceStream(ResourceName);
                reader = new StreamReader(st);

                string resourceInfo = reader.ReadToEnd();
                return (resourceInfo);
            }
            catch (Exception e)
            {

                throw new Exception(string.Format("Error reading resource '{0}'.", ResourceName), e);

            }
            finally {
                st.Dispose();
                reader.Dispose(); 
            }


        }

    }
}
;