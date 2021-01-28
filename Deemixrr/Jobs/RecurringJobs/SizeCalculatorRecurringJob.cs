using System;
using System.IO;
using System.Threading.Tasks;

using Deemixrr.Helpers;
using Deemixrr.Repositories;

using Hangfire;
using Hangfire.Console;
using Hangfire.Server;

namespace Deemixrr.Jobs.RecurringJobs
{
    public class SizeCalculatorRecurringJob : IRecurringJob
    {
        private readonly IDataRepository _dataRepository;

        public SizeCalculatorRecurringJob(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(PerformContext context)
        {
            var folders = await _dataRepository.GetFolders();

            var overallProgressbar = context.WriteProgressBar();

            foreach (var folder in folders.WithProgress(overallProgressbar))
            {
                folder.Size = IOHelpers.DirSize(new DirectoryInfo(folder.Path));

                await _dataRepository.UpdateFolder(folder);
            }
        }
    }
}