using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorySize
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Console.ReadLine();
            DirSize.GetDirSizeAsync(path, updateSize);
            Console.ReadKey();
        }

        static void updateSize(long filesize, bool done)
        {
            Console.Write((char)13 + "Size is " + filesize + new string(' ',20));
        }
    }
}
