using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Deemix.AutoLoader.Models;
using Deemix.AutoLoader.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Deemix.AutoLoader.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataRepository _dataRepository;

        public HomeController(ILogger<HomeController> logger, IDataRepository dataRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new HomeIndexViewModel
            {
                ArtistCount = await _dataRepository.GetArtistCount(),
                FolderCount = await _dataRepository.GetFolderCount(),
                PlaylistCount = await _dataRepository.GetPlaylistCount(),
                FolderSizeCumulated = await _dataRepository.GetFolderSizeCumulated(),
                Artists = await _dataRepository.GetLastUpdatedArtists(0, 10),
                Playlists = await _dataRepository.GetLastUpdatedPlaylists(0, 10),
                Folders = await _dataRepository.GetLastUpdatedFolders(0, 10)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(ArtistIndexInputViewModel model)
        {
            return View(new HomeIndexViewModel
            {
                ArtistCount = await _dataRepository.GetArtistCount(),
                FolderCount = await _dataRepository.GetFolderCount(),
                PlaylistCount = await _dataRepository.GetPlaylistCount(),
                FolderSizeCumulated = await _dataRepository.GetFolderSizeCumulated()
            });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
