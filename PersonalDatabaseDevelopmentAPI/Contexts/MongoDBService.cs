using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PersonalDatabaseDevelopmentAPI.Models;

namespace PersonalDatabaseDevelopmentAPI.Contexts
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _categoryCollection;
        private readonly IMongoCollection<BsonDocument> _subCategoryCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            _database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _categoryCollection = _database.GetCollection<BsonDocument>(mongoDBSettings.Value.CategoryCollection);
            _subCategoryCollection = _database.GetCollection<BsonDocument>(mongoDBSettings.Value.SubCategoryCollection);
        }

        public IMongoCollection<BsonDocument> GetCategoryCollection(IOptions<MongoDBSettings> mongoDBSettings)
        {
            return _database.GetCollection<BsonDocument>(mongoDBSettings.Value.CategoryCollection);
        }

        public IMongoCollection<BsonDocument> GetSubCategoryCollection(IOptions<MongoDBSettings> mongoDBSettings)
        {
            return _database.GetCollection<BsonDocument>(mongoDBSettings.Value.SubCategoryCollection);
        }

        public async Task<List<BsonDocument>> GetCategoriesByUserId(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", userId);
            return await _categoryCollection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> GetSubCategoriesByCategoryId(string categoryId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("category_id", categoryId);
            var projection = Builders<BsonDocument>.Projection.Include("_id").Include("name");
            var results = await _subCategoryCollection.Find(filter).Project(projection).ToListAsync();
            if (results != null) return results;
            return new List<BsonDocument>();
        }

        public async Task<BsonDocument> GetSubCategoryById(BsonObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var document = await _subCategoryCollection.Find(filter).FirstOrDefaultAsync();
            return document;
        }

        public async Task<bool> AddSubcategory(string name, string categoryId)
        {
            BsonDocument document = new BsonDocument
            {
                { "category_id", categoryId },
                { "name", name },
            };
            try
            {
                await _subCategoryCollection.InsertOneAsync(document);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddCategory(string name, string userId)
        {
            BsonDocument document = new BsonDocument
            {
                { "user_id", userId },
                { "name", name },
            };
            try
            {
                await _categoryCollection.InsertOneAsync(document);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategory(BsonObjectId id, string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var update = Builders<BsonDocument>.Update.Set("name", name);
            var updateResult = await _categoryCollection.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> UpdateSubCategoryName(BsonObjectId id, string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var update = Builders<BsonDocument>.Update.Set("name", name);
            var updateResult = await _subCategoryCollection.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> UpdateSubCategory(BsonObjectId id, [FromBody] dynamic requestData)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var updateDefinitionBuilder = Builders<BsonDocument>.Update;
            var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();
            foreach (var field in requestData.fields)
            {
                string fieldName = field.Name;
                BsonValue fieldValue = BsonValue.Create((string)field.Value);
                updateDefinitions.Add(updateDefinitionBuilder.Set(fieldName, fieldValue));
            }

            var combinedUpdate = updateDefinitionBuilder.Combine(updateDefinitions);
            var result = await _subCategoryCollection.UpdateOneAsync(filter, combinedUpdate);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> DeleteCategory(BsonObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await _categoryCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteSubCategory(BsonObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await _subCategoryCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<BsonDocument> SearchCategory(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("name", name);
            var document = await _categoryCollection.Find(filter).FirstOrDefaultAsync();
            return document;
        }

        public async Task<bool> DeleteSubCategoryField(BsonObjectId id, [FromBody] dynamic requestData)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            string fieldName = string.Empty;
            foreach (var field in requestData.fields)
            {
                fieldName = field.Name;
            }

            var update = Builders<BsonDocument>.Update.Unset(fieldName);
            var result = await _subCategoryCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UploadFile(string id, string fieldName, Stream fileStream, string fileName)
        {
            var gridFSBucket = new GridFSBucket(_database);
            var fileId = await gridFSBucket.UploadFromStreamAsync(fileName, fileStream);
            var fileIdString = fileId.ToString();
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            var update = Builders<BsonDocument>.Update.Set(fieldName, fileIdString);
            var result = await _subCategoryCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}