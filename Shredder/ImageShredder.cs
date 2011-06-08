using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GettyImages.Editorial.App.Image;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using WPD.Shared;

namespace Shredder
{
    public class ImageShredder
    {
        private byte[] _original;
        private byte[] _thumb;
        private byte[] _preview;


        private AssetPack _asset;
        private FileInfo _fi;

        public void Process(FileInfo fi)
        {
            _asset = new AssetPack();
            _fi = fi;

            Console.WriteLine("Processing {0}", _fi.Name);

            _asset.OriginalFileName = _fi.Name;
            _asset.IngestDate = DateTime.UtcNow;
            _asset.Id = ObjectId.GenerateNewId();
            _asset.IngestMachine = Environment.MachineName;
            _asset.IngestPath = _fi.Directory.FullName;

            long currentLength = fi.Length;


           var root = Task.Factory.StartNew(() =>
            {
                FileStream fs;

                try
                {

                    System.Threading.Thread.Sleep(1000);

                    if (currentLength != fi.Length)
                        return;

                    fs = new FileStream(_fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return;
                }

                _original = new byte[fs.Length];

                var task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, _original, 0, (int)fs.Length, fs);

                task.ContinueWith((after) =>
                                      {
                                          ((FileStream)after.AsyncState).Close();
                                          _fi.MoveTo(string.Format("c:\\backup\\{0}.jpg", _asset.Id.ToString()));
                                      });
                task.ContinueWith((after) =>
                                      {
                                         
                                                          WriteGridFs(0, _original);
                                      });

                task.ContinueWith((after) =>
                                      {
                                          Task.Factory.StartNew<MetaData>(ReadMetadata).ContinueWith(a=> Save(a.Result));
                                      });
                task.ContinueWith((after) =>
                                      {
                                          var t2 =
                                              Task<MemoryStream>.Factory.StartNew(() => Resize(200)).ContinueWith(
                                                  (s2) =>
                                                  {
                                                      if (s2.Result != null)
                                                      {
                                                          WriteGridFs( 200,  s2.Result.GetBuffer());
                                                          /*WriteFile(after.Result, _fi, 600) */
                                                      }
                                                  });
                                          var t6 = Task<MemoryStream>.Factory.StartNew(() => Resize(600)).ContinueWith(
                                              s6 =>
                                              {
                                                  if (s6.Result != null)
                                                  {
                                                      WriteGridFs(600,  s6.Result.GetBuffer());
                                                  }
                                                  /* WriteFile(after.Result, _fi, 200) */
                                              });
                                      });
            });
            

        }

        private void Save(MetaData data)
        {
         
            var db = Mongo.GetDatabase();

            var coll = db.GetCollection("images");
            _asset.ImageMetadata = data;
            coll.Insert(_asset);

  
        }
        private void WriteGridFs(int size, byte[] buffer)
        {
            var db = Mongo.GetDatabase();
            var gfs = db.GridFS;
            var s = new MemoryStream(buffer);
            var opts = new MongoGridFSCreateOptions();

            //opts.ChunkSize = 2408;
            opts.Metadata = new BsonDocument() {{"size", size}, {"asset", _asset.OriginalFileName} };

            gfs.Upload(s, string.Format("{0}_{1}.jpg", _asset.Id, size), opts);
            // var upload = gfs.OpenWrite(string.Format("{0}_{1}.jpg", _asset.Id, size), opts);

            // upload.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(onComplete), upload);

            //Task.Factory.FromAsync(upload.BeginWrite, upload.EndWrite, buffer, 0, buffer.Length, upload)
            //    .ContinueWith(t =>
            //                      {
            //                          var fs = (MongoGridFSStream)t.AsyncState;

            //                          fs.Close();
            //                      }).Wait();
        }
        private void onComplete(IAsyncResult result)
        {
            var fs = (MongoGridFSStream)result.AsyncState;
        
            fs.EndWrite(result);

            fs.Close();
        }
        private MetaData ReadMetadata()
        {
            try
            {
                var buf = new byte[_original.Length];
                _original.CopyTo(buf, 0);
               
                var ms = new MemoryStream(buf, 0, buf.Length);

              return new GettyImages.Editorial.App.Image.WICMetaDataHandler().GetMetaData(ms);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception reading metadata: {0}", ex.Message);
            }
            return null;
        }

        private Task WriteFile(MemoryStream result, FileInfo fi, int size)
        {

            if (result == null)
                return null;
            var db = Mongo.GetDatabase();

            var gfs = db.GridFS;

            return Task.Factory.StartNew(() => gfs.Upload(result, string.Format("{0}_{1}.jpg", fi.Name.Substring(0, fi.Name.IndexOf(".")), size)));

        }


        private static Point GetDimensions(int height, int width, int size)
        {
            int h = height > width ? size : size * height / width;
            int w = width > height ? size : size * width / height;

            return new Point(w, h);
        }
        private MemoryStream Resize(int size)
        {
            try
            {
                //   var bytes = new byte[_original.Length];
                // _original.CopyTo(bytes, 0);

                var ms = new MemoryStream(_original, 0, _original.Length, false, true);
                Image image;

                image = Image.FromStream(ms); // Image.FromStream(ms, false, false);

                var px = GetDimensions(image.Height, image.Width, size);

                var codec = getEncoderInfo("image/jpeg");


                var quality = new EncoderParameter(Encoder.Quality, (long)97);
                var p = new EncoderParameters(1);


                p.Param[0] = quality;

                var out_image = new Bitmap(px.X, px.Y);

                //  out_image_600.Palette = image.Palette

                Graphics g = Graphics.FromImage(out_image);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                g.DrawImage(image, new Rectangle(0, 0, px.X, px.Y), new Rectangle(0, 0, image.Width, image.Height),
                            GraphicsUnit.Pixel);

                var outStream = new MemoryStream();

                out_image.Save(outStream, codec, p);

                return outStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Resize on {0} to {1} failed. Exception: {2}", _asset.OriginalFileName, size, ex.Message);
                return null;
            }
        }

        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

       
    }
}
