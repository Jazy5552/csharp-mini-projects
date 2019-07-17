using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Mouse_Macro
{
    class MouseMacro
    {
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        static void Main(string[] args)
        {
            new MouseMacro(args);
            Console.ReadKey();
        }

        public MouseMacro(string[] args)
        {
            Args a = new Args(false, null);
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string s = args[i];
                    if (s.Contains("-?") || s.ToLower().Contains("-h"))
                    {
                        PrintHelp();
                        return;
                    }
                    else if (s.ToLower().Contains("-r"))
                    {
                        a.recording = true;
                    }
                    else if (s.ToLower().Contains("-f"))
                    {
                        string path = args[i + 1];
                        path = path.Replace('"', ' ');
                        path = path.Trim();
                        if (!File.Exists(path))
                        {
                            throw new Exception();
                        }
                        a.filePath = path;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Incorrect arguments! " + e.Message);
                PrintHelp();
                return;
            }

            if (a.filePath == null)
            {
                //No filepath was supplied so assume recording mode and set default file path
                a.recording = true;
                a.filePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "MouseMacro-" +  DateTime.Now.ToString("M-d-HH-mm");
            }

            Console.WriteLine("Record mode:" + a.recording + "\nFile path:" + a.filePath);

            if (a.recording)
            {

            }
        }

        private void Record(Args a)
        {
            string path = a.filePath;
            try
            {

            }
            catch (Exception e)
            {

            }
        }

        private struct Args
        {
            public bool recording;
            public string filePath;
            public Args(bool r, string f)
            {
                this.recording = r;
                this.filePath = f;
            }
        }

        private void PrintHelp()
        {
            Console.WriteLine("Jazy's Mouse Macro App\nCommand line options:\n" +
                "-h\tDisplays this help prompt\n" +
                "-r\tEnables record mode and output to file\n" +
                "-f \"filepath\"\tSpecify \"filepath\" to output mouse records\n\tor if -r omited then will playback file\n");
        }
    }
}
