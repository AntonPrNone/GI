using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GI
{
    // Коллекция изображений
    public class ImageUploader
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _directoryPath;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _collection;

        public ImageUploader(string databaseName = "DB", string collectionName = "CharactersGI", string directoryPath = "null", string connectionString = "mongodb://localhost:27017")
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;

            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "imgs", "pers");
            }

            Directory.CreateDirectory(directoryPath);
            _directoryPath = directoryPath;

            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(_databaseName);
            _collection = _database.GetCollection<BsonDocument>(_collectionName);
        }

        public async Task UploadImageAsync(string filename) // Загрузка в БД конкретное изображение
        {
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

                await _collection.InsertOneAsync(document);
            }

            else
            {
                //throw new FileNotFoundException($"File {filename} not found.");
            }
        }

        public async Task UploadImageAsync() // Загрузка в БД все изображения из папки
        {
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

                await _collection.InsertOneAsync(document);
            }
        }

        public async Task LoadImageFromDbAsync(string filename) // Загрузка из БД конкретный документ
        {
            filename = Path.ChangeExtension(filename, ".png");

            var filter = Builders<BsonDocument>.Filter.Eq("filename", filename);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();

            if (document != null)
            {
                var bytes = document["image"].AsByteArray;
                await Task.Run(() => File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes));
            }
        }

        public async Task LoadImageFromDbAsync() // Загрузка из БД всех документов коллекции
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var documents = await _collection.Find(filter).ToListAsync();

            foreach (var document in documents)
            {
                var filename = document["filename"].ToString();
                var bytes = document["image"].AsByteArray;
                await Task.Run(() => File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes));
            }
        }

        public async Task DeleteImageAsync(string filename) // Удаление из БД документа коллекции
        {
            var filter = Builders<BsonDocument>.Filter.Eq("filename", filename);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }
        }
    }
}
