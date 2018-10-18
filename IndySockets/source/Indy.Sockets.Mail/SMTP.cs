using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Indy.Sockets.Core;

namespace Indy.Sockets.Mail {
    public class SMTP : TCPClient {

        protected override int GetDefaultPort() {
            return 25;
        }

        protected void SendGreeting() {
            //  Capabilities.Clear;
            string xHELOName = "";
            if (HELOName == "") {
                //TODO: Get host name from Windows
                //VCL for .Net used System.Windows.Forms.SystemInformation.ComputerName
                //Besides, I think RFC 821 was refering to the computer's Internet
                //DNS name.  We use the Computer name only if we can't get the DNS name.
                //xHELOName = GStack.HostName;
            } else {
                xHELOName = mHELOName;
            }
            if (mUseEHLO) {
                Do("EHLO", xHELOName, 250);
                // Capabilities.AddStrings(LastCmdResult.Text);
                //  if Capabilities.Count > 0 then begin
                //    //we drop the initial greeting.  We only want the feature list
                //    Capabilities.Delete(0);
                //  end;
            } else {
                Do("HELO", xHELOName, 250);
            }
        }

        protected static string mHELOName = "";
        public static string HELOName {
            get { return mHELOName; }
            set { mHELOName = value; }
        }

        public SMTP(string aHost) : this(aHost, true) { }

        protected readonly bool mUseEHLO;
        public SMTP(string aHost, bool aUseEHLO) : base(aHost) {
            mUseEHLO = aUseEHLO;
            switch (ReadResponse(220, 554)) {
                case 220:
                    SendGreeting();
                    break;
                case 554:
                    Close();
                    break;
            }
        }

        public override void Disconnect() {
            Do("QUIT", 221);
            base.Disconnect();
        }

        public void Send(Message aMsg) {
            Do("MAIL FROM:<" + aMsg.From.Email + ">", 250);
            foreach (var x in aMsg.To) {
                Do("RCPT TO:<" + x.Email + ">", 250);
            }
            foreach (var x in aMsg.CC) {
                Do("RCPT TO:<" + x.Email + ">", 250);
            }
            foreach (var x in aMsg.BCC) {
                Do("RCPT TO:<" + x.Email + ">", 250);
            } 
            Do("DATA", 354);

            aMsg.WriteHeader(mConnection.TextWriter);
            mConnection.TextWriter.WriteLine();
            aMsg.WriteBody(mConnection.TextWriter, true);
            mConnection.TextWriter.WriteLine("."); 
            ReadResponse(250);
        }

    }
}
