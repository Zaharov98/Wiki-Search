using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SearchDbApi.Data.Model
{
    public class Word
    {
        [Key]
        [Column("Word")]
        [MaxLength(400)]
        public string Value { get; set; }
    }
}
