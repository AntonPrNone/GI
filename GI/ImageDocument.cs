using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GI
{
    public class ImageDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("filename")]
        public string Filename { get; set; }

        [BsonElement("image")]
        public byte[] ImageBytes { get; set; }

        [BsonElement("imageBase64")]
        public string ImageBase64 { get; set; }

        public ImageDocument(string filename, byte[] imageBytes)
        {
            Filename = filename;
            ImageBytes = imageBytes;
            ImageBase64 = Convert.ToBase64String(imageBytes);
        }
    }
}
