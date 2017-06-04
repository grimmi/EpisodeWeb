using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EpisodeWeb.Controllers
{
    [Route("api/[controller]")]
    public class DecodedFilesController : Controller
    {
        [HttpGet]
        public async Task<JObject> Get()
        {
            var fileTasks = Directory.GetFiles(@"z:\downloads\decoded", "*.avi")
                .Select(f => new FileInfo(f))
                .OrderBy(i => i.CreationTime);
                //.Select(f => InfoCollector.GetEpisodeInfoTaskAsync(f.Name));

            //var fileInfos = await Task.WhenAll(fileTasks);

            var response = new JObject();
            response.Add("files", JToken.FromObject(null));

            return response;
        }
    }
}
