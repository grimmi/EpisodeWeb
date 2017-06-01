using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EpisodeWeb
{
    public class DecodeJob
    {
        public Guid Id { get; } = Guid.NewGuid();
        [JsonIgnore]
        public double ProgressValue { get; private set; } = 0;
        public decimal Progress
        {
            get
            {
                var progValue = ProgressValue.ToString("N2");
                //progValue = progValue.Replace(",", ".");
                return decimal.Parse(progValue);
            }
        }
        public string CurrentStep { get; private set; } = "n/a";
        public bool Done { get; set; } = false;
        [JsonIgnore]
        public List<string> Files { get; internal set; }

        public async Task Run()
        {
            for (int i = 0; i < Files.Count; i++)
            {
                ProgressValue = (100.0 / Files.Count) * (i+1);
                CurrentStep = Files[i];
                await Task.Delay(1000);
            }
            Done = true;
        }
    }
}
