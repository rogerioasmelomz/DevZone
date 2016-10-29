using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutCode.Net
{
    public class ResponsePOP3 : Response {
        public const int OK = 0;
        public const int ERR = -1;

        public override bool Parse(string aText) {
            // Do not assume a space after reply, sometimes servers
            // reply with only +OK/-ERR and no message.
            if (aText.Substring(0, 3) == "+OK") {
                mNumeric = OK;
                if (aText.Length > 4) {
                    Text.Add(aText.Substring(4));
                }
            } else if (aText.Substring(0, 4) == "-ERR") {
                mNumeric = ERR;
                if (aText.Length > 5) {
                    Text.Add(aText.Substring(5));
                }
            } else {
                throw new OutCode.Net.Exception("Invalid response");
            }
            return true;
        }
    }
}
