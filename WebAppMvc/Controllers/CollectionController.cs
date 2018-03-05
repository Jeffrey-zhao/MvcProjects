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
    public class CollectionController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            NameValueCollection dataSource = new NameValueCollection();
            dataSource.Add("foo", "ABC");
            dataSource.Add("foo", "xx");
            dataSource.Add("foo", "yy@163.com");

            dataSource.Add("bar", "456");
            dataSource.Add("bar", "123");
            dataSource.Add("bar", "789");

            this.ValueProvider = new NameValueCollectionValueProvider(dataSource, CultureInfo.CurrentCulture);
        }
        // GET: Complex
        public object Index()
        {
            return this.InvokeAction("DemoAction");
        }

        public ActionResult DemoAction(string[] foo,IEnumerable<int> bar)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            for (int i = 0; i < foo.Length; i++)
            {
                arguments.Add(string.Format("foo[{0}]",i),foo[i]);
            }
            int j = 0;
            foreach (var item in bar)
            {
                arguments.Add(string.Format("bar[{0}]", j++), item);
            }
            return View("Arguments", arguments);
        }
    }
}