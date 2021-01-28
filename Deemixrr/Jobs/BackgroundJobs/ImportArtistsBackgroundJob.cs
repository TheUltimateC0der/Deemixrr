using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deemixrr.Configuration;
using Deemixrr.Data;
using Deemixrr.Jobs.Models;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;
using Hangfire.Server;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public class ImportArtistsBackgroundJob : IBackgroundJob<string>
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly DelayConfiguration _delayConfiguration;

        public ImportArtistsBackgroundJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, DelayConfiguration delayConfiguration)
        {
            _deezerApiService = deezerApiService;
            _dataRepository = dataRepository;
            _delayConfiguration = delayConfiguration;
        }

        public async Task Execute(string param, PerformContext context)
        {
            var folder = await _dataRepository.GetFolder(param);
            if (folder != null)
            {
                try
                {
                    folder.State = Enums.ProcessingState.Processing;
                    await _dataRepository.UpdateFolder(folder);

                    var dirs = Directory.GetDirectories(folder.Path);

                    foreach (var dir in dirs)
                    {
                        var dirName = Path.GetFileName(dir);
                        var foundArtists = await _deezerApiService.GetDeezerApi().Search.FindArtists(dirName, CancellationToken.None, 0, 1);
                        var firstArtist = foundArtists.FirstOrDefault();

                        if (firstArtist != null)
                        {
                            var dbArtist = await _dataRepository.GetArtist(firstArtist.Id);
                            if (dbArtist == null)
                            {
                                BackgroundJob.Enqueue<CreateArtistBackgroundJob>(
                                    x => x.Execute(
                                        new CreateArtistBackgroundJobData()
                                        {
                                            ArtistDeezerId = firstArtist.Id,
                                            FolderId = folder.Id
                                        }, null)
                                );
                            }
                        }

                        await Task.Delay(_delayConfiguration.ImportArtistsBackgroundJob_ExecuteDelay);
                    }

                    folder.State = Enums.ProcessingState.None;
                }
                catch (Exception e)
                {
                    folder.State = Enums.ProcessingState.Failed;

                    throw e;
                }

                await _dataRepository.UpdateFolder(folder);
            }
        }
    }
}