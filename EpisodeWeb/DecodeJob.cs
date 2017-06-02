using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EpisodeWeb
{
    public class DecodeJob
    {
        [JsonProperty("id")]
        public Guid Id { get; } = Guid.NewGuid();
        [JsonIgnore]
        public double ProgressValue { get; private set; } = 0;
        [JsonProperty("progress")]
        public decimal Progress => decimal.Parse(ProgressValue.ToString("N2"));
        [JsonProperty("currentstep")]
        public string CurrentStep { get; private set; } = "n/a";
        [JsonProperty("done")]
        public bool Done { get; set; } = false;
        [JsonIgnore]
        public List<string> Files { get; internal set; }

        public void Run()
        {
            for (int i = 0; i < Files.Count; i++)
            {
                ProgressValue = (100.0 / Files.Count) * (i+1);
                CurrentStep = Files[i];
                Thread.Sleep(500);
            }
            Done = true;
        }
    }
}
