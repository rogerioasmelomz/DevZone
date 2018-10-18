using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Indy.Sockets.Core;

namespace Indy.Sockets.Mail {
    public abstract class Message {

        protected string mSubject = null;
        public string Subject {
            get { return mSubject; }
            set { mSubject = value; }
        }

        protected EmailAddress mFrom = new EmailAddress();
        public EmailAddress From {
            get { return mFrom; }
        }

        protected EmailAddress mSender = new EmailAddress();
        public EmailAddress Sender {
            get { return mSender; }
        }

        protected EmailAddress mReplyTo = new EmailAddress();
        public EmailAddress ReplyTo {
            get { return mReplyTo; }
        }

        protected List<EmailAddress> mTo = new List<EmailAddress>();
        public List<EmailAddress> To {
            get { return mTo; }
        }

        protected List<EmailAddress> mCC = new List<EmailAddress>();
        public List<EmailAddress> CC {
            get { return mCC; }
        }

        protected List<EmailAddress> mBCC = new List<EmailAddress>();
        public List<EmailAddress> BCC {
            get { return mBCC; }
        }

        public abstract void WriteBody(TextWriter aWriter, bool aRFCFormat);
        public abstract void ReadBody(TextReader aReader);

        protected void ProcessEmailList(List<EmailAddress> aProperty, Dictionary<string, List<string>> aHeaders, string aName) {
            List<string> xValues;
            if (aHeaders.TryGetValue(aName, out xValues)) {
                foreach (var x in xValues) {
                    aProperty.Add(new EmailAddress(x));
                }
            }
        }

        public void ReadHeaders(TextReader aReader) {
            var xHeaders = aReader.ReadHeaders();
            mSubject = xHeaders["Subject"][0];
            mFrom = new EmailAddress(xHeaders["From"][0]);

            ProcessEmailList(To, xHeaders, "To");
            ProcessEmailList(CC, xHeaders, "CC");
            // Might seems stupid to put BCC here, but when we save to disk etc
            // sometimes BCC can be saved            
            ProcessEmailList(BCC, xHeaders, "BCC");
        }

        protected string HeaderLine(string aName, string aValue) {
            return aName + ": " + aValue;
        }

        protected string AddressList(List<EmailAddress> aList) {
            var xResult = new StringBuilder();
            if (aList.Count > 0) {
                foreach (var x in aList) {
                    xResult.Append(x.ToString() + ", ");
                }
                xResult.Remove(xResult.Length - 2, 2);
            }
            return xResult.ToString();
        }

        public void WriteHeader(TextWriter aWriter) {
            //var
            //  ISOCharset: string;
            //  HeaderEncoding: Char;
            //  TransferHeader: TTransfer;
            //  LN: Integer;
            //  LEncoding: string;
            //  LMIMEBoundary: string;
            //begin
            //  MessageParts.CountParts;
            //  {CC2: If the encoding is meDefault, the user wants us to pick an encoding mechanism:}
            //  if Encoding = meDefault then begin
            //    if MessageParts.Count = 0 then begin
            //      {If there are no attachments, we want the simplest type, just the headers
            //      followed by the message body: mePlainText does this for us}
            //      Encoding := mePlainText;
            //    end else begin
            //      {If there are any attachments, default to MIME...}
            //      Encoding := meMIME;
            //    end;
            //  end;
            //  for LN := 0 to MessageParts.Count-1 do begin
            //    {Change any encodings we don't kSys.Now to base64 for MIME and UUE for PlainText...}
            //    LEncoding := MessageParts[LN].ContentTransfer;
            //    if LEncoding <> '' then begin
            //      if Encoding = meMIME then begin
            //        if ( (TextIsSame(LEncoding, '7bit') = False) and                     {do not localize}
            //         (TextIsSame(LEncoding, '8bit') = False) and                         {do not localize}
            //         (TextIsSame(LEncoding, 'binary') = False) and                     {do not localize}
            //         (TextIsSame(LEncoding, 'base64') = False) and                       {do not localize}
            //         (TextIsSame(LEncoding, 'quoted-printable') = False) and             {do not localize}
            //         (TextIsSame(LEncoding, 'binhex40') = False)) then begin             {do not localize}
            //          MessageParts[LN].ContentTransfer := 'base64';                      {do not localize}
            //        end;
            //      end else begin  //mePlainText
            //        if ( (TextIsSame(LEncoding, 'UUE') = False) and                      {do not localize}
            //         (TextIsSame(LEncoding, 'XXE') = False)) then begin                  {do not localize}
            //          MessageParts[LN].ContentTransfer := 'UUE';                         {do not localize}
            //        end;
            //      end;
            //    end;
            //  end;
            //  {CC2: We dont support attachments in an encoded body.
            //  Change it to a supported combination...}
            //  if MessageParts.Count > 0 then begin
            //    if ((ContentTransferEncoding <> '') and
            //        (not TextIsSame(ContentTransferEncoding, '7bit')) and         {do not localize}
            //        (not TextIsSame(ContentTransferEncoding, 'binary')) and       {do not localize}
            //        (not TextIsSame(ContentTransferEncoding, '8bit'))) then begin {do not localize}
            //      ContentTransferEncoding := '';
            //    end;
            //  end;
            //  if Encoding = meMIME then begin
            //    //HH: Generate Boundary here so we kSys.Now it in the headers
            //    LMIMEBoundary := IdMIMEBoundaryStrings.IndyMIMEBoundary;
            //    //CC: Moved this logic up from SendBody to here, where it fits better...
            //    if Length(ContentType) = 0 then begin
            //      //User has omitted ContentType.  We have to guess here, it is impossible
            //      //to determine without having procesed the parts.
            //      //See if it is multipart/alternative...
            //      if MessageParts.TextPartCount > 1 then begin
            //        if MessageParts.AttachmentCount > 0 then begin
            //          ContentType := 'multipart/mixed';    {do not localize}
            //        end else begin
            //          ContentType := 'multipart/alternative';   {do not localize}
            //        end;
            //      end else begin
            //        //Just one (or 0?) text part.
            //        if MessageParts.AttachmentCount > 0 then begin
            //          ContentType := 'multipart/mixed';    {do not localize}
            //        end else begin
            //          ContentType := 'text/plain';    {do not localize}
            //        end;
            //      end;
            //    end;
            //    TIdMessageEncoderInfo(MessageParts.MessageEncoderInfo).InitializeHeaders(Self);
            //  end;

            //  InitializeISO(TransferHeader, HeaderEncoding, ISOCharSet);
            //  LastGeneratedHeaders.Clear;

            //  if (FHeaders.Count > 0) then begin
            //    LastGeneratedHeaders.Assign(FHeaders);
            //  end;

            aWriter.WriteLine(HeaderLine("From", From.ToString()));
            //TODO: EncodeHeader(Subject, '', HeaderEncoding, TransferHeader, ISOCharSet);
            if (mSubject != null) {
                aWriter.WriteLine(HeaderLine("Subject", Subject));
            }
            //TODO:    Values['To'] := EncodeAddress(Recipients, HeaderEncoding, TransferHeader, ISOCharSet); {do not localize}
            aWriter.WriteLine(HeaderLine("To", AddressList(To)));
            if (CC.Count > 0) {
                aWriter.WriteLine(HeaderLine("CC", AddressList(CC)));
            }
            //    {CC: SaveToFile sets FGenerateBCCListInHeader to True so that BCC names are saved
            //     when saving to file and omitted otherwise (as required by SMTP)...}
            //    if FGenerateBCCListInHeader = False then begin
            //      Values['Bcc'] := ''; {do not localize}
            //    end else begin
            //      Values['Bcc'] := EncodeAddress(BCCList, HeaderEncoding, TransferHeader, ISOCharSet); {do not localize}
            //    end;
            //    Values['Newsgroups'] := NewsGroups.CommaText; {do not localize}
            //    if Encoding = meMIME then
            //    begin
            //      if DetermineIfMsgIsSinglePartMime = True then begin
            //        {This is a single-part MIME: the part may be a text part or an attachment.
            //        The relevant headers need to be taken from MessageParts[0].  The problem,
            //        however, is that we have not yet processed MessageParts[0] yet, so we do
            //        not have its properties or header content properly set up.
            //        So we will let the processing of MessageParts[0] append its headers to
            //        the message headers, i.e. DON'T generate Content-Type or Content-Transfer-Encoding
            //        headers here.}
            //        Values['MIME-Version'] := '1.0'; {do not localize}
            //      end else begin
            //        Values['Content-Type'] := ContentType;  {do not localize}
            //        if FCharSet > '' then begin
            //          Values['Content-Type'] := Values['Content-Type'] + ';' + EOL + TAB + 'charset="' + FCharSet + '"';  {do not localize}
            //        end;
            //        if MessageParts.Count > 0 then begin
            //          Values['Content-Type'] := Values['Content-Type'] + '; boundary="' + LMIMEBoundary + '"'; {do not localize}
            //        end;
            //        {CC2: We may have MIME with no parts if ConvertPreamble is True}
            //        Values['MIME-Version'] := '1.0'; {do not localize}
            //        Values['Content-Transfer-Encoding'] := ContentTransferEncoding; {do not localize}
            //      end;
            //    end else begin
            //      //CC: non-MIME can have ContentTransferEncoding of base64, quoted-printable...
            //      Values['Content-Transfer-Encoding'] := ContentTransferEncoding; {do not localize}
            //      Values['Content-Type'] := ContentType;  {do not localize}
            //    end;
            if (Sender.Email != "") {
                aWriter.WriteLine(HeaderLine("Sender", Sender.ToString()));
            }
            if (ReplyTo.Email != "") {
                aWriter.WriteLine(HeaderLine("Reply-To", ReplyTo.ToString()));
            }
            //    Values['Organization'] := EncodeHeader(Organization, '', HeaderEncoding, TransferHeader, ISOCharSet); {do not localize}

            //    Values['Disposition-Notification-To'] := EncodeAddressItem(ReceiptRecipient, {do not localize}
            //      HeaderEncoding, TransferHeader, ISOCharSet);

            //    Values['References'] := References; {do not localize}

            //    if UseNowForDate then begin
            //      Values['Date'] := Sys.DateTimeToInternetStr(Sys.Now); {do not localize}
            //    end else begin
            //      Values['Date'] := Sys.DateTimeToInternetStr(Self.Date); {do not localize}
            //    end;

            //    // S.G. 27/1/2003: Only issue X-Priority header if priority <> mpNormal (for stoopid spam filters)
            //    if Priority <> mpNormal then begin
            //      Values['X-Priority'] := Sys.IntToStr(Ord(Priority) + 1) {do not localize}
            //    end else begin
            //      if IndexOfName('X-Priority') >= 0 then begin  {do not localize}
            //        delete(IndexOfName('X-Priority'));    {do not localize}
            //      end;
            //    end;

            //    // Add extra headers created by UA - allows duplicates
            //    if (FExtraHeaders.Count > 0) then begin
            //      AddStrings(FExtraHeaders);
            //    end;
            //    {Generate In-Reply-To if at all possible to pacify SA.  Do this after FExtraHeaders
            //     added in case there is a message-ID present as an extra header.}
            //    if InReplyTo = '' then begin
            //      if Values['Message-ID'] <> '' then begin  {do not localize}
            //        Values['In-Reply-To'] := Values['Message-ID'];  {do not localize}
            //      end else begin
            //        {CC: The following was originally present, but it so wrong that it has to go!
            //        Values['In-Reply-To'] := Subject;   {do not localize}
            //      end;
            //    end else begin
            //      Values['In-Reply-To'] := InReplyTo; {do not localize}
            //    end;
            //  end;
            //end;
        }
    }
}
