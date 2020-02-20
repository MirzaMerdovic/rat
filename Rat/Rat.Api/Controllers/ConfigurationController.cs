using Microsoft.AspNetCore.Mvc;
using Rat.Api.Stores;
using Rat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationStore _configurationStore;
        private readonly IClientStore _clientStore;
        public ConfigurationController(IConfigurationStore configurationStore, IClientStore clientStore)
        {
            _configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        }

        [HttpGet("reload")]
        public async Task<IActionResult> Get(CancellationToken cancellation)
        {
            await _configurationStore.Load(cancellation).ConfigureAwait(false);
            await _clientStore.Notify(Enumerable.Empty<ConfigurationEntry>(), cancellation).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key, CancellationToken cancellation)
        {
            var entry = await _configurationStore.GetEntry(key, cancellation).ConfigureAwait(false);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }
    }
}