using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using MongoDB.Driver.Builders;
using WPD.Shared;

namespace WPD.Web.Endpoints
{
    public class MongoImage : Nina.Application
    {
        public MongoImage()
        {
            Get("", (m, c) =>
                        {
                            var db = Mongo.GetDatabase();

                            //var images = db.GridFS.Find(Query.EQ("metadata.size", 200)).OrderByDescending(t => t.UploadDate).Select(x => new { name = x.Name, id = x.Id.ToString() });

                            var images =
                                db.GetCollection<AssetPack>("images").FindAll().OrderByDescending(t => t.IngestDate).
                                Select(x => new { Id = x.Id.ToString(), AssetPack = x });

                            return Json(images).ETagged();

                        });
            Post("", (m, c) =>
                         {
                             return Nothing();
                         });
            Get("{id}.jpg", (m, c) =>
                                {
                                    var db = Mongo.GetDatabase();

                                    // var image = db.GridFS.FindOne(path.Substring(1, path.Length -1));


                                    var ms = new MemoryStream();

                                    try
                                    {
                                        db.GridFS.Download(ms, m["id"] + ".jpg");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.Message);
                                    }
                                    var buf = ms.GetBuffer();
                                    ;
                                    return File(buf, "image/jpeg").ETagged();
                                });

        }


    }
}