using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppMvc.Models;

namespace WebAppMvc.Controllers
{
    public class ValidationController : Controller
    {
        // GET: Validation
        [HttpGet]
        public ActionResult Index()
        {
            return View(new Person());
        }

        public ActionResult Index(Person person)
        {
            //Validate(person);
            if (!ModelState.IsValid)
            {
                return View(person);
            }
            else
            {
                return Content("输入数据通过验证");
            }
        }
        //IValidatableObject
        [HttpGet]
        public ActionResult Student()
        {
            return View(new Student());
        }
        public ActionResult Student(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }
            else
            {
                return Content("输入数据通过验证");
            }
        }
        //IDataErrorInfo
        [HttpGet]
        public ActionResult Teacher()
        {
            return View(new Teacher());
        }
        public ActionResult Teacher(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }
            else
            {
                return Content("输入数据通过验证");
            }
        }
        private void Validate(Person person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                ModelState.AddModelError("Name", "'Name'是必须字段");
            }
            if (string.IsNullOrEmpty(person.Gender))
            {
                ModelState.AddModelError("Gender", "'Gender'是必须字段");
            }
            else if (!new string[] { "M", "F" }.Any(g => string.Compare(person.Gender, g, true) == 0))
            {
                ModelState.AddModelError("Gender", "有效'Gender'必须是'M','F'之一");
            }
            if (null == person.Age)
            {
                ModelState.AddModelError("Age", "'Age'是必须字段");
            }
            else if (person.Age > 25 || person.Age < 18)
            {
                ModelState.AddModelError("Age", "有效'Age'必须是 18 到 25 之间");
            }
        }
    }
}