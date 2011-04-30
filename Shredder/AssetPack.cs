using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace Shredder
{
    public class AssetPack
    {
        public ObjectId Id { get; set; }
        public GettyImages.Editorial.App.Image.MetaData ImageMetadata { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime IngestedDate { get; set; }
    }
}
