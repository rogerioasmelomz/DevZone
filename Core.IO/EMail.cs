using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.IO
{
    class EMail
    {

        #region "PRIVATE VARS"

        private SmtpClient client;

        private MailMessage mail;

        #endregion

        #region "PROPERTIES"

        public string SMTPHost
        {
            get { return client.Host; }
            set { client.Host = value; }
        }

        public int SMTPPort
        {
            get { return client.Port; }
            set { client.Port = value; }
        }

        public string From
        {
            get { return mail.From.ToString() ; }
            set { mail.From = new MailAddress(  value); }
        }

        public string To
        {
            get { return mail.To.ToString (); }
            set { mail.To.Add ( value); }
        }

        public string Body
        {
            get { return mail.Body; }
            set { mail.Body = value; }
        }

        public string subject
        {
            get { return mail.Subject; }
            set { mail.Subject = value; }
        }

        public bool EnableSSL
        {
            get { return client.EnableSsl; }
            set { client.EnableSsl = value; }
        }
        #endregion

        #region "CONSTRUCTORS"

        public EMail() {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            mail = new MailMessage();
        }

        #endregion

        #region "SEND MAIL"

        public void Send()
        {
            try
            { 
                client.Send(mail);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Attach a file to mail message
        /// </summary>
        /// <param name="FullPathFile">Full path to file to be used to create Attach</param>
        public void AttachFile(string FullPathFile)
        {
            try
            {
                mail.Attachments.Add(new Attachment(FullPathFile));
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        #endregion

        #region "READ MAIL"



        #endregion

        #region "SHARED TOOLS"

        public static bool IsEmailFormat(string Email)
        {

            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

            return (Regex.IsMatch(Email, pattern));
        } 

        #endregion

    }
}
