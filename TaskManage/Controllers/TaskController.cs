using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using TaskMange.Models;
using Result = TaskMange.Models.Result;

namespace TaskMange.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public static List<TaskMasterApiResult> getTask()//int TaskId
        {
            var Db = DAL.Connections();
            var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter = Builders<TaskMasterModel>.Filter.Empty;
            //var filter = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, TaskId);
            var projection = new BsonDocument() {
                { "_id",0 },
                { "Activity._id",0 },
                { "Activity.Id",0 },
                { "Activity.TaskMasterId",0 }
            };
            var projection1 = new BsonDocument()
            {
                { "Id",1 },
                { "Title",1},
                { "CreatedDate",1},
                { "CreatedBy","$Assign.CreatedBy"},
                { "Status",1},
                { "AssignTo","$Assign.AssignTo"},
                { "TargetDate",1},
                { "Description",1},
                { "Activity",1}
            };
            //{ "CreatedBy", new BsonDocument(){ {"$dateToString",new BsonDocument() { { "format", "%Y-%m-%d" }, { "date", "$CreatedDate" } } } } },
            //var temp = TaskMasterCollection.Aggregate()
            //         .Match(filter)
            //         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
            //         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Unwind("Assign").Project(projection).Project(projection1).As<BsonDocument>().ToString();
            var result = TaskMasterCollection.Aggregate()
                     .Match(filter)
                     .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                     .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Unwind("Assign").Project(projection).Project(projection1).As<TaskMasterApiResult>().ToList();

            return result;
        }
        public static Result InsertIntoAssignTo(TaskAssign obj)
        {
            try
            {
                var Db = DAL.Connections();
                var Collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
                var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.TasKId);
                var exist = Collection.Find(filter1).ToList();
                Result res = new Result();
                res.Status = "Failure";
                if (exist.Count != 0)
                {
                    res.Status = "Success";
                    var AssignToCollection = Db.GetCollection<TaskAssignModel>("TaskAssign");
                    var temp = AssignToCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = AssignToCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    TaskAssignModel ToInsert = new TaskAssignModel();
                    ToInsert.Id = currentIndex; 
                    ToInsert.TaskMasterId = obj.TasKId;
                    ToInsert.CreatedBy = 1; // EmployeeMaster Id
                    ToInsert.AssignTo = obj.AssignTo;
                    ToInsert.AssignDate = DateTime.Now;
                    AssignToCollection.InsertOne(ToInsert);
                }
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Result InsertIntoOrder(TaskOrder obj)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.TaskId);
            //var filters = Builders<TaskMasterModel>.Filter.And(filter1, filter2);

            var update1 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, obj.OrderIndex);
            Result res = new Result();
            res.Status = "Failure";
            if (collection.Find(filter1).ToList().Count() !=0) {
                var Beforechange = collection.Find(filter1).FirstOrDefault().OrderIndex;
                if (Beforechange != obj.OrderIndex)
                {
                    if(Beforechange > obj.OrderIndex)
                    {
                        var temp = Beforechange - obj.OrderIndex -1; // temp to beforechange-1
                        for(int i =temp; i < Beforechange; i++)
                        {
                            var filter2 = Builders<TaskMasterModel>.Filter.Eq(p => p.OrderIndex, i);
                            var update2 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, i+1);
                            collection.UpdateOne(filter2, update2);
                        }
                    }
                    else
                    {
                        var temp =  obj.OrderIndex - Beforechange; // temp to orderindex
                        for (int i = temp; i <= obj.OrderIndex; i++)
                        {
                            var filter2 = Builders<TaskMasterModel>.Filter.Eq(p => p.OrderIndex, i);
                            var update2 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, i - 1);
                            collection.UpdateOne(filter2, update2);
                        }
                    }
                    collection.UpdateOne(filter1, update1);
                }
                res.Status = "Success";
            }
            return res;
    }
        public  static Root TaskFindWithPage(int status, string search, int pageindex, int pagesize)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            collection.Indexes.CreateOne(Builders<TaskMasterModel>.IndexKeys.Text(x => x.Description));
            var filter1 = Builders<TaskMasterModel>.Filter.Text(search);
            var filter2 = Builders<TaskMasterModel>.Filter.Eq(x=>x.Status,status);
            var filter = Builders<TaskMasterModel>.Filter.And(filter1,filter2);
            var projection = new BsonDocument() {
                { "_id",0 }
            };
            var projection1 = new BsonDocument()
            {
                { "Id",1 },
                { "Title",1},
                { "CreatedDate",1},
                { "CreatedBy","$Assign.CreatedBy"},
                { "AssignTo","$Assign.AssignTo"},
                { "TargetDate",1},
            };
            var result = collection.Aggregate()
                     .Match(filter)
                     .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                     .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Unwind("Assign").Skip((pageindex-1) * pagesize).Limit(pagesize).Project(projection).Project(projection1).As<Card>().ToList();
            var temp = collection.Aggregate()
                     .Match(filter)
                     .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                     .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Unwind("Assign").Skip((pageindex - 1) * pagesize).Limit(pagesize).Project(projection).Project(projection1).As<BsonDocument>().ToString();
            Lists lists = new Lists();
            List1 l1 = new List1 ();
            l1.title = "Pending";
            l1.id = "list-1";
            l1.cards = result;
            lists.list1 = l1;

            List2 l2 = new List2 ();
            l2.title = "InProgress";
            l2.id = "list-2";
            l2.cards = result;
            lists.list2 = l2;

            List3 l3 = new List3 ();
            l3.title = "Completed";
            l3.id = "list-3";
            l3.cards = result;
            lists.list3 = l3;

            List4 l4 = new List4 ();
            l4.title = "Reopened";
            l4.id = "list-4";
            l4.cards = result;
            lists.list4 = l4;

            Root root = new Root();
            root.lists = lists;

            var isIndexExist = collection.Indexes;
            isIndexExist.DropOne("Description_text");
            return root;
        }
        public static Result InsertIntoTask(TaskInsert obj)
        {
            var Db = DAL.Connections();
            var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var TaskAssignCollection = Db.GetCollection<TaskAssignModel>("TaskAssign");
            var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
            Result res = new Result();
            res.Status = "Failure";
            if (ProjectMasterCollection.Find(p => p.Id == obj.ProjectId).ToList().Count() != 0)
            {
                try
                {
                    var temp = TaskMasterCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskMasterCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    TaskMasterModel obj1 = new TaskMasterModel();
                    obj1.Id = currentIndex;
                    obj1.ProjectMasterId = obj.ProjectId;
                    if (currentIndex < 10)
                        obj1.TaskId = "TEG00" + currentIndex.ToString(); //taskid
                    if (currentIndex > 10)
                        obj1.TaskId = "TEG0" + currentIndex.ToString(); //taskid
                    if (currentIndex > 100)
                        obj1.TaskId = "TEG" + currentIndex.ToString(); //taskid
                    obj1.Title = obj.TaskName;
                    obj1.Description = obj.Description;
                    obj1.Status = 0;
                    obj1.CreatedDate = DateTime.Now;
                    obj1.UpdatedDate = DateTime.Now;
                    int orderindex = 1;
                    if (temp != 0)
                    {
                        orderindex = TaskMasterCollection.Find(_ => true).SortByDescending(p => p.OrderIndex).Limit(1).FirstOrDefault().OrderIndex;
                        orderindex++;
                    }
                    obj1.OrderIndex = orderindex;
                    obj1.TargetDate = DateTime.Now; //target time
                    TaskMasterCollection.InsertOne(obj1);

                    temp = TaskAssignCollection.Count(new BsonDocument());
                    currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskAssignCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    TaskAssignModel ToInsert = new TaskAssignModel();
                    ToInsert.Id = currentIndex;
                    ToInsert.TaskMasterId = obj1.Id;
                    ToInsert.CreatedBy = 1; // EmployeeMaster Id
                    ToInsert.AssignTo = obj.AssignTo;
                    ToInsert.AssignDate = DateTime.Now;
                    TaskAssignCollection.InsertOne(ToInsert);
                    res.Status = "Success";
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return res;
        }
    }
}




