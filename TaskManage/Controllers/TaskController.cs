using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TaskMange.Models;

namespace TaskMange.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public static List<TaskMasterApiResult> getTask(int TaskId)
        {
            var Db = DAL.Connections();
            var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, TaskId);
            var projection = new BsonDocument() {
                { "Id",1 },
                { "Title",1},
                { "CreatedDate",1},
                { "CreatedBy","$Assign.CreatedBy"},
                { "Status",1},
                { "AssignTo","$Assign.AssignTo"},
                { "TargetDate",1},
                { "Description",1},
                { "Activity",1},
            };
            var projection1 = new BsonDocument()
            {
                { "_id",0 },
                { "Id",0 },
                { "Activity._id",0 },
                { "Activity.Id",0 },
                { "Activity.TaskMasterId",0 }
            };

            var result = TaskMasterCollection.Aggregate()
                     .Match(filter)
                     .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                     .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Unwind("Assign").Project(projection).Project(projection1).As<TaskMasterApiResult>().ToList();

            return result;
        }
        public static List<Result> AssignToStatus(TaskAssign obj)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter = Builders<TaskMasterModel>.Filter.Eq(p => p.TaskId, obj.taskid);
            var projection = new BsonDocument() {
                {"Status",1 }
            };
            var projection1 = new BsonDocument()
            {
                { "_id",0 },
                { "Id",0 }
            };
            var res = collection.Aggregate().Match(filter).Project(projection).Project(projection1).As<Result>().ToList();
            return res;
        }
        public static List<Result> OrderStatus(TaskOrder obj)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.TaskId, obj.taskid); 
            var filter2 = Builders<TaskMasterModel>.Filter.Eq(p => p.OrderIndex, obj.orderindex); 
            var filters = Builders<TaskMasterModel>.Filter.And(filter1, filter2);   
            var projection = new BsonDocument() {
                {"Status",1 }
            };
            var projection1 = new BsonDocument()
            {
                { "_id",0 },
                { "Id",0 }
            };
            var res = collection.Aggregate().Match(filters).Project(projection).Project(projection1).As<Result>().ToList();
            return res;
        }
        public  static List<TaskMasterModel> TaskFindWithPage(int status, string search, int pageindex, int pagesize)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            collection.Indexes.CreateOne(Builders<TaskMasterModel>.IndexKeys.Text(x => x.Description));
            var filter1 = Builders<TaskMasterModel>.Filter.Text(search);
            var filter2 = Builders<TaskMasterModel>.Filter.Eq(x=>x.Status,status);
            var filter = Builders<TaskMasterModel>.Filter.And(filter1,filter2);
            var temp = collection.Find(filter).Skip((pageindex-1) * pagesize).Limit(pagesize).As<TaskMasterModel>().ToList(); 
            return temp;
        }
    }
}




