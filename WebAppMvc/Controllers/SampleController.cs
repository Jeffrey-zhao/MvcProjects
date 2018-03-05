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
    public class SampleController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            NameValueCollection dataSource = new NameValueCollection();
            dataSource.Add("Foo", "ABC");
            dataSource.Add("bar","123");
            dataSource.Add("Baz","456.01");
            dataSource.Add("qux","789.12");
            this.ValueProvider = new NameValueCollectionValueProvider(dataSource, CultureInfo.CurrentCulture);
        }

        public object Index()
        {
            return this.InvokeAction("DemoAction");
        }

        public ActionResult DemoAction(
            string foo,
            int bar,
            [Bind(Prefix ="qux")]
            double baz)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("foo", foo);
            arguments.Add("bar", bar);
            arguments.Add("baz", baz);
            return View("Arguments", arguments);
        }
    }
}