using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SmartTender.Api
{
	public static class CommercialApiAuthorizationWrapper
	{
		// Cached per ClientId — a single static field leaked one client's token to every other config.
		private static readonly ConcurrentDictionary<string, string> _tokens = new ConcurrentDictionary<string, string>();
		private static readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

		// Test seam only. When set, replaces the real token-endpoint round-trip.
		internal static Func<CommercialApiConfigurations, Task<string>> TokenRequestOverride;

		internal static void ResetTokenCacheForTests()
		{
			_tokens.Clear();
			TokenRequestOverride = null;
		}

		internal static async Task<string> GetAccessTokenAsync(CommercialApiConfigurations config, bool force = false)
		{
			var key = config.ClientId ?? string.Empty;

			string cached;
			if (!force && _tokens.TryGetValue(key, out cached) && IsStillUsable(cached))
				return cached;

			await _gate.WaitAsync().ConfigureAwait(false);
			try
			{
				if (!force && _tokens.TryGetValue(key, out cached) && IsStillUsable(cached))
					return cached;

				var token = await GetAccessTokenAsync(config).ConfigureAwait(false);
				if (!string.IsNullOrEmpty(token))
					_tokens[key] = token;
				return token;
			}
			finally
			{
				_gate.Release();
			}
		}

		private static bool IsStillUsable(string token)
		{
			if (string.IsNullOrEmpty(token))
				return false;
			try
			{
				var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
				// Use the cached token only while it is valid for at least two more minutes.
				return jwt.ValidTo.ToUniversalTime() - DateTime.UtcNow > TimeSpan.FromMinutes(2);
			}
			catch
			{
				return false;
			}
		}

		public static async Task<string> GetAccessTokenAsync(CommercialApiConfigurations config) {
			if (TokenRequestOverride != null)
				return await TokenRequestOverride(config).ConfigureAwait(false);

			using (var client = new HttpClient())
			{
				var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync(config.Authority).ConfigureAwait(false);
				var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
				{
					Address = discoveryDocumentResponse.TokenEndpoint,
					ClientId = config.ClientId,
					ClientSecret = config.ClientSecret,
					Scope = string.IsNullOrEmpty(config.QueryScopes) ? "smarttender.outerapi" : config.QueryScopes
				}).ConfigureAwait(false);

				return tokenResponse.AccessToken;
			}
		}
	}
}
