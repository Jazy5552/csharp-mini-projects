using System;
using System.Threading;
using System.IO;

namespace DirectorySize
{
    class DirSize
    {
        public static void GetDirSizeAsync(string path, FileSizeUpdateCallback fsuc, Object extradata) //returns the thread incase of kill
        {
            Thread worker = new Thread((Object wThread) =>
                {
                    try
                    {
                        long dirsize = 0;
                        fsuc(dirsize, false, extradata, (Thread)wThread);
                        GetDirSizeWithCallback(path, fsuc, ref dirsize, extradata, (Thread)wThread);
                        fsuc(dirsize, true, extradata, (Thread)wThread);
                    }
                    catch (Exception)
                    {
                        //Catch any exception that made it this far up (most likely an abort exception meaning to end the thread)
                        //Let the callback know so as to kill the thread "nicely"
                        fsuc(0, true, null, null);
                    }
                });
            worker.IsBackground = true;
            worker.Start(worker);
        }
        public delegate void FileSizeUpdateCallback(long filesize, bool done, Object extradata , Thread workerThread);
        //Returns the callback everytime a file or directory is scanned.
        private static void GetDirSizeWithCallback(string path, FileSizeUpdateCallback fsuc, ref long totalsize, Object extradata, Thread workerThread)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    GetDirSizeWithCallback(dir, fsuc, ref totalsize, extradata, workerThread);
                    //System.Diagnostics.Debug.WriteLine(dir + ":" + dirSize);
                }
                foreach (string filename in Directory.GetFiles(path))
                {
                    long fileSize = GetFileSize(filename);
                    //System.Diagnostics.Debug.WriteLine(filename + ":" + fileSize);
                    if (fileSize != 0)
                    { fsuc(totalsize += fileSize, false, extradata, workerThread); }
                }
            }
            catch (ThreadInterruptedException tie)
            {
                //Handle the interruption as a call to kill the thread "nicely"
                throw tie;
            }
            catch (Exception e)
            {
                //Carry on ignoring the silly error
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        //Returns size of directory in bytes
        public static long GetDirSize(string path)
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }
            long totalsize = 0;
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    totalsize += GetDirSize(dir);
                }
                foreach (string filename in Directory.GetFiles(path))
                {
                    totalsize += GetFileSize(filename);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return totalsize;
        }

        public static long GetFileSize(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return 0;
            }
            FileInfo fi = new FileInfo(filepath);
            return fi.Length;
        }
    }
}
