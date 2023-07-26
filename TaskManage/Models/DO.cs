using MongoDB.Bson.Serialization.Attributes;
using Nest;
using System.Runtime.Serialization;
using ThirdParty.Json.LitJson;

namespace TaskMange.Models
{
    public class EmployeeMasterModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public int IsDelete { get; set; } = 0;
    }
    public class EmpModelforUpdate
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
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
        public int IsDelete { get; set; } = 0;
    }
    public class ProjectModel
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }
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
        public int IsDelete { get; set; } = 0;
    }
    public class EmpModel
    {
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
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
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        //public int CreatedBy { get; set; }
        //public int AssignTo { get; set; }
        public int[] CreatedBy { get; set; }
        public int[] AssignTo { get; set; }
        public DateTime TargetDate { get; set; }
        public string? Description { get; set; }
        public List<Activity> Activity { get; set; }
    }
    public class Result1
    {
        public string Status { get; set; }
        public string Message { get; set; } = "Done";
        
    }
    public class TaskOrder
    {
        public int TaskId { get; set; }
        public int OrderIndex { get; set; }
    }
    public class TaskAssign
    {
        public int TasKId { get; set; }
        public int AssignTo { get; set; }
    }
    public class TaskInsert
    {
        public int ProjectId { get; set; }
        public string? TaskName { get; set; } //tm title
        public string? Description { get; set; } //tm desc
        public DateTime? TargetDate { get; set; }
        public int AssignTo { get; set; } //ta assign
    }
    public class StatusChange
    {
        public int TaskId { get; set; }
        public int Status { get; set; }
    }

    public class Card
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? _id { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int[] CreatedBy { get; set; }
        public int[] AssignTo { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class List1
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<Card> cards { get; set; }
    }

    public class List2
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<Card> cards { get; set; }
    }

    public class List3
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<Card> cards { get; set; }
    }

    public class List4
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<Card> cards { get; set; }
    }

    public class Lists
    {
        public List1 list1 { get; set; }

        public List2 list2 { get; set; }

        public List3 list3 { get; set; }

        public List4 list4 { get; set; }
    }

    public class Root
    {
        public Lists lists { get; set; }
    }
    public class TaskUpdateModel
    {
        public int Id { get; set; }
        //public int ProjectMasterId { get; set; }
        //public string? TaskId {get;set;}
        public string? Title {get;set;}
        public string? Description {get;set;}
        public int Status {get;set;}
        public DateTime TargetDate {get;set;}
    }
}

