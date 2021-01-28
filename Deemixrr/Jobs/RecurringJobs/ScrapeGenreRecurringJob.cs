using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Data;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;
using Hangfire.Server;

namespace Deemixrr.Jobs.RecurringJobs
{
    public class ScrapeGenreRecurringJob : IRecurringJob
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;

        public ScrapeGenreRecurringJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IMapper mapper)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(PerformContext context)
        {
            var genres = await _deezerApiService.GetDeezerApi().Genre.GetCommonGenre(CancellationToken.None);

            foreach (var genre in genres)
            {
                var dbGenre = await _dataRepository.GetGenre(genre.Id);
                if (dbGenre == null)
                {
                    await _dataRepository.CreateGenre(_mapper.Map<Genre>(genre));
                }
            }
        }
    }
}