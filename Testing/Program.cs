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

            IniFile ini = new IniFile("mylic.lic");

            Console.WriteLine("Please enter a password to use:");
            string password = Console.ReadLine();
            Console.WriteLine("Please enter a string to encrypt:");
            string plaintext = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("Your encrypted string is:");
            string encryptedstring = StringCipher.Thing(plaintext, password);
            Console.WriteLine(encryptedstring);
            Console.WriteLine("");


            ini.Write("L1", encryptedstring,"Licence"); 

            encryptedstring = ini.Read("L1", "Licence");

            Console.WriteLine("Your decrypted string is:");
            string decryptedstring = StringCipher.What (encryptedstring, password);
            Console.WriteLine(decryptedstring);
            Console.WriteLine("");

            encryptedstring = ini.Read("L1");

            Console.WriteLine("Your decrypted string is:");
            decryptedstring = StringCipher.What(encryptedstring, password);
            Console.WriteLine(decryptedstring);
            Console.WriteLine("");

            Console.ReadKey();

        }
    }
}
