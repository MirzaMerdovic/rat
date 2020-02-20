using Microsoft.AspNetCore.Mvc;
using Rat.Api.Stores;
using Rat.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IClientStore _clientRegistration;

        public ClientController(IClientStore clientStore)
        {
            _clientRegistration = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] ClientRegistrationEntry entry, CancellationToken cancellation)
        {
            await _clientRegistration.Register(entry, cancellation).ConfigureAwait(false);

            // TODO: Return 201
            return Ok();
        }
    }
}