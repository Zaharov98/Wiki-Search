using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class CrawlerController : ControllerBase
    {
    #region Construstor
        private WordsDbContext context;

        public CrawlerController(WordsDbContext context)
        {
            this.context = context;
        }
    #endregion

        // POST api/v1/crawler
        [HttpPost]
        public IActionResult IndexPage()
        {
            return this.Ok();
        }
    }
}