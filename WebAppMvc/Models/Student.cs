using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppMvc.Models
{
    public class Student:IValidatableObject
    {
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("性别")]
        public string Gender { get; set; }
        [DisplayName("年龄")]
        public int? Age { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Student student = validationContext.ObjectInstance as Student;
            if (null == student)
            {
                yield break;
            }
            if (string.IsNullOrEmpty(student.Name))
            {
                yield return new ValidationResult("'Name' 是必须字段", new string[] { "Name" });
            }
            if (string.IsNullOrEmpty(student.Gender))
            {
                yield return new ValidationResult("'Gender' 是必须字段", new string[] { "Age" });
            }
            else if (!new string[] { "M","F","m","f"}.Any(g=>string.Compare(student.Gender,g,true)==0))
            {
                yield return new ValidationResult("有效'Gender' 必须是'M','F','m','f'之一", new string[] { "Gender" });
            }

            if (null== student.Age)
            {
                yield return new ValidationResult("'Age' 是必须字段", new string[] { "Age" });
            }
            else if(student.Age > 25 || student.Age < 18)
            {
                yield return new ValidationResult("有效'Age' 必须是18 到 25之间", new string[] { "Age" });
            }

        }
    }
}