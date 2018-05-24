using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class WordLocation
    {
        public int WordLocationId { get; set; }

        public int WordId { get; set; }
        public Word Word { get; set; }

        public int UrlId { get; set; }
        public Url Url { get; set; }

        public int Location { get; set; }
    }
}
