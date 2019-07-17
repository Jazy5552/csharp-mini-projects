using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Zipper
{
    class Photos
    {
        List<Bitmap> photos;
        private List<String> photosPaths;

        public Photos(String[] photos)
        {
            this.photos = new List<Bitmap>(photos.Length);
            this.photosPaths = new List<String>(photos.Length);
            foreach (String f in photos)
            {
                this.photos.Add(new Bitmap(f));
                this.photosPaths.Add(f);
            }
        }

        public void ZipPhotos(String outFile, long compLvl)
        {
            //First compress the files using jpeg compression
            List<String> files = CompressPhotos(compLvl);
            //Now move them into a temp directory
            String tempPath = Path.GetTempPath() + Path.GetRandomFileName() + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(tempPath);
            for (int i=0; i<photosPaths.Count; i++)
            {
                File.Move(files[i], tempPath + Path.GetFileName(photosPaths[i]));
            }
            //Replace file if exists
            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }
            //Now zip the file and move it to the outFile
            System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, outFile);
        }

        public long GetExpectedTotalSize(long compLvl)
        {
            String filePath = Path.GetTempPath() + Path.GetRandomFileName();
            ZipPhotos(filePath, compLvl);
            FileInfo file = new FileInfo(filePath);
            return file.Length;
        }

        public long GetRealTotalSize()
        {
            long size = 0;
            foreach (String f in photosPaths)
            {
                FileInfo file = new FileInfo(f);
                size += file.Length;
            }
            return size;
        }

        private List<String> CompressPhotos(long compLvl)
        {
            List<String> compFiles = new List<String>(photos.Count);
            //Compress the photos using given compression level and return a string array of the filenames
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            //Create the parameter that will hold the quality of the jpeg
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compLvl);
            //PLace the one parameter into the myEncoderParameters array
            myEncoderParameters.Param[0] = myEncoderParameter;
            foreach (Bitmap pic in photos)
            {
                String fileName = Path.GetTempFileName();
                pic.Save(fileName, jpgEncoder, myEncoderParameters);
                compFiles.Add(fileName);
            }
            return compFiles;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public List<String> GetPhotoPaths()
        {
            return photosPaths;
        }
    }
}
