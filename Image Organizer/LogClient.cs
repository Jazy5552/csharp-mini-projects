using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerLogClass
{
    public class LogClient
    {
        public delegate void MessageSentEvent(bool success, string error);

        public static void SendMessage(string msg, MessageSentEvent mse)
        {
            //Returns nothing
            Thread t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(msg);
                        client.Connect("jazyserver.myftp.org", 8008);
                        NetworkStream ns = client.GetStream();
                        ns.Write(buffer, 0, buffer.Length);
                    }
                    mse(true, null);
                }
                catch (Exception e)
                {
                    mse(false, e.ToString());
                }
            }));
            t.IsBackground = false;
            t.Start();
            //TODO Make background function and sync one
        }
    }
}
