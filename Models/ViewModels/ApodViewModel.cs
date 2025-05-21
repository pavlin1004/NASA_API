using NASA_API.Models.Enums;

namespace NASA_API.Models.ViewModels
{
    public class ApodViewModel
    {
        public string? Url { get; set; }

        public string? Title { get; set; }

        public string? Explanation { get; set; }
        public string? Date { get; set; }
        public string? Copyright { get; set; }
        public MediaType? MediaType { get; set; }



    }
}
