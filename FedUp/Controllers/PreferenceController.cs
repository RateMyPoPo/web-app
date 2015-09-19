using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fedup.Controllers
{
    public class PreferenceController : Controller
    {
        // GET: Preference
        public ActionResult Index()
        {
            return View();
        }
    }
}