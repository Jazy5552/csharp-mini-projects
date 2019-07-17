using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FindEmptyFolders
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO Make cute gui
            //NOTE The search shows empty parent directories and their empty children as well (Want to only show empty parent)
            Console.WriteLine("Enter the desired extensions to search for \n"+
                "(If they are not found in a folder then the folder will be considered empty and subfolders count towards not empty)\n"+
                "in the following manner  .jpg .mp3 .flac .wav .wma .jpg .gif .png .txt  \n"+
                "Leaving a space between each extension");
            Console.Write(":");
            string input = Console.ReadLine();
            string[] exts = ParseExtensions(input);

            Console.WriteLine("\nEnter the parent directory");
            Console.Write(":");
            string dir = Console.ReadLine();
            
            Console.Write("Searching...");
            List<string> emptyDirs = new List<string>(0);
            IsEmpty(dir, emptyDirs, exts);
            Console.WriteLine(" Done\n");

            Console.WriteLine("Empty Directories:");
            foreach (string s in emptyDirs)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("\n\nDelete the empty folders (y/n)?");
            char an  = Console.ReadKey(false).KeyChar;
            if (an == 'y')
            {
                Console.Write("\nRemoving empty folders...");
                foreach (string s in emptyDirs)
                {
                    RemoveDirectory(s);
                }
            }
            Console.WriteLine(" Done\n");
            Console.ReadKey(true);
        }

        public static void RemoveDirectory(string dir)
        {
            try
            {
                string[] childDirs = Directory.GetDirectories(dir);
                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                foreach (string d in childDirs)
                {
                    RemoveDirectory(d);
                }
                Directory.Delete(dir);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private static bool IsEmpty(string dir, List<string> emptyFolders, params string[] exts)
        {
            bool empty = false;
            if (Directory.Exists(dir))
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(dir);
                    string[] files = Directory.GetFiles(dir);
                    bool childempty = true;
                    empty = true;

                    foreach (string d in dirs)
                    {
                        if (!IsEmpty(d, emptyFolders, exts))
                        {
                            childempty = false;
                        }
                    }

                    foreach (string f in files)
                    {
                        foreach (string ext in exts)
                        {
                            if (f.EndsWith(ext))
                            {
                                empty = false;
                            }
                        }
                    }

                    if (empty && childempty)
                    {
                        if (emptyFolders.Count > 0 && emptyFolders.Last().Substring(0, emptyFolders.Last().LastIndexOf(Path.DirectorySeparatorChar)) == dir)
                        {
                            emptyFolders.RemoveAt(emptyFolders.Count - 1);
                        }
                        emptyFolders.Add(dir);
                        
                    }
                    else if (!childempty)
                    {
                        empty = false;
                    }

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            return empty;
        }

        private static string[] ParseExtensions(string s)
        {
            string[] exts = new string[0];
            if (s.Contains('.'))
            {
                s = s.Trim();
                exts = s.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            }
            foreach (string ss in exts)
            {
                System.Diagnostics.Debug.WriteLine(ss);
            }
            return exts;
        }
    }
}
