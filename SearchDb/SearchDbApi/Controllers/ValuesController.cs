using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Controllers
{
    /// <summary>
    /// For testing connection to Database
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ValuesController : ControllerBase
    {
    #region Constructor
        private WordsDbContext context;

        public ValuesController(WordsDbContext context)
        {
            this.context = context;
        }
    #endregion

        // GET api/v1/values
        [HttpGet]
        public IEnumerable<Word> Get()
        {
            IEnumerable<Word> words = context.Words.AsEnumerable();

            return words;
        }

        // GET api/v1/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            Word word = context.Words.FirstOrDefault(w => w.Value == id);

            return $"{word.Value}:{id}";
        }

        // POST api/v1/values
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return this.Ok();
        }

        // PUT api/v1/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return this.Ok();
        }

        // DELETE api/v1/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return this.Ok();
        }
    }
}