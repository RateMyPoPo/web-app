using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fedup.Controllers
{
    public class IncidentController : Controller
    {
        public struct incident {
            public double longitute;
            public double latitude;
            public DateTime timestamp;
            public string dropBoxURL;
        }

        // GET: Incident
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<incident> Incidents = new List<incident>();

            incident incident1 = new incident();
            incident incident2 = new incident();
            incident incident3 = new incident();

            incident1.latitude = 51.5033630;
            incident1.longitute = -0.1276250;
            incident1.timestamp = new DateTime(2015, 9, 15, 7, 0, 0);
            incident1.dropBoxURL = "about:home";

            incident2.latitude = 38.6043618;
            incident2.longitute = -90.5857429;
            incident2.timestamp = new DateTime(2015, 9, 1, 7, 0, 0);
            incident2.dropBoxURL = "about:home";

            incident3.latitude = 37.7892969;
            incident3.longitute = -122.3892897;
            incident3.timestamp = new DateTime(2015, 9, 5, 7, 0, 0);
            incident3.dropBoxURL = "about:home";

            Incidents.Add(incident1);
            Incidents.Add(incident2);
            Incidents.Add(incident3);

            ViewBag.Incidents = Incidents;

            return View();
        }
    }
}