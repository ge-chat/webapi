using System.ComponentModel.DataAnnotations;

namespace Geofy.WebAPi.ViewModels.Chart
{

    public class ParticipantChangeNameModel
    {
        [Required]
        public string ChatId { get; set; }
        [Required]
        public string Name { get; set; }    
    }
}