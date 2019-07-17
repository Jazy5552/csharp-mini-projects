using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
//using MySql.Data.MySqlClient;

namespace MySQL_App
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string registrySubKey = @"SYSTEM\CurrentControlSet\Control\Session Manager";
            string valueName = "PendingFileRenameOperations";

            RegistryKey rk = Registry.LocalMachine.OpenSubKey(registrySubKey, true);
            string[] values = (string[])rk.GetValue(valueName);
            string[] newValues = new string[values.Length + 2];


            for (int i = 0; i < values.Length; i++)
            {
                newValues[i] = values[i];
            }
            newValues[newValues.Length - 2] = @"\??\" + System.Reflection.Assembly.GetExecutingAssembly().Location;
            newValues[newValues.Length - 1] = "";
            rk.SetValue(valueName, newValues);

            foreach (string s in newValues)
            {
                Console.WriteLine(Encoding.ASCII.GetByteCount(s));
            }

            Console.WriteLine(new string('-', 20));

            foreach (string s in newValues)
            {
                foreach (char c in s)
                {
                    Console.Write((uint)c + "-");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
