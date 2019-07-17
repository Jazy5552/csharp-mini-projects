using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encrypter
{
    class Encrypter
    {
        public static int StringtoInt(string mes)
        {
            return Encoding.ASCII.GetByteCount(mes);
        }

        public static string Decrypt(string message, int key)
        {
            return Encrypt(message, (key * -1));
        }

        public static string Encrypt(string message, int key)
        {
            //System.Diagnostics.Debug.WriteLine("Key:{0} Encrypting:{1}", key, message);
            char[] msg = message.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == 10 || msg[i] == 13)
                {
                    //It's a newline char, ignore it
                }
                else if (msg[i] < 0)
                {
                    //It's an end of file char, set to zero and gtfo
                    msg[i] = (char)0;
                    return new string(msg);
                }
                else if ((msg[i] < 33) || (msg[i] > 126))
                {
                    //It's a weird character so we will leave it alone.
                }
                else if (msg[i] + key < 127 && msg[i] + key > 32)
                {
                    //Let's displace. Within our range.
                    msg[i] = (char)(msg[i] + key);
                }
                else if (msg[i] + key > 126)
                {
                    //The displacment is going past our range
                    do
                    {
                        msg[i] = (char)(msg[i] - (127 - 33));
                    } while (msg[i] + key > 127 && msg[i] + key < 32);
                    msg[i] = (char)(msg[i] + key);
                }
                else if (msg[i] + key < 33)
                {
                    //The displacment is going below our range
                    do
                    {
                        msg[i] = (char)(msg[i] + (127 - 33));
                    } while (msg[i] + key > 127 && msg[i] + key < 32);
                    msg[i] = (char)(msg[i] + key);
                }
                else
                {
                    //idk..
                    //Console.WriteLine("Error encrypting: {0}", msg[i]);
                }
            }
            return new string(msg);
        }
    }
}
