using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class PageRank
    {
        [Key]
        [ForeignKey("Url")]
        public string UrlId { get; set; }
        public Url Url { get; set; }

        public int Score { get; set; }
    }
}
