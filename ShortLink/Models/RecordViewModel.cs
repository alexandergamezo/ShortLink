using System.ComponentModel.DataAnnotations;

namespace ShortLink.Models
{
    public class RecordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "LongUrl is required.")]
        [Url(ErrorMessage = "LongUrl must be a valid URL.")]
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime DateTime { get; set; }
        public int ClickCount { get; set; }
    }
}
