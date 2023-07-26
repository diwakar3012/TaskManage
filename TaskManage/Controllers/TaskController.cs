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
            var filter = Builders<TaskMasterModel>.Filter.And(Builders<TaskMasterModel>.Filter.Eq(rec => rec.Id, TaskId), Builders<TaskMasterModel>.Filter.Eq(rec => rec.IsDelete, 0));
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
            var result = TaskMasterCollection.Aggregate()
                     .Match(filter)
                     .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                     .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity").Project(projection).Project(projection1).As<TaskMasterApiResult>().ToList();
            return result;
        }
        public static Result1 InsertIntoAssignTo(TaskAssign obj)
        {
            Result1 res = new Result1();
            try
            {
                var Db = DAL.Connections();
                var Collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
                var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.TasKId);
                var filters = Builders<TaskMasterModel>.Filter.And(filter1,Builders<TaskMasterModel>.Filter.Eq(p=>p.IsDelete, 0));
                var exist = Collection.Find(filters).ToList();
                res.Status = "Failed";
                if (exist.Count != 0)
                {
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

                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    temp = TaskActivityCollection.Count(new BsonDocument());
                    currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex;
                    obj2.TaskMasterId = obj.TasKId;
                    obj2.Description = "Task Assigned";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
                    res.Status = "Success";
                    res.Message = "Done";
                }
                else
                {
                    res.Status = "Failed";
                    res.Message = "Id not Found";
                }
            }
            catch (Exception e)
            {
                res.Status = "Failed";
                res.Message = "Error";

            }
            return res;
        }
        public static Result1 InsertIntoOrder(TaskOrder obj)
        {
            var Db = DAL.Connections();
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.TaskId);
            var filter2 = Builders<TaskMasterModel>.Filter.Eq(p => p.IsDelete, 0 );
            var filters = Builders<TaskMasterModel>.Filter.And(filter1, filter2);

            var update1 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, obj.OrderIndex); 
            Result1 res = new Result1();
            if (collection.Find(filters).ToList().Count() != 0) {
                var Beforechange = collection.Find(filter1).FirstOrDefault().OrderIndex;
                if (Beforechange != obj.OrderIndex)
                {
                    if(Beforechange > obj.OrderIndex)
                    {
                        var temp = Beforechange - obj.OrderIndex -1; // temp to beforechange-1
                        for(int i =temp; i < Beforechange; i++)
                        {
                            var filter3 = Builders<TaskMasterModel>.Filter.Eq(p => p.OrderIndex, i);
                            var update2 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, i+1);
                            collection.UpdateOne(filter3, update2);
                        }
                    }
                    else
                    {
                        var temp =  obj.OrderIndex - Beforechange; // temp to orderindex
                        for (int i = temp; i <= obj.OrderIndex; i++)
                        {
                            var filter3 = Builders<TaskMasterModel>.Filter.Eq(p => p.OrderIndex, i);
                            var update2 = Builders<TaskMasterModel>.Update.Set(p => p.OrderIndex, i - 1);
                            collection.UpdateOne(filter3, update2);
                        }
                    }
                    collection.UpdateOne(filters, update1);

                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    var temp1 = TaskActivityCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp1 != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex;
                    obj2.TaskMasterId = collection.Find(filter1).FirstOrDefault().Id;
                    obj2.Description = $"Order Index is Changed for Id {obj.TaskId} from {Beforechange} To {obj.OrderIndex}";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
                }
                res.Status = "Success";
            }
            else
            {
                res.Status = "Failed";
                res.Message = "Id Not Found";
            }
            return res;
        }
        public static Root TaskFindWithPage(int? status, string? search, int pageindex, int pagesize)
        {
            try
            {
                var Db = DAL.Connections();
                var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
                collection.Indexes.CreateOne(Builders<TaskMasterModel>.IndexKeys.Text(x => x.Description));
                var searchfilter = Builders<TaskMasterModel>.Filter.Eq(rec=>rec.IsDelete,0); 
                if (search != null)
                {
                    searchfilter = Builders<TaskMasterModel>.Filter.And(Builders<TaskMasterModel>.Filter.Text(search), Builders<TaskMasterModel>.Filter.Eq(rec => rec.IsDelete, 0));
                }
                var Pending_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, -1));
                var InProgress_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, -1));
                var Completed_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, -1));
                var Reopened_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, -1));

                if (status == 0 || status == null)
                {
                    Pending_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 1));
                    InProgress_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 2));
                    Completed_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 3));
                    Reopened_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 4));
                }
                else if (status == 1)
                {
                    Pending_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 1));

                }
                else if (status == 2)
                {
                    InProgress_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 2));

                }
                else if (status == 3)
                {
                    Completed_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 3));

                }
                else if (status == 4)
                {
                    Reopened_filter = Builders<TaskMasterModel>.Filter.And(searchfilter, Builders<TaskMasterModel>.Filter.Eq(x => x.Status, 4));

                }
                var projection1 = new BsonDocument()
                {
                    { "Id",1 },
                    { "Title",1},
                    { "CreatedDate",1},
                    { "CreatedBy","$Assign.CreatedBy"},
                    { "AssignTo","$Assign.AssignTo"},
                    { "TargetDate",1},
                };
                var projection = new BsonDocument() {
                    { "_id",0 },
                };

                var result1 = collection.Aggregate()
                         .Match(Pending_filter)
                         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                         .Skip((pageindex - 1) * pagesize).Limit(pagesize)
                         .Project(projection1).Project(projection).As<Card>().ToList();

                //var temp1 = collection.Aggregate()
                //         .Match(Pending_filter)
                //         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                //         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                //         .Unwind("Assign").Skip((pageindex - 1) * pagesize).Limit(pagesize)
                //         .Project(projection1).Project(projection).As<Card>().ToString();
                //var temp2 = collection.Aggregate()
                //         .Match(InProgress_filter)
                //         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                //         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                //         .Unwind("Assign").Skip((pageindex - 1) * pagesize).Limit(pagesize)
                //         .Project(projection1).Project(projection).As<Card>().ToString();
                //var temp3 = collection.Aggregate()
                //         .Match(Completed_filter)
                //         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                //         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                //         .Unwind("Assign").Skip((pageindex - 1) * pagesize).Limit(pagesize)
                //         .Project(projection1).Project(projection).As<Card>().ToString();
                //var temp4 = collection.Aggregate()
                //         .Match(Reopened_filter)
                //         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                //         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                //         .Unwind("Assign").Skip((pageindex - 1) * pagesize).Limit(pagesize)
                //         .Project(projection1).Project(projection).As<Card>().ToString();

                var result2 = collection.Aggregate()
                         .Match(InProgress_filter)
                         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                         .Skip((pageindex - 1) * pagesize).Limit(pagesize)
                         .Project(projection1).Project(projection).As<Card>().ToList();

                var result3 = collection.Aggregate()
                         .Match(Completed_filter)
                         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                         .Skip((pageindex - 1) * pagesize).Limit(pagesize)
                         .Project(projection1).Project(projection).As<Card>().ToList();

                var result4 = collection.Aggregate()
                         .Match(Reopened_filter)
                         .Lookup("TaskAssign", "Id", "TaskMasterId", "Assign")
                         .Lookup("TaskActivity", "Id", "TaskMasterId", "Activity")
                         .Skip((pageindex - 1) * pagesize).Limit(pagesize)
                         .Project(projection1).Project(projection).As<Card>().ToList();

                Lists lists = new Lists();
                List1 l1 = new List1();
                l1.title = "Pending";
                l1.id = "list-1";
                l1.cards = result1;
                lists.list1 = l1;

                List2 l2 = new List2();
                l2.title = "InProgress";
                l2.id = "list-2";
                l2.cards = result2;
                lists.list2 = l2;

                List3 l3 = new List3();
                l3.title = "Completed";
                l3.id = "list-3";
                l3.cards = result3;
                lists.list3 = l3;

                List4 l4 = new List4();
                l4.title = "Reopened";
                l4.id = "list-4";
                l4.cards = result4;
                lists.list4 = l4;

                Root root = new Root();
                root.lists = lists;

                var isIndexExist = collection.Indexes;
                isIndexExist.DropOne("Description_text");
                return root;

            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Result1 InsertIntoTask(TaskInsert obj)
        {
            var Db = DAL.Connections();
            var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var TaskAssignCollection = Db.GetCollection<TaskAssignModel>("TaskAssign");
            var ProjectMasterCollection = Db.GetCollection<ProjectMasterModel>("ProjectMaster");
            var filter = Builders<ProjectMasterModel>.Filter.And(Builders<ProjectMasterModel>.Filter.Eq(p => p.Id, obj.ProjectId), Builders<ProjectMasterModel>.Filter.Eq(p => p.IsDelete, 0));
            Result1 res = new Result1();

            if (ProjectMasterCollection.Find(filter).ToList().Count() != 0)
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
                    if (currentIndex >= 10)
                        obj1.TaskId = "TEG0" + currentIndex.ToString(); //taskid
                    if (currentIndex >= 100)
                        obj1.TaskId = "TEG" + currentIndex.ToString(); //taskid

                    obj1.Title = obj.TaskName;
                    obj1.Description = obj.Description;
                    obj1.Status = 1;
                    obj1.CreatedDate = DateTime.Now;
                    obj1.UpdatedDate = DateTime.Now;
                    int orderindex = 1;
                    if (temp != 0)
                    {
                        orderindex = TaskMasterCollection.Find(_ => true).SortByDescending(p => p.OrderIndex).Limit(1).FirstOrDefault().OrderIndex;
                        orderindex++;
                    }
                    obj1.OrderIndex = orderindex;

                    if(obj.TargetDate == null)
                        obj1.TargetDate = DateTime.Now; 
                    else
                        obj1.TargetDate = DateTime.Now;
                    
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

                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    temp = TaskActivityCollection.Count(new BsonDocument());
                    currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex; 
                    obj2.TaskMasterId = obj1.Id;
                    obj2.Description = "Task Created";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
                    res.Status = "Success";
                }
                catch (Exception e)
                {
                    res.Status = "Failed";
                    res.Message = "Error";
                }
            }
            else
            {
                res.Status = "Failed";
                res.Message = "Id Not Found";
            }
            return res;
        }
        public static Result1 UpdateStatus(StatusChange obj)
        {
            Result1 res = new Result1();
            res.Status = "Failed";
            var Db = DAL.Connections();
            string[] status = { "Pending", "InProgress", "Completed", "Reopened" };
            var collection = Db.GetCollection<TaskMasterModel>("TaskMaster");
            var filter1 = Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.TaskId);
            var filter2 = Builders<TaskMasterModel>.Filter.Eq(p => p.IsDelete, 0);
            var filters = Builders<TaskMasterModel>.Filter.And(filter1,filter2);
            if ( collection.Find(filters).ToList().Count() != 0 )
            {
                bool FLAG = true;
                int before = collection.Find(filter1).ToList().FirstOrDefault().Status;
                collection.UpdateOne(filters, Builders<TaskMasterModel>.Update.Set(rec => rec.Status, obj.Status));
                FLAG = false;
                if (FLAG)
                {
                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    var temp = TaskActivityCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex; 
                    obj2.TaskMasterId = obj.TaskId;
                    obj2.Description = $"Status Changed from {status[before]} to {status[obj.Status]}";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
                }
                res.Status = "Success";
            }
            else
            {
                res.Status = "Failed";
                res.Message = "No Records Found";
            }
            return res;
        }
        public static Result1 UpdateTaskMaster(TaskUpdateModel obj)
        {
            Result1 res = new Result1();
            try
            {
                var Db = DAL.Connections();
                var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
                var filter = Builders<TaskMasterModel>.Filter.And(Builders<TaskMasterModel>.Filter.Eq(p => p.Id, obj.Id), Builders<TaskMasterModel>.Filter.Eq(p => p.IsDelete, 0));
                if (TaskMasterCollection.Find(filter).ToList().Count() != 0) {
                    var update = Builders<TaskMasterModel>.Update.Set(p => p.Title, obj.Title).Set(p => p.Description, obj.Description).Set(p => p.Status, obj.Status).Set(p => p.UpdatedDate, DateTime.Now).Set(p => p.TargetDate, obj.TargetDate);
                    TaskMasterCollection.UpdateOne(filter, update);
                    res.Status = "Success";

                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    var temp = TaskActivityCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex;
                    obj2.TaskMasterId = obj.Id;
                    obj2.Description = "Task Updated";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
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
        public static Result1 DeleteDocument(int pos)
        {
            Result1 res = new Result1();
            try
            {
                var Db = DAL.Connections();
                var TaskMasterCollection = Db.GetCollection<TaskMasterModel>("TaskMaster");
                var filter = Builders<TaskMasterModel>.Filter.And(Builders<TaskMasterModel>.Filter.Eq(p => p.Id, pos), Builders<TaskMasterModel>.Filter.Eq(p => p.IsDelete, 0));
                var DelRec = TaskMasterCollection.Find(filter).ToList().Count();
                if (DelRec != 0)
                {
                    var org_index = TaskMasterCollection.Find(filter).FirstOrDefault().OrderIndex;
                    var update = Builders<TaskMasterModel>.Update.Set(rec => rec.IsDelete, 1).Set(rec=>rec.OrderIndex,-1);
                    int count = TaskMasterCollection.Find(Builders<TaskMasterModel>.Filter.Gt(rec => rec.OrderIndex, org_index)).ToList().Count();
                    TaskMasterCollection.UpdateOne(filter, update);
                    for(int i = 1; i < count+1; i++)
                    {
                        var filter1 = Builders<TaskMasterModel>.Filter.Eq(rec=>rec.OrderIndex, org_index+i);
                        var update1 = Builders<TaskMasterModel>.Update.Set(rec => rec.OrderIndex, org_index + (i-1));
                        TaskMasterCollection.UpdateOne(filter1, update1);
                    }
                    var TaskActivityCollection = Db.GetCollection<TaskActivityModel>("TaskActivity");
                    TaskActivityModel obj2 = new TaskActivityModel();
                    var temp = TaskActivityCollection.Count(new BsonDocument());
                    int currentIndex = 1;
                    if (temp != 0)
                    {
                        currentIndex = TaskActivityCollection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefault().Id;
                        currentIndex++;
                    }
                    obj2.Id = currentIndex;
                    obj2.TaskMasterId = pos;
                    obj2.Description = "Task Deleted";
                    obj2.CreatedBy = 1; //EmployeeMaster Id
                    obj2.CreatedDate = DateTime.Now;
                    TaskActivityCollection.InsertOne(obj2);
                    res.Status = "Success";
                }
                else
                {
                    res.Status = "Failed";
                    res.Message = "Not Found";
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





