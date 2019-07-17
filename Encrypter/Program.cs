using System;
using System.Text;
using System.IO;

namespace Encrypter
{
    class Program
    {
        static bool fileOutMode = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a string and hit enter to encrypt.\nEnter a string starting with / to decrpt.\nEnter :file: followed by a filename/path to encrypt the file's text.\n"+
            "Enter /:file: followed by a filename/path to decrypt the file's text.\n\nEnter -f to toggle output to file.\nEnter -q to quit.\n");
            
            string input;
            do
            {
                input = Console.ReadLine();
                if (input.StartsWith("-q"))
                {
                    //Quit
                }
                else if (input.StartsWith("-f"))
                {
                    fileOutMode = true;
                    Console.WriteLine("File Output Mode:{0}", fileOutMode);
                }
                else if (input.StartsWith(":file:"))
                {
                    string fileName = input.Substring(6);
                    int k = GetKey();
                    string msg = Encrypter.Encrypt(File.ReadAllText(fileName), k);
                    PrintLine("Encrypted: " + msg);
                }
                else if (input.StartsWith("/:file:"))
                {
                    string fileName = input.Substring(7);
                    int k = GetKey();
                    string msg = Encrypter.Decrypt(File.ReadAllText(fileName), k);
                    PrintLine("Decrypted: " + msg);
                }
                else if (input.StartsWith("/"))
                {
                    int k = GetKey();
                    string msg = Encrypter.Decrypt(input.Substring(1), k);
                    PrintLine("Decrypted: " + msg);
                }
                else
                {
                    int k = GetKey();
                    string msg = Encrypter.Encrypt(input, k);
                    PrintLine("Encrypted: " + msg);
                }
            } while (input != "-q");
        }

        private static int GetKey()
        {
            Console.Write("\nEnter a key (int):");
            int key = (int)DateTime.Now.Ticks % 64;
            try
            {
                key = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            Console.WriteLine("Using Key:{0}", key);
            return key;
        }

        private static void PrintLine(string msg)
        {
            Print(msg + "\r\n");
        }

        private static void Print(string msg)
        {
            if (fileOutMode)
            {
                try
                {
                    File.AppendAllText("log.txt", msg + "\r\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Console.Write(msg);
            }
        }
    }
}
