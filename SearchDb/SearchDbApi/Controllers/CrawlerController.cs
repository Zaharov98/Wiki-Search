using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MessagePack;
using MessagePack.Resolvers;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;
using SearchDbApi.Indexer;

namespace SearchDbApi.Controllers
{
    [Route("api/v1/[controller]")]
    public class CrawlerController : ControllerBase
    {
        #region Construstor
        private readonly IPageIndexer _indexer;
        private readonly ILogger<CrawlerController> _logger;

        public CrawlerController(IPageIndexer indexer, ILogger<CrawlerController> logger)
        {
            this._indexer = indexer;
            this._logger = logger;
        }
    #endregion

        // POST api/v1/crawler
        [HttpPost]
        public async Task<IActionResult> IndexPage()
        {
            _logger.LogInformation($"POST getted");

            var context = this.Request.HttpContext;
            var data = await MessagePackSerializer.DeserializeAsync<dynamic>(context.Request.Body, ContractlessStandardResolver.Instance);

            var baseUri = data?["Url"] as string ?? String.Empty;
            if (baseUri != String.Empty) {
                var linksList = data?["Links"] ?? new List<string>();
                
                var wordsLocation = data?["WordLocations"]
                    ?? new Dictionary<string, IList<int>>();

                var links = new List<string>();
                foreach (var item in linksList) {
                    links.Add(item.ToString());
                }

                IDictionary<string, IList<int>> wl = new Dictionary<string, IList<int>>();
                foreach (var key in wordsLocation.Keys) {
                    List<int> locations = new List<int>();
                    foreach (var value in wordsLocation[key]) {
                        var strNumber = value.ToString();
                        locations.Add(int.Parse(strNumber));
                    }
                    wl.Add(key.ToString(), locations);
                }

                await _indexer.AddToIndexAsync(baseUri, links, wl);
            }
            

            return this.Ok();
        }
    }
}