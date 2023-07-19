using DnsClient;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMange.Models
{
    public class EmployeeMasterModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public bool Status { get; set; }
    }
    public class ProjectMasterModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDelete { get; set; }
    }
    public class TaskMasterModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public int ProjectMasterId { get; set; } //project Id
        public string? TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public DateTime CreatedDate = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
        public int OrderIndex { get; set; }
        public DateTime TargetDate { get; set; }
        public bool IsDelete { get; set; }
    }
    public class TaskAssignModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public int TaskMasterId { get; set; }  //Taskmaster Id
        public int CreatedBy { get; set; } //EmployeeMaster Id
        public int AssignTo { get; set; } //EmployeeMaster Id
        public DateTime AssignDate { get; set; }
    }
    public class TaskActivityModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public int TaskMasterId { get; set; }  //Taskmaster Id
        public string? Description { get; set; }
        public int CreatedBy { get; set; } //EmployeeMaster Id

        public DateTime CreatedDate = DateTime.Now;
    }

    public class Activity
    {
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class TaskMasterApiResult
    {
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Status { get; set; }
        public int AssignTo { get; set; }
        public DateTime TargetDate { get; set; }
        public string? Description { get; set; }
        public List<Activity> Activity { get; set; }
    }
    public class Result
    {
        public int Status { get; set; }
    }
    public class TaskOrder
    {
        public string taskid { get; set; }
        public int orderindex { get; set; }
    }
    public class TaskAssign
    {
        public string taskid { get; set; }
        public int assignid { get; set; }
    }
    public class Task_Id
    {
        public string taskname { get; set; }
        public string description { get; set; }
        public int assignTo { get; set; }
    }
}

