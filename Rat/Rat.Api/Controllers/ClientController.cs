using Microsoft.AspNetCore.Mvc;
using Rat.Data;
using Rat.Providers.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IClientRegistrationProvider _clientRegistration;

        public ClientController(IClientRegistrationProvider clientRegistration)
        {
            _clientRegistration = clientRegistration ?? throw new ArgumentNullException(nameof(clientRegistration));
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] ClientRegistrationEntry entry, CancellationToken cancellation)
        {
            await _clientRegistration.Register(entry, cancellation).ConfigureAwait(false);

            return Ok();
        }
    }
}