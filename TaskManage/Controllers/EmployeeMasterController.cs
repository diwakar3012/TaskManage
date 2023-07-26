using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TaskMange.Models;

namespace TaskMange.Controllers
{
    public class EmployeeMasterController : Controller
    {
        //mongodb://127.0.0.1:27017
        
        public IActionResult Index()
        {
            return View();
        }
        public static List<EmployeeMasterModel> GetRecords()
        {
            try
            {
                var Db = DAL.Connections();
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var filter = Builders<EmployeeMasterModel>.Filter.Eq(rec => rec.IsDelete, 0);
                List<EmployeeMasterModel> Documents = EmployeeMasterCollection.Find(filter).ToList();
                return Documents;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Result1 InsertDocument(EmployeeMasterModel obj)
        {
            Result1 res = new Result1();
            res.Status = "Failed";
            try
            {
                var Db = DAL.Connections();
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var temp = EmployeeMasterCollection.Count(new BsonDocument());
                int currentIndex = 1;
                if (temp != 0)
                {
                    currentIndex = EmployeeMasterCollection.Find(_ => true).SortByDescending(p=>p.Id).Limit(1).FirstOrDefault().Id;
                    currentIndex++;
                }
                obj.Id = currentIndex;
                if (currentIndex <= 9)
                {
                    obj.EmployeeCode = "EG00" + currentIndex.ToString();
                }
                if (currentIndex >= 10) { 
                    obj.EmployeeCode = "EG0" + currentIndex.ToString();
                }
                if (currentIndex >= 100)
                {
                    obj.EmployeeCode = "EG" + currentIndex.ToString();
                }
                var flag = EmployeeMasterCollection.Find(Builders<EmployeeMasterModel>.Filter.Eq("Email", obj.Email)).ToList().Count();
                if (flag == 0)
                {
                    EmployeeMasterCollection.InsertOne(obj);
                    res.Status = "Success";
                }
                else
                {
                    res.Status = "Failed";
                    res.Message = "Email Already Exists!";
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
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var filter = Builders<EmployeeMasterModel>.Filter.Eq(p => p.Id, pos);
                var DelRec = EmployeeMasterCollection.Find(filter).FirstOrDefault();
                if (DelRec != null)
                {
                    var update = Builders<EmployeeMasterModel>.Update.Set(rec=>rec.IsDelete,1);
                    EmployeeMasterCollection.UpdateOne(filter,update);
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
            }
            return res;
        }
        public static Result1 UpdateDocument(EmployeeMasterModel obj) 
        {
            Result1 res = new Result1();
            try
            {
                var Db = DAL.Connections();
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var filter = Builders<EmployeeMasterModel>.Filter.And(Builders<EmployeeMasterModel>.Filter.Eq(p => p.Id, obj.Id),Builders<EmployeeMasterModel>.Filter.Eq(p => p.IsDelete, 0));
                if (EmployeeMasterCollection.Find(filter).ToList().Count() != 0) {
                    var update = Builders<EmployeeMasterModel>.Update.Set(p => p.EmployeeName, obj.EmployeeName).Set(p => p.Email, obj.Email).Set(p => p.Role, obj.Role).Set(p => p.Status, obj.Status);
                    EmployeeMasterCollection.UpdateOne(filter, update);
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
                res.Status = "Error";
            }
            return res;
        }
    }
}
