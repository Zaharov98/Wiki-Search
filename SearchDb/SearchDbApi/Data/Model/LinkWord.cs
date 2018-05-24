using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class LinkWord
    {
        public int LinkWordId { get; set; }

        [ForeignKey("Link")]
        public int? LinkId { get; set; }
        public Link Link { get; set; }

        [ForeignKey("Word")]
        public int? WordId { get; set; }
        public Word Word { get; set; }
    }
}
