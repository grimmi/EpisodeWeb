using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpisodeWeb
{
    public class DecodeJobService : IDisposable
    {
        private DecodeJob currentJob;

        public DecodeJob CurrentJob => currentJob;

        public DecodeJobService(IConfigurationRoot configuration)
        {
        }

        public void RunJob(IEnumerable<string> files)
        {
            if (CurrentJob == null || CurrentJob.Done)
            {
                var job = new DecodeJob();
                job.Files = files.ToList();
                currentJob = job;
                job.Run();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
