using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public interface IRedisCacheHelper
    {
        Task SetCacheAsync<T>(string key, T data);
        Task<T> GetCacheAsync<T>(string key);
        Task RemoveCacheAsync(string key);
    }
}
