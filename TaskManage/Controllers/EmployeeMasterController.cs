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
                var filter = Builders<EmployeeMasterModel>.Filter.Eq(rec => rec.Status, false);
                List<EmployeeMasterModel> Documents = EmployeeMasterCollection.Find(filter).ToList();
                return Documents;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string InsertDocument(EmployeeMasterModel obj)
        {
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
                obj.Status = false;
                EmployeeMasterCollection.InsertOne(obj);
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
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var filter = Builders<EmployeeMasterModel>.Filter.Eq(p => p.Id, pos);
                var DelRec = EmployeeMasterCollection.Find(filter).FirstOrDefault();
                if (DelRec != null)
                {
                    var update = Builders<EmployeeMasterModel>.Update.Set(rec=>rec.Status,true);
                    EmployeeMasterCollection.UpdateOne(filter,update);
                }
                return "Deleted";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        public static string UpdateDocument(EmployeeMasterModel obj) 
        {
            try
            {
                var Db = DAL.Connections();
                var EmployeeMasterCollection = Db.GetCollection<EmployeeMasterModel>("EmployeeMaster");
                var filter = Builders<EmployeeMasterModel>.Filter.Eq(p => p.Id, obj.Id);
                var update = Builders<EmployeeMasterModel>.Update.Set(p => p.EmployeeCode, obj.EmployeeCode).Set(p => p.EmployeeName, obj.EmployeeName).Set(p => p.Email, obj.Email).Set(p => p.Role, obj.Role);//.Set(p => p.Status,obj.Status);
                EmployeeMasterCollection.UpdateOne(filter, update);
                return "Updated";
            }
            catch(Exception e) 
            { 
                return e.Message.ToString();
            }
        }
    }
}
