using Rat.Data;
using System.Threading.Tasks;

namespace Rat.Api.Stores
{
    public interface IConfigurationStore
    {
        Task<ConfigurationEntry> GetEntry(string key);
    }
}