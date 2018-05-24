using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class SearchController : ControllerBase
    {
    #region Constructor
        private WordsDbContext context;

        public SearchController(WordsDbContext context)
        {
            this.context = context;
        }
    #endregion

        // GET api/v1/search?request={value}
        [HttpGet]
        public Url Search(string request)
        {
            return new Url() { UrlId = 0, Value = "KyKy" };
        }
    }
}