using NASA_API.Models.ViewModels;

namespace NASA_API.Services
{
    public interface INasaService
    {
        public Task<ApodViewModel> GetApodByDateAsync(DateTime? date);
        public Task<List<AsteroidViewModel>> GetAsteroidsAsync(DateTime startDate, DateTime endDate);


    }
}
