using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Services.Abstract;
using System.Collections.Generic;

namespace ShortUrl.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private IShortUrlService shortUrlService;
        private readonly ILogger<ShortUrlController> logger;
        public ShortUrlController(IShortUrlService shortUrlService, ILogger<ShortUrlController> logger)
        {
            this.shortUrlService = shortUrlService;
            this.logger = logger;
        }

        // GET: api/Default
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<ShortURLModel> shortUrls = shortUrlService.GetCollectionFromDataStore();
            return Ok(shortUrls);
        }

        [HttpGet("{shorturl}", Name = "Get")]
        public IActionResult Get(string shorturl, [FromQuery(Name = "redirect")] bool redirect = true)
        {
            ShortURLModel shortUrl = shortUrlService.GetItemFromDataStore(shorturl);

            if (shortUrl != null)
            {
                if(redirect)
                {
                    return Redirect(shortUrl.LongURL);
                }
                else
                {
                    return Ok(shortUrl);
                }
            }

            return NotFound();
        }

        // POST: api/Default
        [HttpPost]
        public IActionResult Post([FromBody] ShortURLRequestModel model)
        {
            if(ModelState.IsValid)
            { 
                ShortUrlResponseModel result = shortUrlService.SaveItemToDataStore(model);
                if (result!=null)
                    return Ok(result);
            }

            return BadRequest(ModelState.Values);
        }
    }
}
