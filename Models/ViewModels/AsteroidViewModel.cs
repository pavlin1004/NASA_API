namespace NASA_API.Models.ViewModels
{
    public class AsteroidViewModel
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? CloseApproachDate { get; set; }
        public double EstimatedDiameterMinKm { get; set; }
        public double EstimatedDiameterMaxKm { get; set; }
        public double RelativeVelocityKph { get; set; }
        public double MissDistanceKm { get; set; }
        public bool IsPotentiallyHazardous { get; set; }
        public string? OrbitingBody { get; set; }
    }
}
