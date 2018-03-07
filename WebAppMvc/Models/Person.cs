using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppMvc.Attributes;
using WebAppMvc.Properties;

namespace WebAppMvc.Models
{
    public class Person
    {
        [DisplayName("姓名")]
        [Required(ErrorMessageResourceName ="Required",ErrorMessageResourceType =typeof(Resources))]
        public string Name { get; set; }
        [DisplayName("性别")]
        [Required(ErrorMessageResourceName ="Required",ErrorMessageResourceType =typeof(Resources))]
        [Domain("M","F","m","f",ErrorMessageResourceType =typeof(Resources),ErrorMessageResourceName ="Domain")]
        public string Gender { get; set; }
        [DisplayName("年龄")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources))]
        [Range(18,25,ErrorMessageResourceName ="Range",ErrorMessageResourceType =typeof(Resources))]
        public int? Age { get; set; }
    }
}