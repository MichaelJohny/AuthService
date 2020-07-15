using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthService
{
    [Route("/api/tests")]
    public class TestsController : ControllerBase
    {
        private string Uri => Request.Scheme + "://" + Request.Host;

        public async Task<IActionResult> Get()
        {
            var token = await RequestClientCredentialsTokenAsync();

            var http = new HttpClient();

            http.SetBearerToken(token.AccessToken);

            var response = await http.GetStringAsync($"{Uri}/api/tests/protected");

            return Ok(response);
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [Route(nameof(Protected))]
        public IActionResult Protected()
        {
            return Ok(nameof(Ok));
        }

        private async Task<TokenResponse> RequestClientCredentialsTokenAsync()
        {
            using var http = new HttpClient();

            var disco = await http.GetDiscoveryDocumentAsync(Uri);

            var request = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ClientCredentials",
                ClientSecret = "Secret",
                Scope = IdentityServerConstants.LocalApi.ScopeName
            };

            return await http.RequestClientCredentialsTokenAsync(request);
        }
    }
}
