using MongoDB.Driver;

namespace TaskMange.Models
{
    public class DAL
    {
        public static IMongoDatabase Connections()
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var Db = client.GetDatabase("TaskManage");
            return Db;
        }
    }
}
