using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using NASA_API.Models.ViewModels;
using NASA_API.Services;
using System.Net;

namespace NASA_API.Controllers
{
    public class AsteroidsController : Controller
    {
        private readonly INasaService _nasaService;
        public AsteroidsController(INasaService nasaService)
        {
            _nasaService = nasaService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            var from = startDate ?? DateTime.Today;
            var to = endDate ?? DateTime.Today.AddDays(1);

            if (startDate > endDate )
            {
                ModelState.AddModelError("", "endDate cannot be earlier than startDate");
            }
            if ((to - from).TotalDays > 7)
            {
                ModelState.AddModelError("", "NASA NEO API only supports a 7-day date range.");
            }
            if (!ModelState.IsValid)
            {
                return View(new List<AsteroidViewModel>());
            }

            ViewBag.StartDate = from;
            ViewBag.EndDate = to;
            var asteroids = await _nasaService.GetAsteroidsAsync(from, to);
            return View(asteroids);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(DateTime startDate, DateTime endDate)
        {
            var asteroids = await _nasaService.GetAsteroidsAsync(startDate, endDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Asteroids");

            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Id";
            worksheet.Cell(1, 3).Value = "Min Diameter (km)";
            worksheet.Cell(1, 4).Value = "Max Diameter (km)";
            worksheet.Cell(1, 5).Value = "Hazardous";
            worksheet.Cell(1, 6).Value = "Close Approach Date";
            worksheet.Cell(1, 7).Value = "Speed (km/h)";
            worksheet.Cell(1, 8).Value = "Miss Distance (km)";
            worksheet.Cell(1, 9).Value = "Orbiting Body";

            for (int i = 0; i < asteroids.Count; i++)
            {
                var a = asteroids[i];
                worksheet.Cell(i + 2, 1).Value = a.Name;
                worksheet.Cell(i + 2, 2).Value = a.Id;
                worksheet.Cell(i + 2, 3).Value = a.EstimatedDiameterMinKm;
                worksheet.Cell(i + 2, 4).Value = a.EstimatedDiameterMaxKm;
                worksheet.Cell(i + 2, 5).Value = a.IsPotentiallyHazardous ? "Yes" : "No";
                worksheet.Cell(i + 2, 6).Value = a.CloseApproachDate;
                worksheet.Cell(i + 2, 7).Value = a.RelativeVelocityKph;
                worksheet.Cell(i + 2, 8).Value = a.MissDistanceKm;
                worksheet.Cell(i + 2, 9).Value = a.OrbitingBody;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Asteroids.xlsx"
            );

        }
    }
}
