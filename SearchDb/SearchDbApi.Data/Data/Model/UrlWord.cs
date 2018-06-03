using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class UrlWord
    {
        public int UrlWordId { get; set; }
        
        [ForeignKey("Word")]
        [Column("WordId")]
        public string WordId { get; set; }
        public Word Word { get; set; }

        [ForeignKey("Url")]
        [Column("UrlId")]
        public string UrlId { get; set; }
        public Url Url { get; set; }
    }
}
