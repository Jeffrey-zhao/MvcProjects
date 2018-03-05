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
    public class ComplexController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            NameValueCollection dataSource = new NameValueCollection();
            dataSource.Add("foo.Name", "ABC");
            dataSource.Add("foo.PhoneNo", "123456789");
            dataSource.Add("foo.EmailAddress", "zhaoshuangwu@163.com");
            dataSource.Add("foo.Address.Provice", "上海");
            dataSource.Add("foo.Address.City", "上海");
            dataSource.Add("foo.Address.District", "闵行");
            dataSource.Add("foo.Address.Street", "吴泾");

            dataSource.Add("bar.Name", "EFG");
            dataSource.Add("bar.PhoneNo", "123456789");
            dataSource.Add("bar.EmailAddress", "zhaoshuangwu@163.com");
            dataSource.Add("bar.Address.Provice", "上海");
            dataSource.Add("bar.Address.City", "上海");
            dataSource.Add("bar.Address.District", "闵行");
            dataSource.Add("bar.Address.Street", "吴泾");

            this.ValueProvider = new NameValueCollectionValueProvider(dataSource, CultureInfo.CurrentCulture);
        }
        // GET: Complex
        public object Index()
        {
            return this.InvokeAction("DemoAction");
        }

        public ActionResult DemoAction(Contact foo,Contact bar)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("foo.Name", foo.Name);
            arguments.Add("foo.PhoneNo", foo.PhoneNo);
            arguments.Add("foo.EmailAddress", foo.EmailAddress);
            Address address1 = foo.Address;
            arguments.Add("foo.Address", string.Format("{0} provice {1} city {2} district {3} street",address1.Province,address1.City,address1.District,address1.Street));

            arguments.Add("bar.Name", bar.Name);
            arguments.Add("bar.PhoneNo", bar.PhoneNo);
            arguments.Add("bar.EmailAddress", bar.EmailAddress);
            Address address2 = bar.Address;
            arguments.Add("bar.Address", string.Format("{0} provice {1} city {2} district {3} street", address2.Province, address2.City, address2.District, address2.Street));

            return View("Arguments", arguments);
        }
    }
}