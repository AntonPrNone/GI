using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace LogicLibrary
{
    public class User
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<string> FavoriteСharacters { get; set; }
    }
}
