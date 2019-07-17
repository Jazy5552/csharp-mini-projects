using System;
using System.Threading;
using System.IO;

namespace DirectorySize
{
    class DirSize
    {
        public static void GetDirSizeAsync(string path, FileSizeUpdateCallback fsuc)
        {
            Thread worker = new Thread(new ThreadStart(() =>
            {
                long dirsize = 0;
                fsuc(dirsize, false);
                GetDirSizeWithCallback(path, fsuc, ref dirsize);
                fsuc(dirsize, true);
            }));
            worker.Start();
        }
        public delegate void FileSizeUpdateCallback(long filesize, bool done);
        //Returns the callback everytime a file or directory is scanned.
        private static void GetDirSizeWithCallback(string path, FileSizeUpdateCallback fsuc, ref long totalsize)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                GetDirSizeWithCallback(dir, fsuc, ref totalsize);
                //System.Diagnostics.Debug.WriteLine(dir + ":" + dirSize);
            }
            foreach (string filename in Directory.GetFiles(path))
            {
                long fileSize = GetFileSize(filename);
                //System.Diagnostics.Debug.WriteLine(filename + ":" + fileSize);
                if (fileSize != 0)
                { fsuc(totalsize += fileSize, false); }
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
            foreach (string dir in Directory.GetDirectories(path))
            {
                totalsize += GetDirSize(dir);
            }
            foreach (string filename in Directory.GetFiles(path))
            {
                totalsize += GetFileSize(filename);
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
