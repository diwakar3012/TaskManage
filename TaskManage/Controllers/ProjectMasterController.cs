using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TaskMange.Models;

namespace TaskMange.Controllers
{
    public class ProjectMasterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public static List<ProjectMasterModel> GetRecords()
        {
            try
            {
                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var filter = Builders<ProjectMasterModel>.Filter.Eq(rec => rec.IsDelete, false);
                List<ProjectMasterModel> Documents = ProjectMasterCollection.Find(filter).ToList();
                return Documents;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string InsertDocument(ProjectMasterModel obj)
        {
            try
            {
                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var temp = ProjectMasterCollection.Count(new BsonDocument());
                int currentIndex = 1;
                if (temp != 0)
                {
                    currentIndex = ProjectMasterCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                    currentIndex++;
                }
                obj.Id = currentIndex;
                obj.IsDelete = false;
                ProjectMasterCollection.InsertOne(obj);
                return "Inserted";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        public static string DeleteDocument(int pos)
        {
            try
            {
                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var filter = Builders<ProjectMasterModel>.Filter.Eq(p => p.Id, pos);
                var DelRec = ProjectMasterCollection.Find(filter).FirstOrDefault();
                if (DelRec != null)
                {
                    var update = Builders<ProjectMasterModel>.Update.Set(rec => rec.IsDelete, true);
                    ProjectMasterCollection.UpdateOne(filter, update);
                }
                return "Deleted";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        public static string UpdateDocument(ProjectMasterModel obj)
        {
            try
            {
                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var filter = Builders<ProjectMasterModel>.Filter.Eq(p => p.Id, obj.Id);
                var update = Builders<ProjectMasterModel>.Update.Set(p => p.ProjectCode, obj.ProjectCode).Set(p => p.ProjectName, obj.ProjectName).Set(p => p.UpdatedDate, DateTime.Now);
                ProjectMasterCollection.UpdateOne(filter, update);
                return "Updated";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
    }
}
