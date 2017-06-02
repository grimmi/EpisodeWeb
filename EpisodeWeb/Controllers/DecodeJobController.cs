using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class DecodeJobController : Controller
    {
        private DecodeJobService JobService { get; }
        private DecodeJob CurrentJob => JobService.CurrentJob;

        public DecodeJobController(DecodeJobService jobService)
        {
            JobService = jobService;
        }

        [HttpGet]
        public JObject Get()
        {
            if(CurrentJob != null)
            {
                return JObject.Parse(JsonConvert.SerializeObject(CurrentJob));
            }

            var response = new JObject();
            response.Add("message", "no active job");

            return response;
        }

        [HttpPost]
        public JObject Post(string files)
        {
            if (CurrentJob == null || CurrentJob.Done)
            {
                
                BackgroundJob.Enqueue(() => JobService.RunJob(files.Split(',').Select(f => f.Trim()).ToList()));
                var jobCreatedResponse = new JObject();
                jobCreatedResponse.Add("message", "job created");
                return jobCreatedResponse;
            }

            var response = new JObject();
            response.Add("error", "job already running");
            return response;
        }
    }
}
