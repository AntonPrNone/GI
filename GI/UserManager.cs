using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI
{
    internal class UserManager
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserManager(string databaseName = "DB", string collectionName = "User", string connectionString = "mongodb://localhost:27017")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _usersCollection = database.GetCollection<User>(collectionName);
        }

        public async Task ReplaceUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Login, user.Login);
            await _usersCollection.ReplaceOneAsync(filter, user);
        }
    }
}
