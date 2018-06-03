using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Data.Context
{
    public class WordsDbContext : DbContext, IDisposable
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<Url> Urls { get; set; }
        public DbSet<WordLocation> WordLocations { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<UrlWord> UrlWords { get; set; }
        public DbSet<PageRank> PageRanks { get; set; }


        public WordsDbContext(DbContextOptions<WordsDbContext> options)
             : base(options)
        {
            Database.EnsureCreated();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
