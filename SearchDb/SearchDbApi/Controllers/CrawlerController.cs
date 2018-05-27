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

namespace SearchDbApi.Controllers
{
    [Route("api/v1/[controller]")]
    public class CrawlerController : ControllerBase
    {
    #region Construstor
        private WordsDbContext _context;
        private readonly ILogger<CrawlerController> _logger;

        public CrawlerController(WordsDbContext context, ILogger<CrawlerController> logger)
        {
            this._context = context;
            this._logger = logger;
        }
    #endregion

        // POST api/v1/crawler
        [HttpPost]
        public async Task<IActionResult> IndexPage()
        {
            _logger.LogInformation($"POST getted");

            var context = this.Request.HttpContext;
            var obj = await MessagePackSerializer.DeserializeAsync<dynamic>(context.Request.Body, ContractlessStandardResolver.Instance);

            Console.WriteLine((string)obj?["Url"]);

            return this.Ok();
        }
    }
}