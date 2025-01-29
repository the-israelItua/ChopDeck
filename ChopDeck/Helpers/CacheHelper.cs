using Microsoft.Extensions.Caching.Memory;

namespace ChopDeck.Helpers
{
    public static class CacheHelper
    {
        public static MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
        }
    }

}
