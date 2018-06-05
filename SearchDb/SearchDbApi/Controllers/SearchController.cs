using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;
using SearchDbApi.Search;

namespace SearchDbApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class SearchController : ControllerBase
    {
    #region Constructor
        private readonly ISearch _search;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearch search, ILogger<SearchController> logger)
        {
            _search = search;
            _logger = logger;
        }
    #endregion

        // GET api/v1/search?request={value}
        [HttpGet]
        public async Task<IDictionary<string, double>> Search(string request)
        {
            _logger.LogInformation($"GET getted: {request}");

            if (ValidRequest(request)) {
                var urls = await _search.SearchUrlsAsync(request);
                return urls;
            } else {
                return new Dictionary<string, double>();
            }
        }

        private bool ValidRequest(string request)
        {
            bool valid = request != null
                      && request.Length > 0;

            return valid;
        }
    }
}