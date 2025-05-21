using NASA_API.Models.Enums;
using NASA_API.Models.ViewModels;
using System.Globalization;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NASA_API.Services
{
    public class NasaService : INasaService
    {
        public readonly HttpClient _client;
        public readonly string _apiKey;

        public NasaService (HttpClient client, IConfiguration config)
        {
            _client = client;
            _apiKey = config["NASA:ApiKey"]!;
        }

        public async Task<ApodViewModel> GetApodByDateAsync(DateTime? date = null)
        {
            DateTime targetDate;

            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var easternNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone).Date;

            if (date.HasValue)
            {
                // If input date is in the future relative to Eastern Time's current date, go back one day
                targetDate = date.Value.Date;
                if (targetDate > easternNow.Date) targetDate = easternNow.Date;
            }
            else
            {
                //Assuming NASA updates their APOD at 12:30 AM
                targetDate = easternNow.Hour < 1 ? easternNow.Date.AddDays(-1) : easternNow.Date;
            }

            string dateParam = targetDate.ToString("yyyy-MM-dd");

            var url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}&date={dateParam}&thumbs=true";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apodJson = JsonDocument.Parse(json).RootElement;

            var model = new ApodViewModel
            {
                Url = apodJson.GetProperty("url").GetString(),
                Date = apodJson.GetProperty("date").GetString(),
                Title = apodJson.TryGetProperty("title", out var titleProp)
                ? titleProp.GetString()
                : "No Title",
                MediaType = apodJson.GetProperty("media_type").GetString() switch
                {
                    "image" => MediaType.Image,
                    "video" => MediaType.Video,
                    _ => MediaType.Other
                },
                Explanation = apodJson.TryGetProperty("explanation", out var explanationProp)
                ? explanationProp.GetString()
                : "No explanation provided.",
                Copyright = apodJson.TryGetProperty("copyright", out var copyrightProp)
                ? copyrightProp.GetString()
                : "Unknown"
            };

            return model;
        }

        public async Task<List<AsteroidViewModel>> GetAsteroidsAsync(DateTime startDate, DateTime endDate)
        {        
            string start = startDate.ToString("yyyy-MM-dd");
            string end = endDate.ToString("yyyy-MM-dd");

            string url = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={start}&end_date={end}&api_key={_apiKey}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode(); 

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var result = new List<AsteroidViewModel>();

            if (!root.TryGetProperty("near_earth_objects", out JsonElement neoDict))
                return result;

            foreach (var dateEntry in neoDict.EnumerateObject())
            {
                foreach (var asteroid in dateEntry.Value.EnumerateArray())
                {
                 
                        var diameter = asteroid.GetProperty("estimated_diameter").GetProperty("kilometers");
                        var approachData = asteroid.GetProperty("close_approach_data");

                        if (approachData.GetArrayLength() == 0)
                            continue; 

                        var approach = approachData[0];

                        result.Add(new AsteroidViewModel
                        {
                            Name = asteroid.GetProperty("name").GetString(),
                            Id = asteroid.GetProperty("id").ToString(),
                            EstimatedDiameterMinKm = diameter.GetProperty("estimated_diameter_min").GetDouble(),
                            EstimatedDiameterMaxKm = diameter.GetProperty("estimated_diameter_max").GetDouble(),
                            IsPotentiallyHazardous = asteroid.GetProperty("is_potentially_hazardous_asteroid").GetBoolean(),
                            CloseApproachDate = approach.GetProperty("close_approach_date").GetString(),
                            RelativeVelocityKph = double.Parse(
                                approach.GetProperty("relative_velocity").GetProperty("kilometers_per_hour").GetString()!,
                                CultureInfo.InvariantCulture),
                            MissDistanceKm = double.Parse(
                                approach.GetProperty("miss_distance").GetProperty("kilometers").GetString()!,
                                CultureInfo.InvariantCulture),
                            OrbitingBody = approach.GetProperty("orbiting_body").GetString()
                        });                  
                }
            }
            return result;
        }



    }
}
