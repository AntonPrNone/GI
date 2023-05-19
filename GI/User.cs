using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GI
{
    class User
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
