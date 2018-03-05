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
    public class DictionaryController : Controller
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

            dataSource.Add("contacts.index", "foo");
            dataSource.Add("contacts.index", "bar");

            dataSource.Add("contacts[foo].Key", "ABC");
            dataSource.Add("contacts[foo].Value.Name", "ABC");
            dataSource.Add("contacts[foo].Value.PhoneNo", "xx");
            dataSource.Add("contacts[foo].Value.EmailAddress", "yy@163.com");
            dataSource.Add("contacts[foo].Value.Address.Province", "yy@163.com");
            dataSource.Add("contacts[foo].Value.Address.City", "yywe");
            dataSource.Add("contacts[foo].Value.Address.District", "ywecom");
            dataSource.Add("contacts[foo].Value.Address.Street", "yyfj");

            dataSource.Add("contacts[foo].Key", "EFG");
            dataSource.Add("contacts[bar].Value.Name", "EFG");
            dataSource.Add("contacts[bar].Value.PhoneNo", "yy");
            dataSource.Add("contacts[bar].Value.EmailAddress", "xx@163.com");
            dataSource.Add("contacts[bar].Value.Address.Province", "yy@163.com");
            dataSource.Add("contacts[bar].Value.Address.City", "yywe");
            dataSource.Add("contacts[bar].Value.Address.District", "ywecom");
            dataSource.Add("contacts[bar].Value.Address.Street", "yyfj");

            this.ValueProvider = new NameValueCollectionValueProvider(dataSource, CultureInfo.CurrentCulture);
        }
        // GET: Complex
        public object Index()
        {
            return this.InvokeAction("DemoAction");
        }

        public ActionResult DemoAction(IDictionary<string,Contact> contacts)
        {
            var contactsArray = contacts.ToArray();
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            int i = 0;
            foreach (var item in contacts)
            {
                arguments.Add(string.Format("contacts[{0}].key", i), item.Key);
                arguments.Add(string.Format("contacts[{0}].Name", i), item.Value.Name);
                arguments.Add(string.Format("contacts[{0}].PhoneNo", i), item.Value.PhoneNo);
                arguments.Add(string.Format("contacts[{0}].EmailAddress", i), item.Value.EmailAddress);
                arguments.Add(string.Format("contacts[{0}].Address.Province", i), item.Value.Address.Province);
                arguments.Add(string.Format("contacts[{0}].Address.City", i), item.Value.Address.City);
                arguments.Add(string.Format("contacts[{0}].Address.District", i), item.Value.Address.District);
                arguments.Add(string.Format("contacts[{0}].Address.Street", i), item.Value.Address.Street);
                i++;
            }
            return View("Arguments", arguments);
        }
    }
}