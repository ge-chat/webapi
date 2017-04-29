using System;

namespace Geofy.ReadModels
{
    public class ShortMessage
    {
        public string MessageId { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Content { get; set; }
    }
}