using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.Message
{
    public class CreateMessageViewModel
    {
        [Required]
        public string ChartId { get; set; }
        [Required]
        public string Message { get; set; }
    }
}