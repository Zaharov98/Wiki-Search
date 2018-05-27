using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class SearchController : ControllerBase
    {
    #region Constructor
        private WordsDbContext _context;
        private readonly ILogger<SearchController> _logger;

        public SearchController(WordsDbContext context, ILogger<SearchController> logger)
        {
            _context = context;
            _logger = logger;
        }
    #endregion

        // GET api/v1/search?request={value}
        [HttpGet]
        public Url Search(string request)
        {
            _logger.LogInformation($"GET getted: {request}");

            return new Url() { UrlId = 0, Value = request };
        }
    }
}