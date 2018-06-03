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

                var links = ValidateLinksList(data);
                var wordsLocation = ValidateWordsLocationDict(data); 
                await _indexer.AddToIndexAsync(baseUri, links, wordsLocation);

                return this.Ok();
            }
            else {
                return this.NoContent();
            }
        }


        private IEnumerable<string> ValidateLinksList(dynamic data)
        {
            var recivedLinks = data?["Links"] ?? new List<string>();

            var linksList = new List<string>();
            foreach (var item in recivedLinks)
            {
                linksList.Add(item.ToString());
            }

            return linksList;
        }


        private IDictionary<string, IList<int>> ValidateWordsLocationDict(dynamic data)
        {
            var recivedWordsLocation = data?["WordLocations"]
                    ?? new Dictionary<string, IList<int>>();

            IDictionary<string, IList<int>> wordsLocation = new Dictionary<string, IList<int>>();
            foreach (var key in recivedWordsLocation.Keys) {
                List<int> locations = new List<int>();
                foreach (var value in recivedWordsLocation[key]) {
                    var strNumber = value.ToString();
                    locations.Add(int.Parse(strNumber));
                }
                wordsLocation.Add(key.ToString(), locations);
            }

            return wordsLocation;
        }
    }
}