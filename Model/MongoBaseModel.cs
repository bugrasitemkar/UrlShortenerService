using MongoDB.Bson;

namespace Model
{
    public abstract class MongoBaseModel
    {
        public ObjectId Id { get; set; }
    }
}