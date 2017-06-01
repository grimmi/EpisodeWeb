using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class DecodeController : Controller
    {
        private static DecodeJob currentJob;

        [HttpGet]
        [Route("EncodedFiles")]
        public JObject EncodedFiles()
        {
            var files = Directory.GetFiles(@"z:\downloads\done", "*.otrkey")
                .OrderBy(f => Path.GetFileName(f))
                .Select((f, i) => new { Path = Path.GetFileName(f), Index = i });

            var response = new JObject();
            response.Add("files", JToken.FromObject(files));

            return response;
        }

        [HttpPost]
        [Route("DecodeFiles")]
        public JObject DecodeFiles(string files)
        {
            if(currentJob == null || currentJob.Done)
            {
                currentJob = new DecodeJob();
                currentJob.Run();
            }

            var response = new JObject();
            response["id"] = currentJob.Id.ToString();
            response["progress"] = currentJob.Progress;
            response["currentstep"] = currentJob.CurrentStep;
            response["done"] = currentJob.Done;

            return response;
        }
    }
}
