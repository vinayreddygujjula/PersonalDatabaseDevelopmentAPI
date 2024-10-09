using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PersonalDatabaseDevelopmentAPI.Contexts;
using Newtonsoft.Json;

namespace PersonalDatabaseDevelopmentAPI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MongoDBService _mongoDbService;

        public CategoryController(MongoDBService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost("addcategory/{name}/{userId}")]
        public async Task<ActionResult> AddCategory(string name, string userId)
        {
            try
            {
                bool result = await _mongoDbService.AddCategory(name, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getcategories/{userId}")]
        public async Task<ActionResult> GetCategoriesByUserId(string userId)
        {
            try
            {
                var results = await _mongoDbService.GetCategoriesByUserId(userId);
                return Ok(results.ToJson());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("updatecategory/{id}/{name}")]
        public async Task<ActionResult> UpdateCategory(string id, string name)
        {
            try
            {
                var objectId = new BsonObjectId(new ObjectId(id));
                bool result = await _mongoDbService.UpdateCategory(objectId, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("deletecategory/{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {
            try
            {
                var objectId = new BsonObjectId(new ObjectId(id));
                bool result = await _mongoDbService.DeleteCategory(objectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        //[HttpGet("searchcategory/{name}")]
        //public async Task<ActionResult> SearchCategory(string name)
        //{
        //    try
        //    {
        //        var results = await _mongoDbService.SearchCategory(name);
        //        return Ok(results);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal Server Error "+ ex);
        //    }
        //}
    }
}
