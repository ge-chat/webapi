using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.Chart
{
    public class CreateChartViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Radius { get; set; }

        public string Description { get; set; }
    }
}