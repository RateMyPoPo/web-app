using DropboxRestAPI;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Fedup.Controllers
{

    public class IncidentController : Controller
    {
        public struct incident {
            public double longitude;
            public double latitude;
            public double timestamp;
            public DateTime newTimeStamp;
            public string file_name;
        }

        // GET: Incident
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
                //initialize dropbox auth options
            var options = new Options
            {
                ClientId = "rnizd2wt4vhv4cn",
                ClientSecret = "k8exfknbce45n5g",
                RedirectUri = "https://fedup.azurewebsites.net/Incident"
                //RedirectUri = "http://localhost:49668/Incident"
            };

                // Initialize a new Client (without an AccessToken)
            var dropboxClient = new Client(options);

                // Get the OAuth Request Url
            var authRequestUrl = await dropboxClient.Core.OAuth2.AuthorizeAsync("code");

            if (Request.QueryString["code"] == null)
            {
                    // Navigate to authRequestUrl using the browser, and retrieve the Authorization Code from the response
                Response.Redirect(authRequestUrl.ToString());
            }
                //get authcode from querstring param
            var authCode = Request.QueryString["code"];

            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await dropboxClient.Core.OAuth2.TokenAsync(authCode);

            // Get account info
            var accountInfo = await dropboxClient.Core.Accounts.AccountInfoAsync();
            Console.WriteLine("Uid: " + accountInfo.uid);
            Console.WriteLine("Display_name: " + accountInfo.display_name);
            Console.WriteLine("Email: " + accountInfo.email);

            // Get root folder without content
            var rootFolder = await dropboxClient.Core.Metadata.MetadataAsync("/", list: false);
            Console.WriteLine("Root Folder: {0} (Id: {1})", rootFolder.Name, rootFolder.path);

            // Get root folder with content
            rootFolder = await dropboxClient.Core.Metadata.MetadataAsync("/", list: true);
            foreach (var folder in rootFolder.contents)
            {
                Console.WriteLine(" -> {0}: {1} (Id: {2})",
                    folder.is_dir ? "Folder" : "File", folder.Name, folder.path);
            }

            // Initialize a new Client (with an AccessToken)
            var client2 = new Client(options);

            // Find a file in the root folder
            var file = rootFolder.contents.FirstOrDefault(x => x.is_dir == false);
            var files = rootFolder.contents.ToList();

            // Download a file
            
            foreach (var item in files)
            {
                var tempFile = Path.GetTempFileName();
                if (item.path.Substring(item.path.Length - 4) == ".mp3")
                {
                    using (var fileStream = System.IO.File.OpenWrite(tempFile))
                    {

                        await client2.Core.Metadata.FilesAsync(item.path, fileStream);

                        fileStream.Flush();
                        fileStream.Close();
                    }

                    int length = item.path.Length;
                    string destination = item.path.Substring(0, length - 4) + ".mp3";
                    destination = AppDomain.CurrentDomain.BaseDirectory + destination.Substring(1);
                    System.IO.File.Copy(tempFile, destination, true);
                }
            }


            List<incident> Incidents = new List<incident>();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "0dwRwB8V2QmJTM9NbKtWfqlys91LwZguvE67oS1f",
                BasePath = "https://fedup.firebaseio.com/"
            };

                //initialize new firebase client
            IFirebaseClient client = new FirebaseClient(config);

            bool hasNext = true;
            int counter = 1;
            do
            {
                try
                {
                    //pull current row
                    FirebaseResponse response = await client.GetAsync(counter.ToString());

                    //parse our json object
                    incident jsonObject = JsonConvert.DeserializeObject<incident>(response.Body);

                    System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(jsonObject.timestamp).ToLocalTime();

                    jsonObject.newTimeStamp = dtDateTime;

                    Incidents.Add(jsonObject);
                }
                catch
                {
                    hasNext = false;
                    break;
                }
                counter++;
            }
            while (hasNext);

            ViewBag.Incidents = Incidents;

            return View();
        }
    }
}