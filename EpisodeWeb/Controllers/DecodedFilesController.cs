using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
            var files = Directory.GetFiles(@"z:\downloads\decoded", "*.avi")
                .Select(f => new FileInfo(f))
                .OrderBy(i => i.CreationTime)
                .Select((f, idx) => new { Path = f.Name, Index = idx });

            var episode = await Renamer.getEpisodeAsTask(files.First().Path);

            var response = new JObject();
            response.Add("files", JToken.FromObject(files));

            return response;
        }
    }
}
