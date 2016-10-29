using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Security;
using Core.IO;  

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Please enter a NUIT to use:");
            string nuit = Console.ReadLine();
            Console.WriteLine("Please enter a string to encrypt:");
            string plaintext = Console.ReadLine();
            Console.WriteLine("");

            IniFile ini = new IniFile(String.Format("{0}.lic", nuit));

            Console.WriteLine("Your encrypted string is:");
            string encryptedstring = StringCipher.Thing(plaintext, nuit);
            string encryptedKey = StringCipher.Thing(nuit,"CSULICENSE.LIC"); 
            Console.WriteLine(encryptedstring);
            Console.WriteLine("");

            ini.Write("L0", encryptedKey, "GALU");
            ini.Write("L1", encryptedstring,"GALU"); 

            //encryptedstring = ini.Read("L1", "Licence");

            //Console.WriteLine("Your decrypted string is:");
            //string decryptedstring = StringCipher.What (encryptedstring, password);
            //Console.WriteLine(decryptedstring);
            //Console.WriteLine("");

            encryptedstring = ini.Read("L1","GALU");

            Console.WriteLine("Your decrypted string is:");
            string decryptedstring = StringCipher.What(encryptedstring, nuit);
            Console.WriteLine(decryptedstring);
            Console.WriteLine("");

            Console.ReadKey();

        }
    }
}
