using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchDbApi.Indexer;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Model;

namespace SearchDbApi.Indexer
{
    public partial class SearchDbIndexer: IPageIndexer
    {
        private async Task AddLinksAsync(Url urlObj, IEnumerable<string> links)
        {
            var linksList = new List<Link>();

            foreach (var link in links)
            {
                var linkObj = await AddOrFindUrlAsync(link);
                linksList.Add(new Link()
                {
                    FromUrl = urlObj,
                    ToUrl = linkObj
                });
            }

            await _context.Links.AddRangeAsync(linksList);
            await _context.SaveChangesAsync();
        }

        private async Task<Url> AddOrFindUrlAsync(string url)
        {
            var urlObj = await _context.Urls.FindAsync(url);
            if (urlObj == null)
            {
                var urlEntity = await _context.AddAsync(new Url() { Value = url });
                urlObj = urlEntity.Entity;
            }

            return urlObj;
        }
    }
}