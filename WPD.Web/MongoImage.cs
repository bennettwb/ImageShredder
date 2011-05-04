using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MongoDB.Driver.Builders;
using WPD.Shared;

namespace WPD.Web
{
    public class MongoImage : Nina.Application
    {
        public MongoImage()
        {
            Get("", (m, c) =>
                        {
                            var db = Mongo.GetDatabase();

                            var images = db.GridFS.Find(Query.EQ("metadata.size", 200)).OrderByDescending(t => t.UploadDate).Select(x => new { name = x.Name, id = x.Id.ToString() });

                            return Json(images).ETagged();
         
                        }); 

            Get("{id}.jpg", (m,c) =>
                                {
                                    var db = Mongo.GetDatabase();

                                    // var image = db.GridFS.FindOne(path.Substring(1, path.Length -1));


                                    var ms = new MemoryStream();

                                    
                                    db.GridFS.Download(ms, m["id"] + ".jpg");

                                    var buf = ms.GetBuffer();
;
                                    return File(buf, "image/jpg").ETagged();
                                });
        }
    }
}