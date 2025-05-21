
using Microsoft.AspNetCore.Mvc;
using NASA_API.Services;

namespace NASA_API.Controllers
{
    public class APODController : Controller
    {
        private readonly INasaService _nasaService;
        
        public APODController(INasaService nasaService)
        {
            _nasaService = nasaService;
        }
        public async Task<IActionResult> PictureOfTheDay(DateTime? date = null)
        {
            var pictureOfTheDay = await _nasaService.GetApodByDateAsync(date);

            return View(pictureOfTheDay);
        }

        
    }
}
