namespace PersonalDatabaseDevelopmentAPI.Models
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; }
        public string DatabaseName { get; set; }
        public string CategoryCollection { get; set; }
        public string SubCategoryCollection { get; set; }
    }
}
