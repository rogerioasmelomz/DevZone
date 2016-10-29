using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutCode.Utils
{
    public static class Check
    {
        public static bool MailFormat(string strEmail) {
            Regex regex1 = new Regex("^[a-zA-Z]+[a-zA-Z0-9]+[[a-zA-Z0-9-_.!#$%'*+/=?^]{1,20}@[a-zA-Z0-9]{1,20}.[a-zA-Z]{2,3}$");
            return (regex1.IsMatch(strEmail));
        }

    }
}
