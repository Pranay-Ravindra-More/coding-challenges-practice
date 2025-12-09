using System.ComponentModel.DataAnnotations;

namespace UrlShortner.Model
{
    public class UrlRedirectInfo
    {
        [Key]
        public int id { get; set; }
        public string hashKey { get; set; }
        public string originalUrl { get; set; }
        public string redirectUrl { get; set; }
    }
}
