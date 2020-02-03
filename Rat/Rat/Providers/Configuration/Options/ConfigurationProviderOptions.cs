using System;
using System.Collections.ObjectModel;

namespace Rat.Providers.Configuration
{
    public class ConfigurationProviderOptions
    {
        public bool CachingDisabled { get; set; }

        public TimeSpan CacheExpiry { get; set; }

        public string ConfigurationApiBaseUrl { get; set; }

        public Collection<string> KeysForPreLoad { get; set; } = new Collection<string>();
    }
}