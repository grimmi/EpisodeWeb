using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class DecodeController : Controller
    {
        [HttpGet]
        public JObject EncodedFiles()
        {
            var files = Directory.GetFiles(@"z:\downloads\done", "*.otrkey")
                .OrderBy(f => Path.GetFileName(f))
                .Select((f, i) => new { Path = Path.GetFileName(f), Index = i });

            var response = new JObject();
            response.Add("files", JToken.FromObject(files));

            return response;
        }
    }
}
