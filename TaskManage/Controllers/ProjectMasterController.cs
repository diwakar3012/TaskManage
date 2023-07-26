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
                var filter = Builders<ProjectMasterModel>.Filter.Eq(rec => rec.IsDelete, 0);
                List<ProjectMasterModel> Documents = ProjectMasterCollection.Find(filter).ToList();
                return Documents;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Result1 InsertDocument(ProjectMasterModel obj)
        {
            Result1 res = new Result1();
            try
            {
                res.Status = "Failed";
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
                if (currentIndex <= 9)
                {
                    obj.ProjectCode = "PEG00" + currentIndex.ToString();
                }
                else if (currentIndex >= 10)
                {
                    obj.ProjectCode = "PEG0" + currentIndex.ToString();
                }
                else if (currentIndex >= 100)
                {
                    obj.ProjectCode = "PEG" + currentIndex.ToString();
                }
                ProjectMasterCollection.InsertOne(obj);
                res.Status = "Success";
            }
            catch (Exception e)
            {
                res.Status = "Failed";
                res.Message = "Error";
            }
            return res;
        }
        public static Result1 DeleteDocument(int pos)
        {
            Result1 res = new Result1();
            try
            {

                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var filter = Builders<ProjectMasterModel>.Filter.And(Builders<ProjectMasterModel>.Filter.Eq(p => p.Id, pos), Builders<ProjectMasterModel>.Filter.Eq(p => p.IsDelete, 0));
                var DelRec = ProjectMasterCollection.Find(filter).FirstOrDefault();
                if (DelRec != null)
                {
                    var update = Builders<ProjectMasterModel>.Update.Set(rec => rec.IsDelete, 1);
                    ProjectMasterCollection.UpdateOne(filter, update);
                    res.Status = "Success";
                }
                else
                {
                    res.Status = "Failed";
                    res.Message = "No Records Found";
                }
            }
            catch (Exception e)
            {
                res.Status = "Failed";
                res.Message = "Error";
            }
            return res;
        }
        public static Result1 UpdateDocument(ProjectMasterModel obj)
        {
            Result1 res = new Result1();
            try
            {
                var Db = DAL.Connections();
                var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
                var filter = Builders<ProjectMasterModel>.Filter.And(Builders<ProjectMasterModel>.Filter.Eq(p => p.Id, obj.Id), Builders<ProjectMasterModel>.Filter.Eq(p => p.IsDelete, 0));
                if (ProjectMasterCollection.Find(filter).ToList().Count() != 0)
                {
                    var update = Builders<ProjectMasterModel>.Update.Set(p => p.ProjectName, obj.ProjectName).Set(p => p.UpdatedDate, DateTime.Now);
                    ProjectMasterCollection.UpdateOne(filter, update);
                    res.Status = "Success";
                }
                else
                {
                    res.Status = "Failed";
                    res.Message = "No Records Found";
                }
            }
            catch (Exception e)
            {
                res.Status = "Failed";
                res.Message = "Error";
            }
            return res;
        }
    }
}
