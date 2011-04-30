using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Encoder = System.Drawing.Imaging.Encoder;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Shredder
{
    class Program
    {
        public static HashSet<string> files = new HashSet<string>();

        static void Main(string[] args)
        {
            while (true)
            {
                var di = new DirectoryInfo("C:\\drop\\");

                foreach (var fi in di.GetFiles("*.jpg"))
                {
                    if (files.Contains(fi.FullName) == false)
                    {
                        files.Add(fi.FullName);
                        new ImageShredder().Process(fi);
                    }
                }

                System.Threading.Thread.Sleep(1000);
            }


        }
        //private static void ResizeWic(byte[] buf, int size)
        //{

        //    var dv = new DrawingVisual();
        //    var ms = new MemoryStream(buf);
        //    var jpeg = new JpegBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);


        //    var bf = jpeg.Frames[0];

        //    var px = GetDimensions(bf.PixelHeight, bf.PixelWidth, size);
      
        //    var group = new DrawingGroup();
        //    RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);

        //    var output = new RenderTargetBitmap(px.X, px.Y, (double)96, (double)96, System.Windows.Media.PixelFormats.Default);


        //    group.Children.Add(new ImageDrawing(bf, new System.Windows.Rect(0, 0, px.X, px.Y)));

        //    using (DrawingContext dc = dv.RenderOpen())
        //    {

        //        dc.DrawDrawing(group);


             

        //    }
        //    output.Render(dv);
        //    var output_bm = BitmapFrame.Create(output);
        //    var encoder = new JpegBitmapEncoder();

        //    encoder.QualityLevel = 97;
        //    encoder.Frames.Add(output_bm);

        //    using (var fs = new FileStream(string.Format("c:\\out_wic_{0}_97.jpg", size), FileMode.Create))
        //    {
        //        encoder.Save(fs);
        //    }

        //}

    

    }
}
