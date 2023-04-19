﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI
{
    internal class CharactersManager
    {
        private readonly IMongoCollection<CharacterDocument> _characters;

        public CharactersManager(string databaseName = "DB", string collectionName = "CharactersGI", string connectionString = "mongodb://localhost:27017")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _characters = database.GetCollection<CharacterDocument>(collectionName);
        }

        public async Task<bool> UploadCharacterAsync(CharacterDocument character) // Добавление персонажа в БД
        {
            var filter = Builders<CharacterDocument>.Filter.Eq(c => c.Name, character.Name);
            var existingCharacter = await _characters.Find(filter).FirstOrDefaultAsync();

            if (existingCharacter == null)
            {
                await _characters.InsertOneAsync(character);
            }

            return existingCharacter == null;
        }

        public async Task<CharacterDocument> LoadCharacterAsync(string name) // Получение персонажа из БД
        {
            var filter = Builders<CharacterDocument>.Filter.Eq(c => c.Name, name);
            var character = await _characters.Find(filter).FirstOrDefaultAsync();
            return character;
        }

        public async Task<List<CharacterDocument>> LoadCharacterAsync() // Получение всех персонажей из БД
        {
            var characters = await _characters.Find(_ => true).ToListAsync();
            return characters;
        }

        public async Task<bool?> DeleteCharacterAsync(string name) // Удаление персонажа из БД
        {
            var filter = Builders<CharacterDocument>.Filter.Eq(c => c.Name, name);
            var result = await _characters.DeleteOneAsync(filter);

            return result.DeletedCount != 0;
        }
    }
}
