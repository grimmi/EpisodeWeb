using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class KeyFilesController : Controller
    {
        [HttpGet]
        public JObject Get()
        {
            var files = Directory.GetFiles(@"z:\downloads\done", "*.otrkey")
                .Select(f => new FileInfo(f))
                .OrderBy(i => i.CreationTime)
                .Select((f, idx) => new { Path = f.Name, Index = idx });

            var response = new JObject();
            response.Add("files", JToken.FromObject(files));

            return response;
        }
    }
}
