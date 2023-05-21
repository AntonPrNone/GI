using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LogicLibrary
{
    // Коллекция изображений
    public class ImageManager
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _directoryPath;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ImageDocument> _collection;

        public ImageManager(string databaseName = "DB", string collectionName = "ImageGI", string directoryPath = null, string connectionString = "mongodb://localhost:27017")
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;

            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GI", "imgs", "pers2");
            }

            Directory.CreateDirectory(directoryPath);
            _directoryPath = directoryPath;

            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(_databaseName);
            _collection = _database.GetCollection<ImageDocument>(_collectionName);
        }

        public async Task<bool> UploadImageAsync(string filename) // Загрузка в БД конкретное изображение
        {
            filename = Path.ChangeExtension(filename, ".png");
            var file = new FileInfo(Path.Combine(_directoryPath, filename));

            if (file.Exists)
            {
                var imageBytes = await Task.Run(() => File.ReadAllBytes(file.FullName));
                var imageDocument = new ImageDocument(file.Name, imageBytes);
                await _collection.InsertOneAsync(imageDocument);
            }

            return file.Exists;
        }

        public async Task UploadImageAsync() // Загрузка в БД все изображения из папки (Не для пользовательского пользования, поэтому не нужна обработка исключения)
        {
            var directory = new DirectoryInfo(_directoryPath);

            foreach (var file in directory.GetFiles())
            {
                var imageBytes = await Task.Run(() => File.ReadAllBytes(file.FullName));

                var document = new ImageDocument(file.Name, imageBytes);

                await _collection.InsertOneAsync(document);
            }
        }

        public async Task<bool> LoadImageFromDbAsync(string filename) // Загрузка из БД конкретное изображение
        {
            filename = Path.ChangeExtension(filename, ".png");

            var filter = Builders<ImageDocument>.Filter.Eq("filename", filename);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();

            if (document != null)
            {
                var bytes = document.ImageBytes;
                await Task.Run(() => File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes));
            }

            return document != null;
        }

        public async Task LoadImageFromDbAsync() // Загрузка из БД всех изображений коллекции
        {
            var filter = Builders<ImageDocument>.Filter.Empty;
            var documents = await _collection.Find(filter).ToListAsync();

            foreach (var document in documents)
            {
                var filename = document.Filename;
                var path = Path.Combine(_directoryPath, filename);
                var pathEx = Path.Combine(_directoryPath, Path.ChangeExtension(filename, ".png"));

                // Проверяем, существует ли файл в папке
                if (!File.Exists(pathEx))
                {
                    var bytes = document.ImageBytes;
                    await Task.Run(() => File.WriteAllBytes(path, bytes));
                }
            }
        }

        public async Task<bool> DeleteImageAsync(string filename) // Удаление из БД изображения коллекции
        {
            var filter = Builders<ImageDocument>.Filter.Eq("filename", filename);
            var result = await _collection.DeleteOneAsync(filter);

            return result.DeletedCount != 0;
        }
    }
}
