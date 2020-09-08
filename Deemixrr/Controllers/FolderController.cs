using System;
using System.Threading.Tasks;

using Deemixrr.Data;
using Deemixrr.Models;
using Deemixrr.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Deemixrr.Controllers
{
    [Authorize]
    public class FolderController : Controller
    {
        private readonly ILogger<FolderController> _logger;
        private readonly IDataRepository _dataRepository;

        public FolderController(ILogger<FolderController> logger, IDataRepository dataRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var folders = await _dataRepository.GetFolders(0, 100);

            return View(new FolderIndexInputViewModel
            {
                Folders = folders
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(FolderIndexInputViewModel model)
        {
            var folders = await _dataRepository.GetFolders(model.SearchTerm);

            return View(new FolderIndexInputViewModel
            {
                Folders = folders
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FolderCreateInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _dataRepository.CreateFolder(new Folder()
            {
                Name = model.Name,
                Path = model.Path
            });

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var folder = await _dataRepository.GetFolder(id);
            var folderArtistCount = await _dataRepository.GetFolderArtistCount(folder);

            return View(new FolderDeleteViewModel()
            {
                Folder = folder,
                ArtistCount = folderArtistCount
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(FolderDeleteViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var folder = await _dataRepository.GetFolder(model.Folder.Id);
            if (folder == null) return BadRequest();

            await _dataRepository.DeleteFolder(folder);

            return RedirectToAction(nameof(Index));
        }


    }
}