using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Logging_Server
{
    class Program
    {
        static Object myLock = new Object(); //Make sure only one thread is trying to write to file.
        static Int64 counter = 0;
        //TODO Make it so if another one runs the older one closes down.
        static int serverPort = 8008;
        static string fileName = "ServerLogger_" + DateTime.Now.ToString("M_d_yy_HH-mm-ss") + ".log";

        static void Main(string[] args)
        {
            if (PrintHelp(args))
                return;
            //File.Create(fileName);
            if (args.Length > 0)
            {
                try
                {
                    string tmpfileName = args[0];
                    fileName = tmpfileName;
                    int tmpPort = int.Parse(args[1]);
                    serverPort = tmpPort;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }

            try
            {
                TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, serverPort));
                listener.Start(10);
                listener.BeginAcceptTcpClient(OnAccept, listener);
                Console.WriteLine("CTRL + C to exit. Writing to file: " + fileName + "\nListening on port " + serverPort + "\nCommandline: appname LOGFILENAME LISTENPORTNUMBER");
                ManualResetEvent mre = new ManualResetEvent(false);
                mre.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        static private bool PrintHelp(string[] args)
        {
            foreach (string a in args)
            {
                if (a.ToLower().Contains("?") || a.ToLower().Contains("-h"))
                {
                    Console.WriteLine("Commandline arguments ( [] optional ):\n\tappname.exe [filepathforlog] [port]");
                    return true;
                }
            }
            return false;
        }

        static void OnAccept(IAsyncResult iar)
        {
            TcpListener list = (TcpListener)iar.AsyncState;
            try
            {
                TcpClient client = list.EndAcceptTcpClient(iar);
                string remoteip = client.Client.RemoteEndPoint.ToString();
                log(++counter + ": " + remoteip + " has connected at " + DateTime.Now.ToString());
                Thread t = new Thread(() =>
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            byte[] buffer = new byte[1024];
                            int bytesread;
                            while ((bytesread = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                string packet = new String(' ', 3) + Encoding.ASCII.GetString(buffer, 0, bytesread);
                                packet = packet.Replace("\r\n", "\r\n   "); //Look perty
                                packet = packet.Replace("\n", "\n   ");
                                log(packet);
                                if (packet.ToLower().Contains("quit") || packet.ToLower().Contains("exit"))
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.ToString());
                            Console.WriteLine(e.ToString());
                        }
                        client.Close();
                    }
                );
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                Console.WriteLine(e.ToString());
            }
            list.BeginAcceptTcpClient(OnAccept, list);
        }

        static void log(string msg)
        {
            if (msg == "")
                return;
            lock (myLock)
            {
                try
                {
                    File.AppendAllText(fileName, msg + "\n");
                    Console.Write(msg + "\n");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    Console.WriteLine(e.ToString());
                }
            }
        }

    }
}
