using System;
using System.Collections.Generic;
using System.Text;

namespace WikiCrawler.WikiUriFilter
{
    class EnglishWikiFilter
    {
        private ISet<string> _recentlyVisitedSet;
        private static int _recentlyVisitedSetMaxSize = 4000;

        private static string _requaeredSubstring = "://en.wikipedia.org";

        public EnglishWikiFilter()
        {
            _recentlyVisitedSet = new HashSet<string>();
        }

        public bool AcceptUri(string uri)
        {
            if (uri.Contains(_requaeredSubstring)) 
            {
                if (!_recentlyVisitedSet.Contains(uri)) {
                    MarkAsVisited(uri);
                    return true;
                }
                else {
                    return false;
                }
            } 
            else {
                return false;
            }
        }

        private void MarkAsVisited(string uri)
        {
            if (_recentlyVisitedSet.Count > _recentlyVisitedSetMaxSize) {
                _recentlyVisitedSet.Clear();
            }

            _recentlyVisitedSet.Add(uri);
        }
    }
}
