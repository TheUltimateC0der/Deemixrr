using AutoMapper;

using Deemixrr.Models;
using Deemixrr.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace Deemixrr.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IDeemixService _deemixService;
        private readonly IMapper _mapper;

        public SettingsController(ILogger<SettingsController> logger, IDeemixService deemixService, IMapper mapper)
        {
            _logger = logger;
            _deemixService = deemixService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Arl()
        {
            return View(_mapper.Map<SettingsArlInputViewModel>(await _deemixService.GetArl()));
        }

        [HttpPost]
        public async Task<IActionResult> Arl(SettingsArlInputViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            await _deemixService.SetArl(model.Arl);

            return RedirectToAction(nameof(Arl));
        }
    }
}