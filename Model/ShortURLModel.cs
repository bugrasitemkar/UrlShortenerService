using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Model
{
    public class ShortURLModel :MongoBaseModel
    {
     

        [BsonElement("ShortURL")]
        public string ShortURL { get; set; }

        [BsonElement("LongURL")]
        public string LongURL { get; set; }

        [BsonElement("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
}
