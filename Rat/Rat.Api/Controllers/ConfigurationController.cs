using Microsoft.AspNetCore.Mvc;
using Rat.Api.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationStore _store;

        public ConfigurationController(IConfigurationStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key, CancellationToken cancellation)
        {
            var entry = await _store.GetEntry(key, cancellation).ConfigureAwait(false);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }
    }
}