﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI
{
    public class ImageUploader
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _directoryPath;

        public ImageUploader(string databaseName = "DB", string collectionName = "CharactersGI", string directoryPath = "-", string connectionString = "mongodb://localhost:27017")
        {
            if (directoryPath == "-") directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"imgs\pers");
            Directory.CreateDirectory(directoryPath);
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _directoryPath = directoryPath;
        }

        public void UploadImage(string filename) // Загрузка в БД конкретное изображение
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<BsonDocument>(_collectionName);

            filename = Path.ChangeExtension(filename, ".png");
            var file = new FileInfo(Path.Combine(_directoryPath, filename));

            if (file.Exists)
            {
                var imageBytes = File.ReadAllBytes(file.FullName);
                var imageBase64 = Convert.ToBase64String(imageBytes);

                var document = new BsonDocument
                {
                    {"filename", file.Name},
                    {"image", new BsonBinaryData(imageBytes)},
                    {"imageBase64", imageBase64},
                    {"uploadDate", DateTime.UtcNow.AddHours(3)}
                };

                collection.InsertOne(document);
            }
            else
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }

            client = null;
        }

        public void UploadImage() // Загрузка в БД все изображения из папки
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<BsonDocument>(_collectionName);

            var directory = new DirectoryInfo(_directoryPath);

            foreach (var file in directory.GetFiles())
            {
                var imageBytes = File.ReadAllBytes(file.FullName);
                var imageBase64 = Convert.ToBase64String(imageBytes);

                var document = new BsonDocument
                {
                    {"filename", file.Name},
                    {"image", new BsonBinaryData(imageBytes)},
                    {"imageBase64", imageBase64},
                    {"uploadDate", DateTime.UtcNow.AddHours(3)}
                };

                collection.InsertOne(document);
            }

            client = null;
        }

        public void LoadImageFromDb(string filename) // Загрузка из БД конкретный файл
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<BsonDocument>(_collectionName);

            filename = Path.ChangeExtension(filename, ".png");

            var filter = Builders<BsonDocument>.Filter.Eq("filename", filename);
            var document = collection.Find(filter).FirstOrDefault();

            if (document != null)
            {
                var bytes = document["image"].AsByteArray;
                File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes);
            }

            client = null;
        }

        public void LoadImageFromDb()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<BsonDocument>(_collectionName);

            var filter = Builders<BsonDocument>.Filter.Empty;
            var documents = collection.Find(filter).ToList();

            foreach (var document in documents)
            {
                var filename = document["filename"].ToString();
                var bytes = document["image"].AsByteArray;
                File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes);
            }

            client = null;
        }


    }
}
