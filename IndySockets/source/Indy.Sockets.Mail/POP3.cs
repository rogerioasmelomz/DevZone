using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indy.Sockets.Core;

namespace Indy.Sockets.Mail {
    public class POP3 : TCPClient {

        public POP3(string aHost, string aUser, string aPassword) : base(aHost) {
            //  S: String;
            //  I: Integer;
            //begin
            //  FHasAPOP := False;
            //  FHasCAPA := False;

            //  if UseTLS in ExplicitTLSVals then begin
            //    // TLS only enabled later in this case!
            //    (IOHandler as TIdSSLIOHandlerSocketBase).PassThrough := True;
            //  end;

            //  if (IOHandler is TIdSSLIOHandlerSocketBase) then begin
            //      case FUseTLS of
            //       utNoTLSSupport :
            //       begin
            //        (IOHandler as TIdSSLIOHandlerSocketBase).PassThrough := True;
            //       end;
            //       utUseImplicitTLS :
            //       begin
            //         (IOHandler as TIdSSLIOHandlerSocketBase).PassThrough := False;
            //       end
            //       else
            //        if FUseTLS<>utUseImplicitTLS then begin
            //         (IOHandler as TIdSSLIOHandlerSocketBase).PassThrough := True;
            //        end;
            //      end;
            //  end;
            //  inherited;
            var xGreting = ReadResponse(ResponsePOP3.OK);
            //  // the initial greeting text is needed to determine APOP availability
            //  S := LastCmdResult.Text.Strings[0];  //read response
            //  I := Pos('<', S);    {Do not Localize}
            //  if i > 0 then begin
            //    S := Copy(S, I, MaxInt); //?: System.Delete(S,1,i-1);
            //    I := Pos('>', S);    {Do not Localize}
            //    if I > 0 then begin
            //      S := Copy(S, 1, I);
            //    end else begin
            //      S := '';    {Do not Localize}
            //    end;
            //  end else begin
            //    S := ''; //no time-stamp    {Do not Localize}
            //  end;
            //  FAPOPToken := S;
            //  FHasAPOP := (Length(S) > 0);
            //  CAPA;

            // It's rare that users would not want to autologin. 
            // So we do support it, but we do not make a separate construtor for it
            if ((aUser != null) && (aPassword != null)) {
                Authenticate(aUser, aPassword);
            }
        }

        protected override int GetDefaultPort() {
            return 110;
        }

        protected override Response NewResponse() {
            return new ResponsePOP3();
        }

        public override void Disconnect() {
            Do("QUIT", ResponsePOP3.OK);
            base.Close();
        }

        public void Authenticate(string aUser, string aPassword) {
            Do("USER", aUser, ResponsePOP3.OK);
            Do("PASS", aPassword, ResponsePOP3.OK);
        }

        public int GetStatus() {
            int xVoid;
            return GetStatus(out xVoid);
        }

        public int GetStatus(out int oSize) {
            var xResponse = Do("STAT", ResponsePOP3.OK);
            var xParts = xResponse.Text[0].Split(' ');
            oSize = int.Parse(xParts[1]);
            return int.Parse(xParts[0]);
        }

        public void Delete(int aMsgNo) {
            Do("DELE", aMsgNo.ToString(), ResponsePOP3.OK);
        }

        public void NoOp() {
            Do("NOOP", ResponsePOP3.OK);
        }

        public void Reset() {
            Do("RSET", ResponsePOP3.OK);
        }

        public Dictionary<string, List<string>> RetrieveHeaders(int aMsgNo) {
            Do("TOP", new object[] { aMsgNo, 0 }, ResponsePOP3.OK);
            var xResult = mConnection.TextReader.ReadHeaders();

            // Next line should be ., but just in case of
            // buggy servers we read whatever follows and
            // discard it
            var xVoid = new List<string>();
            mConnection.TextReader.ReadRFCLines(xVoid);

            return xResult;
        }

        public void Retrieve(int aMsgNo, Message aMsg) {
            Do("RETR", aMsgNo.ToString(), ResponsePOP3.OK);
            Retrieve(aMsg);
        }

        public void Retrieve(int aMsgNo, Message aMsg, int aLines) {
            Do("TOP", new object[] {aMsgNo, aLines}, ResponsePOP3.OK);
            Retrieve(aMsg);
        }

        protected void Retrieve(Message aMsg) {
            aMsg.ReadHeaders(mConnection.TextReader);
            aMsg.ReadBody(mConnection.TextReader);
        }

        public class MsgInfo {
            public int Size;
            public string[] Extra;
        }

        public Dictionary<int, string> GetUIDs() {
            Do("UIDL", ResponsePOP3.OK);
            var xResult = new Dictionary<int, string>();
            string xLine;
            while ((xLine = mConnection.TextReader.ReadLine()) != ".") {
                var xParts = xLine.Split(' ');
                xResult.Add(int.Parse(xParts[0]), xParts[1]);
            }
            return xResult;
        }

        public string GetUID(int aMsgNo) {
            var xResponse = Do("UIDL", aMsgNo.ToString(), ResponsePOP3.OK);
            var xParts = xResponse.Text[0].Split(' ');
            return xParts[1];
        }

        public Dictionary<int, MsgInfo> GetSizes() {
            Do("LIST", ResponsePOP3.OK);
            // A dictionary is used because:
            // -Deleted messages are not included in list
            // -Servers should return them in order, but are not
            //  required to
            var xResult = new Dictionary<int, MsgInfo>();
            string xLine;
            while ((xLine = mConnection.TextReader.ReadLine()) != ".") {
                int xMsgNo;
                var xMsgInfo = ParseScanLine(xLine, out xMsgNo);
                xResult.Add(xMsgNo, xMsgInfo);
            }
            return xResult;
        }

        public MsgInfo GetSize(int aMsgNo) {
            var xResponse = Do("LIST", aMsgNo.ToString(), ResponsePOP3.OK);
            int xVoid;
            return ParseScanLine(xResponse.Text[0], out xVoid);
        }

        protected MsgInfo ParseScanLine(string aScanLine, out int oMsgNo) {
            var xResult = new MsgInfo();
            var xParts = aScanLine.Split(' ');
            oMsgNo = int.Parse(xParts[0]);
            xResult.Size = int.Parse(xParts[1]);
            // Severs generally do not return extra info.
            // If they do, format is undefined so we just
            // store it.
            if (xParts.Length > 2) {
                xResult.Extra = new string[xParts.Length - 2];
                Array.ConstrainedCopy(xParts, 2, xResult.Extra, 0, xParts.Length - 2);
            }
            return xResult;
        }

    }
}

//      APOP name digest

//         Arguments:
//             a string identifying a mailbox and a MD5 digest string
//             (both required)

//         Restrictions:
//             may only be given in the AUTHORIZATION state after the POP3
//             greeting or after an unsuccessful USER or PASS command

//         Discussion:
//             Normally, each POP3 session starts with a USER/PASS
//             exchange.  This results in a server/user-id specific
//             password being sent in the clear on the network.  For
//             intermittent use of POP3, this may not introduce a sizable
//             risk.  However, many POP3 client implementations connect to
//             the POP3 server on a regular basis -- to check for new
//             mail.  Further the interval of session initiation may be on
//             the order of five minutes.  Hence, the risk of password
//             capture is greatly enhanced.

//             An alternate method of authentication is required which
//             provides for both origin authentication and replay
//             protection, but which does not involve sending a password
//             in the clear over the network.  The APOP command provides
//             this functionality.

//             A POP3 server which implements the APOP command will
//             include a timestamp in its banner greeting.  The syntax of
//             the timestamp corresponds to the `msg-id' in [RFC822], and
//             MUST be different each time the POP3 server issues a banner
//             greeting.  For example, on a UNIX implementation in which a
//             separate UNIX process is used for each instance of a POP3
//             server, the syntax of the timestamp might be:

//                <process-ID.clock@hostname>

//             where `process-ID' is the decimal value of the process's
//             PID, clock is the decimal value of the system clock, and
//             hostname is the fully-qualified domain-name corresponding
//             to the host where the POP3 server is running.

//             The POP3 client makes note of this timestamp, and then
//             issues the APOP command.  The `name' parameter has
//             identical semantics to the `name' parameter of the USER
//             command. The `digest' parameter is calculated by applying
//             the MD5 algorithm [RFC1321] to a string consisting of the
//             timestamp (including angle-brackets) followed by a shared
//             secret.  This shared secret is a string known only to the
//             POP3 client and server.  Great care should be taken to
//             prevent unauthorized disclosure of the secret, as knowledge
//             of the secret will allow any entity to successfully
//             masquerade as the named user.  The `digest' parameter
//             itself is a 16-octet value which is sent in hexadecimal
//             format, using lower-case ASCII characters.

//             When the POP3 server receives the APOP command, it verifies
//             the digest provided.  If the digest is correct, the POP3
//             server issues a positive response, and the POP3 session
//             enters the TRANSACTION state.  Otherwise, a negative
//             response is issued and the POP3 session remains in the
//             AUTHORIZATION state.

//             Note that as the length of the shared secret increases, so
//             does the difficulty of deriving it.  As such, shared
//             secrets should be long strings (considerably longer than
//             the 8-character example shown below).

//         Possible Responses:
//             +OK maildrop locked and ready
//             -ERR permission denied

//         Examples:
//             S: +OK POP3 server ready <1896.697170952@dbc.mtview.ca.us>
//             C: APOP mrose c4c9334bac560ecc979e58001b3e22fb
//             S: +OK maildrop has 1 message (369 octets)

//             In this example, the shared  secret  is  the  string  `tan-
//             staaf'.  Hence, the MD5 algorithm is applied to the string
//                <1896.697170952@dbc.mtview.ca.us>tanstaaf

//             which produces a digest value of
//                c4c9334bac560ecc979e58001b3e22fb

//12. References
//   [RFC1321] Rivest, R., "The MD5 Message-Digest Algorithm", RFC 1321,
//       MIT Laboratory for Computer Science, April 1992.
//   [RFC1734] Myers, J., "POP3 AUTHentication command", RFC 1734,
//       Carnegie Mellon, December 1994.
