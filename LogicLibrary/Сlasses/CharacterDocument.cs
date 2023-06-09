﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogicLibrary
{
    [BsonIgnoreExtraElements]
    public class CharacterDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("photo")]
        public string Photo { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

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
        public DateTime UploadDate { get; set; }

        [BsonIgnore]
        public string UploadDateString => UploadDate.ToString("dd.MM.yy HH:mm");

        [BsonElement("favoriteСharacters")]
        public string FavoriteСharacters { get; set; }
    }

    public class Stats
    {
        [BsonElement("attack")]
        public int Attack { get; set; }

        [BsonElement("health")]
        public int Health { get; set; }

        [BsonElement("defense")]
        public int Defense { get; set; }
    }
}
