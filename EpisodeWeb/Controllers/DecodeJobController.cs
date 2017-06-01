using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class DecodeJobController : Controller
    {
        private static DecodeJob currentJob;

        [HttpGet]
        public JObject Get()
        {
            if(currentJob != null)
            {
                return JObject.Parse(JsonConvert.SerializeObject(currentJob));
            }

            var response = new JObject();
            response.Add("message", "no active job");

            return response;
        }

        [HttpPost]
        public JObject Post(string files)
        {
            if (currentJob == null || currentJob.Done)
            {
                var job = new DecodeJob();
                job.Files = files.Split(',').ToList();
                job.Run();
                currentJob = job;
                return JObject.Parse(JsonConvert.SerializeObject(job));
            }

            var response = new JObject();
            response.Add("error", "job already running");
            return response;
        }
    }
}
