using System;
using System.IO;
using System.Threading.Tasks;

using Deemix.AutoLoader.Helpers;
using Deemix.AutoLoader.Repositories;

using Hangfire;

namespace Deemix.AutoLoader.Jobs.RecurringJobs
{
    public class SizeCalculatorRecurringJob : IRecurringJob
    {
        private readonly IDataRepository _dataRepository;

        public SizeCalculatorRecurringJob(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute()
        {
            var folders = await _dataRepository.GetFolders();

            foreach (var folder in folders)
            {
                folder.Size = IOHelpers.DirSize(new DirectoryInfo(folder.Path));

                await _dataRepository.UpdateFolder(folder);
            }
        }
    }
}