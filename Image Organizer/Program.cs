using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.IO;
using ServerLogClass;

namespace Image_Organizer
{
    class Program
    {
        const Boolean debug = true;
        static void Main(string[] args)
        {
            try
            {
                List<string> imageFiles = new List<string>(0); //In order

                System.Console.WriteLine("Jazy's Image Organizer app\nSearches current directory for images within folders and\ncopies them to an \"All Photos\" folder in order.\n\n");

                if (Directory.Exists("All Photos")) //If All Photos folder already exists delete it
                {
                    System.Console.WriteLine("***Found All Photos folder still present***\nDeleting...\n\n");
                    Directory.Delete("All Photos", true);
                    if (debug) Thread.Sleep(2000); //Look like its doing something
                    System.Console.WriteLine("Done\n");
                }

                string curDir = Directory.GetCurrentDirectory();
                System.Console.WriteLine("Working in: " + curDir + "\n");
                System.Console.WriteLine("Searching for images...\n\n");

                imageFiles = SearchForImages(curDir, imageFiles);
                imageFiles.TrimExcess();
                if (debug) Thread.Sleep(2000); //Look like its doing something

                System.Console.WriteLine("Done\n");

                if (imageFiles.Capacity < 1)
                {
                    System.Console.WriteLine("**No images were found**");
                }
                else
                {
                    DirectoryInfo newDir = Directory.CreateDirectory("All Photos");

                    System.Console.WriteLine("Found " + imageFiles.Capacity + " total images.\nCopying to " + newDir.ToString() + " folder...\n\n");
                    string fileName;
                    for (int i = 0; i < imageFiles.Capacity; i++)
                    {
                        fileName = newDir.FullName + Path.DirectorySeparatorChar + "IMG" + (i + 1) + ".jpg";
                        Debug.WriteLine("CAP:" + imageFiles.Capacity + " i:" + i);
                        File.Copy(imageFiles[i], fileName);
                    }

                    if (debug) Thread.Sleep(2000); //Look like its doing something
                    System.Console.WriteLine("Done\n");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ERROR: " + e.Message);
                System.Console.WriteLine("Program aborted\n\n");
                SendLog(Environment.MachineName + " image organizer error.\nWorking directory: " + Environment.CurrentDirectory + "\nError: " + e.ToString());
            }
            //TODO Delete the old files?
            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey(true);
        }

        static void SendLog(string msg)
        {
            LogClient.SendMessage(msg, new LogClient.MessageSentEvent((b, s) => { Debug.WriteLine(b + ": " + s); })); //Test
        }

        static List<string> SearchForImages(string dirPath, List<string> imageFiles)
        {
            try
            {
                string[] files = Directory.GetFiles(dirPath, "*.jpg"); //Only images
                string[] dirs = Directory.GetDirectories(dirPath);
                //Add found images to our array
                foreach (string s in files)
                {
                    Debug.WriteLine("Adding file: " + s);
                    imageFiles.Add(s);
                }
                //Search subdirectories
                foreach (string s in dirs)
                {
                    Debug.WriteLine("Searching dir: " + s);
                    imageFiles = SearchForImages(s, imageFiles);
                }
                if (files.Length > 0)
                {
                    System.Console.WriteLine("Found " + files.Length + " images in \"" + dirPath.Substring(dirPath.LastIndexOf(Path.DirectorySeparatorChar)+1) + "\"");
                    if (debug) Thread.Sleep(800);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return imageFiles;
        }
        /*
        static void removeDirectory(string dirName)
        {
            string[] files = Directory.GetFiles(dirName);
            string[] dirs = Directory.GetDirectories(dirName);
            foreach (string s in files)
            {
                File.Delete(s);
            }
            foreach (string s in dirs)
            {
                removeDirectory(s);
            }
            Directory.Delete(dirName);
        }
         */

    }
}
