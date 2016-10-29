using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Interop
{
    public static class ProgInfo
    {
        public static string GetVersion(Object A) {
            return (A.GetType().GetTypeInfo().Assembly.GetName().Version.ToString());
        }

        public static string GetName(Object A) {
            try
            {
                return (A.GetType().GetTypeInfo().Namespace );
            }
            catch 
            {
                return ("Unknown...");
            }
            
        }

    }
}
