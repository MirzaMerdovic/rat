using System.Collections.ObjectModel;

namespace Rat.Providers.Retry
{
    public class RetryProviderOptions
    {
        public Collection<int> Delays { get; set; } = new Collection<int>();
    }
}