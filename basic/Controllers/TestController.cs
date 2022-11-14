using basic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace basic.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            List<Employee> employeesList = new List<Employee>(); 
            Employee employee = new Employee();
            employeesList.Add(new Employee { Id = 1, Name = "parthipan", Deportment = "mech" });
            employeesList.Add(new Employee { Id = 2, Name = "sasi", Deportment = "cse" });
            employeesList.Add(new Employee { Id = 3, Name = "narmathaparthipan**", Deportment = "cse" });
            employeesList.Add(new Employee { Id = 4, Name = "nish", Deportment = "EEE" });
            employeesList.Add(new Employee { Id = 5, Name = "mani", Deportment = "IT" });
            return View(employeesList);
        }
    }
}