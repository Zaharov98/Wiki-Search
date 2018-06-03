using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class Link
    {
        [Key]
        public int LinkId { get; set; }
        
        [ForeignKey("FromUrl")]
        [Column(Order = 1)]
        public string FromUrlId { get; set; }
        public Url FromUrl { get; set; }

        [ForeignKey("ToUrl")]
        [Column(Order = 2)]
        public string ToUrlId { get; set; }
        public Url ToUrl { get; set; }
    }
}
