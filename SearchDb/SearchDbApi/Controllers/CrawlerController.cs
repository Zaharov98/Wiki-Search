using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessagePack;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Controllers
{
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
        public async Task<IActionResult> IndexPage()
        {
            var request = this.Request;

            int bufferSize = 8192;
            var buffer = new byte[bufferSize];
            await request.Body.ReadAsync(buffer, 0, bufferSize);

            var json = MessagePackSerializer.ToJson(buffer);
            Console.WriteLine(json);

            return this.Ok();
        }
    }
}