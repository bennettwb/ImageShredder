using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Builders;
using WPD.Shared;

namespace WPD.Web.Endpoints
{
    public class MongoImage : Nina.Application
    {
        public MongoImage()
        {
            //BsonClassMap.RegisterClassMap<AssetPack>(cm =>
            //                                             {
            //                                                 cm.SetIgnoreExtraElements(true);
            //                                             });

            Get("", (m, c) =>
                        {
                            var db = Mongo.GetDatabase();
                            AssetPack [] coll;

                            var date = c.Request.QueryString["d"];


                            if (date == null || date == "undefined")
                            {
                                coll =
                                    db.GetCollection<AssetPack>("images").Find(Query.LT("IngestDate", BsonValue.Create(DateTime.Now.Subtract(TimeSpan.FromSeconds(3))))).Take(100).OrderByDescending(t => t.IngestDate)
                                        .ToArray();
                            }
                            else
                            {
                                coll =
                                    db.GetCollection<AssetPack>("images").Find(Query.GT("IngestDate",
                                                                                        BsonValue.Create(DateTime.Parse(date))).LT(BsonValue.Create(DateTime.Now.Subtract(TimeSpan.FromSeconds(3))))).
                                        OrderByDescending(t => t.IngestDate).Take(100).ToArray();

                            }
                            var images = coll.Select(x => new { Id = x.Id.ToString(), AssetPack = x });

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