using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Net
{
    public class Email
    {
        TcpClient mailclient;

        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;

        string hostname ="mail.outcode.pt";
        string username = "afonso.sousa@outcode.pt";
        Int32 serverport = 110;
        string password = "+pop-2015";

        StringBuilder status = new StringBuilder();

        public void ReadMail()
        {
            string response;
            string from = null;
            string subject = null;
            int totmessages;

            try
            {
                mailclient = new TcpClient(hostname, serverport);
            }
            catch (SocketException)
            {
                status.AppendLine( "Unable to connect to server");
                return;
            }

            ns = mailclient.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);

            response = sr.ReadLine(); //Get opening POP3 banner

            sw.WriteLine("User " + username); //Send username
            sw.Flush();

            response = sr.ReadLine();
            if (response.Substring(0, 4) == "-ERR")
            {
                status.AppendLine ( "Unable to log into server");
                return;
            }

            sw.WriteLine("Pass " + password);  //Send password
            sw.Flush();

            try
            {
                response = sr.ReadLine();
            }
            catch (IOException)
            {
                status.AppendLine ( "Unable to log into server");
                return;
            }

            if (response.Substring(0, 3) == "-ER")
            {
                status.AppendLine ( "Unable to log into server");
                return;
            }

            sw.WriteLine("stat"); //Send stat command to get number of messages
            sw.Flush();

            response = sr.ReadLine();
            string[] nummess = response.Split(' ');
            totmessages = Convert.ToInt16(nummess[1]);
            if (totmessages > 0)
            {
                status.AppendLine ( "you have " + totmessages + " messages");
            }
            else
            {
                status.AppendLine( "You have no messages");
            }

            for (int i = 1; i <= totmessages; i++)
            {
                sw.WriteLine("top " + i + " 0"); //read header of each message
                sw.Flush();
                response = sr.ReadLine();

                while (true)
                {
                    response = sr.ReadLine();
                    if (response == ".")
                        break;
                    if (response.Length > 4)
                    {
                        if (response.Substring(0, 5) == "From:")
                            from = response;
                        if (response.Substring(0, 8) == "Subject:")
                            subject = response;
                    }
                }
                status.AppendLine (i + "  " + from + "  " + subject);
            }
        }
    }
    }
