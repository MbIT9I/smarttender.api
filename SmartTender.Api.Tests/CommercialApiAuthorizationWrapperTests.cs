using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Xunit;

namespace SmartTender.Api.Tests
{
	public class CommercialApiAuthorizationWrapperTests : IDisposable
	{
		public CommercialApiAuthorizationWrapperTests()
		{
			CommercialApiAuthorizationWrapper.ResetTokenCacheForTests();
		}

		public void Dispose()
		{
			CommercialApiAuthorizationWrapper.ResetTokenCacheForTests();
		}

		private static CommercialApiConfigurations Config(string clientId)
		{
			return new CommercialApiConfigurations(clientId, "secret", "smarttender.outerapi", isProduction: false);
		}

		// The caching entry point is the (config, force) overload; the single-arg public
		// overload always goes straight to the token endpoint.
		private static Task<string> Acquire(string clientId, bool force = false)
		{
			return CommercialApiAuthorizationWrapper.GetAccessTokenAsync(Config(clientId), force);
		}

		private static string Jwt(TimeSpan validFor)
		{
			var token = new JwtSecurityToken(expires: DateTime.UtcNow.Add(validFor));
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[Fact]
		public async Task A_still_valid_cached_token_is_reused_without_hitting_the_token_endpoint()
		{
			var calls = 0;
			CommercialApiAuthorizationWrapper.TokenRequestOverride = _ =>
			{
				calls++;
				return Task.FromResult(Jwt(TimeSpan.FromMinutes(30)));
			};

			var first = await Acquire("client-A");
			var second = await Acquire("client-A");

			Assert.Equal(1, calls); // the pre-fix expiry check was inverted and refetched every time
			Assert.Equal(first, second);
		}

		[Fact]
		public async Task Force_bypasses_the_cache()
		{
			var calls = 0;
			CommercialApiAuthorizationWrapper.TokenRequestOverride = _ =>
			{
				calls++;
				return Task.FromResult(Jwt(TimeSpan.FromMinutes(30)));
			};

			await Acquire("client-A");
			await Acquire("client-A", force: true);

			Assert.Equal(2, calls);
		}

		[Fact]
		public async Task A_token_within_two_minutes_of_expiry_is_refetched()
		{
			var calls = 0;
			CommercialApiAuthorizationWrapper.TokenRequestOverride = _ =>
			{
				calls++;
				return Task.FromResult(Jwt(TimeSpan.FromSeconds(30)));
			};

			await Acquire("client-A");
			await Acquire("client-A");

			Assert.Equal(2, calls);
		}

		[Fact]
		public async Task Tokens_are_cached_per_client_id()
		{
			var seenClients = new List<string>();
			CommercialApiAuthorizationWrapper.TokenRequestOverride = config =>
			{
				seenClients.Add(config.ClientId);
				return Task.FromResult(Jwt(TimeSpan.FromMinutes(30)));
			};

			await Acquire("client-A");
			await Acquire("client-B");
			await Acquire("client-A"); // served from cache

			Assert.Equal(new[] { "client-A", "client-B" }, seenClients);
		}

		[Fact]
		public async Task An_unparseable_cached_token_is_discarded_and_refetched()
		{
			var responses = new Queue<string>(new[] { "not-a-jwt", Jwt(TimeSpan.FromMinutes(30)) });
			var calls = 0;
			CommercialApiAuthorizationWrapper.TokenRequestOverride = _ =>
			{
				calls++;
				return Task.FromResult(responses.Dequeue());
			};

			var first = await Acquire("client-A");
			var second = await Acquire("client-A");

			Assert.Equal(2, calls);
			Assert.NotEqual(first, second);
		}
	}
}
