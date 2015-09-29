using SDC.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.ServiceLayer
{
    class SearchCache
    {
        private Dictionary<int, SearchResultDTO> _cache;

        public SearchCache()
        {
            _cache = new Dictionary<int, SearchResultDTO>();
        }

        public void Add(int id, SearchResultDTO result)
        {
            _cache.Add(id, result);
        }

        public SearchResultDTO Get(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            else
                return null;
        }

        public void Clear(int id)
        {
            if (_cache.ContainsKey(id))
            {
                _cache.Remove(id);
            }
        }
    }
}
