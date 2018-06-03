using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class Url
    {
        [Key]
        [Column("Url")]
        [MaxLength(450)]
        public string Value { get; set; }
        public bool Indexed { get; set; }
    }
}
