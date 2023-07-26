using Microsoft.AspNetCore.Mvc;
using TaskMange.Controllers;
using TaskMange.Models;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskManageAPIController : ControllerBase
    {
        //Employee Master
        [HttpGet]
        [Route("/employee")]
        public List<EmployeeMasterModel> EmpDisplay()
        {
            return EmployeeMasterController.GetRecords();
        }

        [HttpPost]
        [Route("/employee")]
        public Result1 EmpInsert(EmpModel obj)
        {
            EmployeeMasterModel obj1 = new EmployeeMasterModel();
            obj1.EmployeeName = obj.EmployeeName;
            obj1.Email = obj.Email;
            obj1.Role = obj.Role;
            obj1.Status = 1; 
            Result1 res = EmployeeMasterController.InsertDocument(obj1);
            return res;
        }

        [HttpPut]
        [Route("/employee")]
        public Result1 EmpUpdate(EmpModelforUpdate obj1)
        {
            EmployeeMasterModel obj = new EmployeeMasterModel();
            obj.Id = obj1.Id;
            obj.EmployeeName = obj1.EmployeeName;
            obj.Email = obj1.Email;
            obj.Role = obj1.Role;
            obj.Status = obj1.Status;
            Result1 status = EmployeeMasterController.UpdateDocument(obj);
            return status;
        }

        [HttpDelete]
        [Route("/employee")]
        public Result1 EmpDelete(int Id)
        {
            Result1 status = EmployeeMasterController.DeleteDocument(Id);
            return status;
        }
        // Project Master
        [HttpGet]
        [Route("/project")]
        public List<ProjectMasterModel> ProjectDisplay()
        {
            return ProjectMasterController.GetRecords();
        }

        [HttpPost]
        [Route("/project")]
        public Result1 ProjectInsert(string ProjectName)
        {
            ProjectMasterModel obj = new ProjectMasterModel();
            obj.ProjectName = ProjectName;
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            Result1 status = ProjectMasterController.InsertDocument(obj);
            return status;
        }

        [HttpPut]
        [Route("/project")]
        public Result1 ProjectUpdate(ProjectModel obj1)
        {
            ProjectMasterModel obj = new ProjectMasterModel();
            obj.Id = obj1.Id;
            obj.ProjectName = obj1.ProjectName;
            obj.UpdatedDate = DateTime.Now;
            Result1 status = ProjectMasterController.UpdateDocument(obj);
            return status;
        }

        [HttpDelete]
        [Route("/project")]
        public Result1 ProjectDelete(int Id)
        {
            Result1 status = ProjectMasterController.DeleteDocument(Id);
            return status;
        }

        // Task Master
        [HttpGet]
        [Route("/task")]
        public Root Task(int? pageindex, int? pagesize, string? search, int? status)
        {
            if (pageindex == null)
                pageindex = 1;
            if (pagesize == null)
                pagesize = 10;
            if (status == null)
                status = 0;
            var res = TaskController.TaskFindWithPage(Convert.ToInt32(status), search, Convert.ToInt32(pageindex), Convert.ToInt32(pagesize));
            return res;
        }

        [HttpGet]
        [Route("/task/{TaskId}")]
        public List<TaskMasterApiResult> Task_Id(int TaskId) 
        {
            return TaskController.getTask(TaskId); //TaskId
        }

        [HttpPost]
        [Route("/task")]
        public Result1 InsertTaskRecord(TaskInsert obj)
        {
            Result1 res = TaskController.InsertIntoTask(obj);
            return res;
        }
        
        [HttpPut]
        [Route("/task/update")]
        public Result1 TaskUpdate(TaskUpdateModel obj)
        {
            Result1 status = TaskController.UpdateTaskMaster(obj);
            return status;
        }


        [HttpDelete]
        [Route("/task/delete")]
        public Result1 TaskDelete(int Id)
        {
            Result1 status = TaskController.DeleteDocument(Id);
            return status;
        }

        [HttpPost]
        [Route("/task/assignto")]
        public Result1 InsertRecordToTaskAssign(TaskAssign obj)
        {
            var res = TaskController.InsertIntoAssignTo(obj);
            return res;
        }

        [HttpPost]
        [Route("/task/order")]
        public Result1 InsertRecordToTaskOrder(TaskOrder obj)
        {
            var res = TaskController.InsertIntoOrder(obj);
            return res;
        }

        [HttpPost]
        [Route("/task/status")]
        public Result1 UpdateToActivity(StatusChange obj)
        {
            var res = TaskController.UpdateStatus(obj);
            return res;
        }
    }
}