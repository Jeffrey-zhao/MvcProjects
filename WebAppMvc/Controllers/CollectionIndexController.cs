using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebAppMvc.Models;

namespace WebAppMvc.Controllers
{
    public class CollectionIndexController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            NameValueCollection dataSource = new NameValueCollection();
            //dataSource.Add("contacts1[0].Name", "ABC");
            //dataSource.Add("contacts1[0].PhoneNo", "xx");
            //dataSource.Add("contacts1[0].EmailAddress", "yy@163.com");
            //dataSource.Add("contacts1[0].Address.Province", "yy@163.com");
            //dataSource.Add("contacts1[0].Address.City", "yywe");
            //dataSource.Add("contacts1[0].Address.District", "ywecom");
            //dataSource.Add("contacts1[0].Address.Street", "yyfj");

            //dataSource.Add("contacts2[0].Name", "ert");
            //dataSource.Add("contacts2[0].PhoneNo", "yy");
            //dataSource.Add("contacts2[0].EmailAddress", "xx@163.com");
            //dataSource.Add("contacts2[0].Address.Province", "yy@163.com");
            //dataSource.Add("contacts2[0].Address.City", "yywe");
            //dataSource.Add("contacts2[0].Address.District", "ywecom");
            //dataSource.Add("contacts2[0].Address.Street", "yyfj");

            dataSource.Add("contacts1.index", "foo");
            dataSource.Add("contacts2.index", "bar");

            dataSource.Add("contacts1[foo].Name", "ABC");
            dataSource.Add("contacts1[foo].PhoneNo", "xx");
            dataSource.Add("contacts1[foo].EmailAddress", "yy@163.com");
            dataSource.Add("contacts1[foo].Address.Province", "yy@163.com");
            dataSource.Add("contacts1[foo].Address.City", "yywe");
            dataSource.Add("contacts1[foo].Address.District", "ywecom");
            dataSource.Add("contacts1[foo].Address.Street", "yyfj");

            dataSource.Add("contacts2[bar].Name", "ert");
            dataSource.Add("contacts2[bar].PhoneNo", "yy");
            dataSource.Add("contacts2[bar].EmailAddress", "xx@163.com");
            dataSource.Add("contacts2[bar].Address.Province", "yy@163.com");
            dataSource.Add("contacts2[bar].Address.City", "yywe");
            dataSource.Add("contacts2[bar].Address.District", "ywecom");
            dataSource.Add("contacts2[bar].Address.Street", "yyfj");

            this.ValueProvider = new NameValueCollectionValueProvider(dataSource, CultureInfo.CurrentCulture);
        }
        // GET: Complex
        public object Index()
        {
            return this.InvokeAction("DemoAction");
        }

        public ActionResult DemoAction(IEnumerable<Contact> contacts1,IEnumerable<Contact> contacts2)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            int i = 0;
            foreach (var item in contacts1)
            {
                arguments.Add(string.Format("contacts1[{0}].Name", i), item.Name);
                arguments.Add(string.Format("contacts1[{0}].PhoneNo", i), item.PhoneNo);
                arguments.Add(string.Format("contacts1[{0}].EmailAddress", i), item.EmailAddress);
                arguments.Add(string.Format("contacts1[{0}].Address.Province", i), item.Address.Province);
                arguments.Add(string.Format("contacts1[{0}].Address.City", i), item.Address.City);
                arguments.Add(string.Format("contacts1[{0}].Address.District", i), item.Address.District);
                arguments.Add(string.Format("contacts1[{0}].Address.Street", i), item.Address.Street);
                i++;
            }

            int j = 0;
            foreach (var item in contacts2)
            {
                arguments.Add(string.Format("contacts2[{0}].Name", j), item.Name);
                arguments.Add(string.Format("contacts2[{0}].PhoneNo", j), item.PhoneNo);
                arguments.Add(string.Format("contacts2[{0}].EmailAddress", j), item.EmailAddress);
                arguments.Add(string.Format("contacts2[{0}].Address.Province", j), item.Address.Province);
                arguments.Add(string.Format("contacts2[{0}].Address.City", j), item.Address.City);
                arguments.Add(string.Format("contacts2[{0}].Address.District", j), item.Address.District);
                arguments.Add(string.Format("contacts2[{0}].Address.Street", j), item.Address.Street);
                j++;
            }
            return View("Arguments", arguments);
        }
    }
}