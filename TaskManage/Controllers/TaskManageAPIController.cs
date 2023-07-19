using Microsoft.AspNetCore.Mvc;
using TaskMange.Controllers;
using TaskMange.Models;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskManageAPIController : ControllerBase
    {
        //private readonly ILogger<TaskManageAPIController> _logger;

        //public TaskManageAPIController(ILogger<TaskManageAPIController> logger)
        //{
        //    _logger = logger;
        //}

        //Employee Master
        [HttpGet]
        [Route("employee")]
        public List<EmployeeMasterModel> EmpDisplay()
        {
            return EmployeeMasterController.GetRecords();
        }

        [HttpPost]
        [Route("employee")]
        public string EmpInsert(string EmployeeCode, string EmployeeName, string Email, int Role)
        {
            EmployeeMasterModel obj = new EmployeeMasterModel();
            obj.EmployeeCode = EmployeeCode; 
            obj.EmployeeName = EmployeeName;
            obj.Email = Email;
            obj.Role = Role;
            string status = EmployeeMasterController.InsertDocument(obj);
            return status;
        }

        [HttpPut]
        [Route("employee")]
        public string EmpUpdate(int Id, string EmployeeCode, string EmployeeName, string Email, int Role)
        {
            EmployeeMasterModel obj = new EmployeeMasterModel();
            obj.Id = Id;
            obj.EmployeeCode = EmployeeCode;
            obj.EmployeeName = EmployeeName;
            obj.Email = Email;
            obj.Role = Role;
            string status = EmployeeMasterController.UpdateDocument(obj);
            return status;
        }

        [HttpDelete]
        [Route("employee")]
        public string EmpDelete(int Id)
        {
            string status = EmployeeMasterController.DeleteDocument(Id);
            return status;
        }
        // Project Master
        [HttpGet]
        [Route("Project")]
        public List<ProjectMasterModel> ProjectDisplay()
        {
            return ProjectMasterController.GetRecords();
        }

        [HttpPost]
        [Route("Project")]
        public string ProjectInsert(string ProjectCode, string ProjectName)
        {
            ProjectMasterModel obj = new ProjectMasterModel();
            obj.ProjectCode = ProjectCode;
            obj.ProjectName = ProjectName;
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            string status = ProjectMasterController.InsertDocument(obj);
            return status;
        }

        [HttpPut]
        [Route("Project")]
        public string ProjectUpdate(int Id,string ProjectCode, string ProjectName)
        {
            ProjectMasterModel obj = new ProjectMasterModel();
            obj.Id = Id;
            obj.ProjectCode = ProjectCode;
            obj.ProjectName = ProjectName;
            obj.UpdatedDate = DateTime.Now;
            string status = ProjectMasterController.UpdateDocument(obj);
            return status;
        }

        [HttpDelete]
        [Route("Project")]
        public string ProjectDelete(int Id)
        {
            string status = ProjectMasterController.DeleteDocument(Id);
            return status;
        }

        // Task Master
        [HttpGet]
        [Route("Task")]
        public List<TaskMasterApiResult> Task_Id(int TaskId)
        {
            return TaskController.getTask(TaskId);
        }
        [HttpGet]
        [Route("/task/assignto")]
        public List<Result> TaskAssignTo(TaskAssign obj)
        {
            var res = TaskController.AssignToStatus(obj);
            return res;
        }

        [HttpGet]
        [Route("/task/order")]
        public List<Result> TaskOrder(TaskOrder obj)
        {
            var res = TaskController.OrderStatus(obj);
            return res;
        }

        [HttpGet]
        [Route("/task")]
        public List<TaskMasterModel> Task(int status, int pageindex, int pagesize, string search)
        {
            var res = TaskController.TaskFindWithPage(status, search, pageindex, pagesize);
            return res;
        }
    }
}