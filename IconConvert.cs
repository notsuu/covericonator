﻿using System.Drawing;

namespace covericonator
{
    internal class IconConvert
    {
        // thx https://stackoverflow.com/questions/21387391
        internal static Icon IconFromImage(Image img)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            // Header
            bw.Write((short)0);   // 0 : reserved
            bw.Write((short)1);   // 2 : 1=ico, 2=cur
            bw.Write((short)1);   // 4 : number of images

            // Image directory
            var w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w);    // 0 : width of image
            var h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write(0);     // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data

            // Image data
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, SeekOrigin.Begin);

            return new Icon(ms);
        }

        internal static void WriteFile(Icon icon, string path)
        {
            using (FileStream s = File.Create(path))
            {
                icon.Save(s);
            }
        }
    }
}
