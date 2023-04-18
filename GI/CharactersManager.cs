using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI
{
    internal class CharactersManager
    {
        private readonly IMongoCollection<Character> _characters;

        public CharactersManager(string databaseName = "DB", string collectionName = "CharactersGI", string connectionString = "mongodb://localhost:27017")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _characters = database.GetCollection<Character>(collectionName);
        }

        public async Task UploadCharacterAsync(Character character)
        {
            await _characters.InsertOneAsync(character);
        }

        public async Task<Character> LoadCharacterAsync(string name)
        {
            var filter = Builders<Character>.Filter.Eq(c => c.Name, name);
            var character = await _characters.Find(filter).FirstOrDefaultAsync();
            return character;
        }

        public async Task<List<Character>> LoadCharacterAsync()
        {
            var characters = await _characters.Find(_ => true).ToListAsync();
            return characters;
        }

        public async Task DeleteCharacterAsync(string name)
        {
            var filter = Builders<Character>.Filter.Eq(c => c.Name, name);
            var result = await _characters.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new ArgumentException($"Character {name} not found.");
            }
        }
    }
}
