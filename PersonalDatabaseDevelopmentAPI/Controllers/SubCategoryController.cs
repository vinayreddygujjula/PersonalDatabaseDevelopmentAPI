using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PersonalDatabaseDevelopmentAPI.Contexts;

namespace PersonalDatabaseDevelopmentAPI.Controllers
{
    public class SubCategoryController : Controller
    {

        private readonly MongoDBService _mongoDbService;

        public SubCategoryController(MongoDBService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost("addsubcategory/{name}/{categoryId}")]
        public async Task<ActionResult> AddSubCategory(string name, string categoryId)
        {
            try
            {
                bool result = await _mongoDbService.AddSubcategory(name, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getsubcategories/{categoryId}")]
        public async Task<ActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            try
            {
                var results = await _mongoDbService.GetSubCategoriesByCategoryId(categoryId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getsubcategory/{id}")]
        public async Task<ActionResult> GetSubCategoryById(string id)
        {
            try
            {
                var objectId = new BsonObjectId(new ObjectId(id));
                var document = await _mongoDbService.GetSubCategoryById(objectId);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("updatesubcategory/{id}")]
        public async Task<ActionResult> UpdateSubCategory(string id, [FromBody] dynamic requestData)
        {
            try
            {
                var objectId = new BsonObjectId(new ObjectId(id));
                bool result = await _mongoDbService.UpdateSubCategory(objectId, requestData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        [HttpDelete("deletesubcategory/{id}")]
        public async Task<ActionResult> DeleteSubCategory(string id)
        {
            try
            {
                var objectId = new BsonObjectId(new ObjectId(id));
                bool result = await _mongoDbService.DeleteSubCategory(objectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}