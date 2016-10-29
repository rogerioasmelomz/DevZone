using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutCode.Net
{
    // We use this instead of System.Net.Mail.MailAddress for the following reasons:
    // -MailAddress is very basic, using it would save little code
    // -This would create the first and possibly only dependency on System.Net.Mail
    // -Indy may offer more functionality in the future than MailAddress offers
    // -System.Net.Mail itself is quite basic and Indy will eccplipse its functionality quickly
    public class EmailAddress {
        public EmailAddress() {
        }

        public EmailAddress(string aText) {
            //TODO: Need to fix this
            // There are two formats, need to detect and parse
            Email = aText;
        }

        public EmailAddress(string aEmail, string aText) {
            Email = aEmail;
            mText = aText;
        }

        protected string mUser = "";
        public string User {
            get { return mUser; }
            set { mUser = value; }
        }

        protected string mDomain = "";
        public string Domain {
            get { return mDomain; }
            set { mDomain = value; }
        }
        
        protected string mText = "";
        public string Text {
            get { return mText; }
            set { mText = value; }
        }

        public string Email {
            get {
                if ((mUser == "") || (mDomain == "")) {
                    return "";
                } else {
                    return mUser + "@" + mDomain;
                }
            }
            set {
                //TODO: Add exceptions for invalid emails
                var xParts = value.Split('@');
                mUser = xParts[0];
                mDomain = xParts[1];
            }
        }

        public override string ToString() {
            if (Text == "") {
                return "<" + Email + ">";
            } else {
                return "\"" + Text + "\" <" + Email + ">";
            }
        }
        
    }
}
