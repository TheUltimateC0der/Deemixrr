using System;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Data;
using Deemixrr.Jobs.BackgroundJobs;
using Deemixrr.Jobs.Models;
using Deemixrr.Models;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Deemixrr.Controllers
{
    [Authorize]
    public class PlaylistController : Controller
    {
        private readonly ILogger<PlaylistController> _logger;
        private readonly IDataRepository _dataRepository;
        private readonly IDeezerApiService _deezerApiService;
        private readonly IMapper _mapper;

        public PlaylistController(ILogger<PlaylistController> logger, IDataRepository dataRepository, IDeezerApiService deezerApiService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }


        [HttpGet]
        public async Task<IActionResult> Test()
        {
            var playlists = await _dataRepository.GetPlaylists();

            foreach (var playlist in playlists)
            {
                BackgroundJob.Enqueue<CheckPlaylistForUpdatesBackgroundJob>(x => x.Execute(playlist.DeezerId, false));
            }

            return View(nameof(Index), new PlaylistIndexInputViewModel
            {
                Playlists = playlists
            });
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlists = await _dataRepository.GetPlaylists(0, 100);

            return View(new PlaylistIndexInputViewModel()
            {
                Playlists = playlists
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(PlaylistIndexInputViewModel model)
        {
            var playlists = await _dataRepository.GetPlaylists(model.SearchTerm);

            return View(new PlaylistIndexInputViewModel
            {
                Playlists = playlists
            });
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var folders = await _dataRepository.GetFolders();

            return View(new PlaylistCreateInputModel()
            {
                Folders = new SelectList(folders, nameof(Folder.Id), nameof(Folder.NamePath))
            });
        }

        [HttpPost]
        public IActionResult Create(PlaylistCreateInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            BackgroundJob.Enqueue<CreatePlaylistBackgroundJob>(x => x.Execute(_mapper.Map<CreatePlaylistBackgroundJobData>(model), false));

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var playlist = await _dataRepository.GetPlaylist(id);

            return View(new PlaylistDeleteViewModel
            {
                Playlist = playlist
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(PlaylistDeleteViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var artist = await _dataRepository.GetPlaylist(model.Playlist.Id);
            if (artist == null) return BadRequest();

            await _dataRepository.DeletePlaylist(artist);

            return RedirectToAction(nameof(Index));
        }
    }
}