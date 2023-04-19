using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GI
{
    [BsonIgnoreExtraElements]
    public class CharacterDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("rarity")]
        public int Rarity { get; set; }

        [BsonElement("element")]
        public string Element { get; set; }

        [BsonElement("weapon")]
        public string Weapon { get; set; }

        [BsonElement("region")]
        public string Region { get; set; }

        [BsonElement("stats")]
        public Stats Stats { get; set; }

        [BsonElement("uploadDate")]
        public string UploadDate { get; set; }
    }

    public class Stats
    {
        [BsonElement("health")]
        public int Health { get; set; }

        [BsonElement("attack")]
        public int Attack { get; set; }

        [BsonElement("defense")]
        public int Defense { get; set; }
    }
}
