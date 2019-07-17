using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA5NameChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            Process process = Process.GetProcessesByName("notepad.exe").FirstOrDefault();
            ProcessMemoryReader memoryReader = new ProcessMemoryReader();
            memoryReader.ReadProcess = process;
            memoryReader.OpenProcess();
        }
    }
}
