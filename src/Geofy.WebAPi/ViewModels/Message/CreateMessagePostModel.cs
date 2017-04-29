using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.Message
{
    public class CreateMessagePostModel
    {
        [Required]
        public string ChartId { get; set; }
        [Required]
        public string Message { get; set; }
    }
}