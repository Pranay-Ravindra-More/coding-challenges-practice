
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UrlShortner.Data;
using UrlShortner.Model;

namespace UrlShortner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly UrlContext _urlContext;
        IConfiguration _config;

        public MainController(UrlContext urlContext, IConfiguration config)
        {
            _urlContext = urlContext;
            _config = config;
        }

        [HttpGet("sample")]
        public string sampleApi()
        {
            return "Something";
        }

        [HttpPost("generateHashKey")]
        public string GenerateHashCode([FromBody] string url)
        {
            string input = Guid.NewGuid().ToString() + url;
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            string base64 = Convert.ToBase64String(hash);

            base64 = Convert.ToBase64String(hash)
                .Replace("+", "").Replace("/", "").Replace("=", "");
            return base64.Substring(0, 8);
        }

        [HttpPost("getHashUrl")]
        public async Task<ActionResult<UrlRedirectInfo>> GetHashKey([FromBody] RequestModel model)
        {
            // check if the URL already exists
            var urlRecord = await _urlContext.UrlRedirects
                .FirstOrDefaultAsync(x => x.originalUrl == model.url);

            if (urlRecord != null)
                return Ok(urlRecord); // return existing record

            // create new URL record
            string hashKey;
            do
            {
                hashKey = GenerateHashCode(model.url);
            } 
            while(await _urlContext.UrlRedirects.AnyAsync(x => x.hashKey == hashKey));

            var urlRedirectInfo = new UrlRedirectInfo
            {
                originalUrl = model.url,
                hashKey = hashKey,
                redirectUrl = _config["BaseSettings:BaseUrl"]+hashKey
            };

            await _urlContext.UrlRedirects.AddAsync(urlRedirectInfo);
            await _urlContext.SaveChangesAsync();

            return Ok(urlRedirectInfo); // return newly created record
        }


        [HttpGet("/{hashKey}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RedirectToOriginal(string hashKey)
        {
            var urlRecord = await _urlContext.UrlRedirects
                .FirstOrDefaultAsync(x => x.hashKey == hashKey);

            if (urlRecord == null)
                return NotFound();

            return Redirect(urlRecord.originalUrl);
        }

    }

}
