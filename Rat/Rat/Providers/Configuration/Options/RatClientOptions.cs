using System;
using System.Collections.ObjectModel;

namespace Rat.Providers.Configuration
{
    public class RatClientOptions
    {
        public bool CachingDisabled { get; set; }

        public double CacheExpiry { get; set; } = TimeSpan.FromHours(24).TotalSeconds;

        public string ApiBaseUrl { get; set; }

        public Collection<string> KeysForPreLoad { get; set; } = new Collection<string>();
    }
}