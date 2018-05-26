using System;
using System.Collections.Generic;
using MessagePack;

namespace Indexer.HtmlParse
{
    [MessagePackObject]
    public class HtmlPageItems
    {
        [Key("Url")]
        public string PageUrl { get; set; }

        [Key("Links")]
        public IList<string> Links { get; set; }

        [Key("WordLocations")]
        public IDictionary<string, List<int>> WordLocations { get; set; }
    }
}