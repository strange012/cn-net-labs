using LabClassLibrary.BusinessLogic;
using LabClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LabServiceCore.Controllers
{
    [Route("employees")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            var server = new EmployeeServer();
            var employees = server.GetStoredCollection();

            return employees;
        }

        [HttpGet]
        [Route("initialize")]
        public void Initialize()
        {
            var server = new EmployeeServer();
            var employees = server.InitializeCollection();
            server.SaveCollection(employees);
        }

        [HttpPost]
        [Route("add")]
        public void Add([FromBody]Employee employee)
        {
            EventLog.WriteEntry(
                "Application", 
                DateTime.Now + " Received:" + employee.Name + " " + employee.Address);

            var server = new EmployeeServer();
            server.AddToCollection(employee);
        }
    }
}
