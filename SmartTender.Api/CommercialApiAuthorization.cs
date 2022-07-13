using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SmartTender.Api
{
	internal static class CommercialApiAuthorizationWrapper
	{
		static string _token;
		internal static async Task<string> GetAccessTokenAsync(CommercialApiConfigurations config, bool force = false)
		{
			if (!string.IsNullOrEmpty(_token) && !force) {
				var handler = new JwtSecurityTokenHandler();
				var jwt = handler.ReadJwtToken(_token);
				if (jwt.ValidTo.ToUniversalTime().AddMinutes(2) < DateTime.UtcNow)
				{
					return _token;
				} 
			}

			using (var client = new HttpClient())
			{
				var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync(config.Authority);
				var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
				{
					Address = discoveryDocumentResponse.TokenEndpoint,
					ClientId = config.ClientId,
					ClientSecret = config.ClientSecret,
					Scope = "smarttender.outerapi"
				});

				return (_token = tokenResponse.AccessToken);
			}
		}
	}
}