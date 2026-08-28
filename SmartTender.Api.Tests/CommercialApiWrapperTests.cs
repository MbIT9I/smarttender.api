using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CommercialServices.DTO.Tender;
using Xunit;

namespace SmartTender.Api.Tests
{
	public class CommercialApiWrapperTests
	{
		private static CommercialApiConfigurations TestConfig(
			int? organization = 212834,
			Cultures? culture = Cultures.En,
			bool isProduction = false,
			CapturingLogger logger = null)
		{
			return new CommercialApiConfigurations("client", "secret", "smarttender.outerapi", isProduction)
			{
				OverrideOrganizationCode = organization,
				Culture = culture,
				Logger = logger
			};
		}

		private static CommercialApiMethods Wire(
			RecordingHttpMessageHandler handler,
			CommercialApiConfigurations config,
			IList<bool> forceLog = null)
		{
			var methods = new CommercialApiMethods(config);
			methods.CommercialApi.HttpClientOverride = new HttpClient(handler);
			methods.CommercialApi.AccessTokenProvider = (cfg, force) =>
			{
				forceLog?.Add(force);
				return Task.FromResult(force ? "token-refreshed" : "token-cached");
			};
			return methods;
		}

		[Fact]
		public async Task Retry_on_401_sends_a_brand_new_request_with_a_refreshed_token_then_succeeds()
		{
			var handler = new RecordingHttpMessageHandler(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
			var forceLog = new List<bool>();
			var methods = Wire(handler, TestConfig(), forceLog);

			var result = await methods.GetTenderAsync(777);

			Assert.NotNull(result);
			Assert.Equal(2, handler.Requests.Count);
			// The core regression: an HttpRequestMessage can be sent only once. If the wrapper
			// re-sent the same instance, HttpClient.SendAsync would have thrown before we got here.
			Assert.NotSame(handler.RawRequests[0], handler.RawRequests[1]);
			Assert.Equal("Bearer token-cached", handler.Requests[0].Authorization);
			Assert.Equal("Bearer token-refreshed", handler.Requests[1].Authorization);
			Assert.Equal(new[] { false, true }, forceLog);
		}

		[Fact]
		public async Task Successful_request_is_sent_exactly_once_without_a_token_refresh()
		{
			var handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
			var forceLog = new List<bool>();
			var methods = Wire(handler, TestConfig(), forceLog);

			await methods.GetTenderAsync(1);

			Assert.Single(handler.Requests);
			Assert.Equal(new[] { false }, forceLog);
		}

		[Fact]
		public async Task Persistent_401_stops_after_one_retry_and_surfaces_the_response_body()
		{
			var logger = new CapturingLogger();
			var handler = new RecordingHttpMessageHandler(
				RecordingHttpMessageHandler.Respond(HttpStatusCode.Unauthorized, "\"denied\""),
				RecordingHttpMessageHandler.Respond(HttpStatusCode.Unauthorized, "\"denied\""));
			var methods = Wire(handler, TestConfig(logger: logger));

			var result = await methods.GetTenderAsync(1);

			Assert.Null(result);
			Assert.Equal(2, handler.Requests.Count); // bounded: original + exactly one retry
			// Old code returned null from the wrapper on exhaustion, so checkResponceStatuses logged
			// "Network Error!" and lost the body. Now the real 401 response reaches the logger.
			Assert.Contains(logger.Errors, e => e.Contains("denied"));
		}

		[Fact]
		public async Task Request_body_is_rebuilt_for_every_attempt()
		{
			var handler = new RecordingHttpMessageHandler(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
			var methods = Wire(handler, TestConfig());

			await methods.PostTenderAsync(new TenderCreateDto<LotCreateDto>());

			Assert.Equal(2, handler.Requests.Count);
			Assert.False(string.IsNullOrEmpty(handler.Requests[0].Body));
			// Same serialized payload, but a fresh StringContent each time (content is also single-use).
			Assert.Equal(handler.Requests[0].Body, handler.Requests[1].Body);
		}

		[Fact]
		public async Task File_upload_is_retried_with_a_rebuilt_multipart_body()
		{
			// Second attempt returns a non-success status so the assertion stays on the transport
			// layer and does not depend on how the document response body is deserialized.
			var handler = new RecordingHttpMessageHandler(
				RecordingHttpMessageHandler.Respond(HttpStatusCode.Unauthorized),
				RecordingHttpMessageHandler.Respond(HttpStatusCode.InternalServerError, "\"boom\""));
			var methods = Wire(handler, TestConfig());
			var bytes = System.Text.Encoding.UTF8.GetBytes("hello-file-contents");

			var ids = await methods.PostTenderDocumentAsync(42, new FakeFormFile(bytes, "doc.pdf"));

			Assert.Null(ids);
			Assert.Equal(2, handler.Requests.Count);
			// A MultipartFormDataContent (and its file part) is single-use; retrying required
			// rebuilding it from the buffered bytes rather than replaying the sent instance.
			Assert.All(handler.Requests, r => Assert.True(r.IsMultipart));
			Assert.All(handler.Requests, r => Assert.Contains("doc.pdf", r.Body));
			Assert.All(handler.Requests, r => Assert.Contains("hello-file-contents", r.Body));
		}

		[Fact]
		public async Task Outgoing_request_uses_an_absolute_uri_and_per_request_headers()
		{
			var handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
			var methods = Wire(handler, TestConfig(organization: 555, culture: Cultures.Uk));

			await methods.GetTenderAsync(123);

			var request = handler.Requests.Single();
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal("https://api-test.smarttender.biz/commercial/tender/123", request.RequestUri.ToString());
			Assert.Equal("555", request.OrganizationItId);
			Assert.Equal("uk", request.Culture);
		}

		[Fact]
		public async Task Each_configuration_gets_its_own_headers_the_first_one_no_longer_wins()
		{
			var handlerA = new RecordingHttpMessageHandler(HttpStatusCode.OK);
			await Wire(handlerA, TestConfig(organization: 111, culture: Cultures.Uk)).GetTenderAsync(1);

			var handlerB = new RecordingHttpMessageHandler(HttpStatusCode.OK);
			await Wire(handlerB, TestConfig(organization: 222, culture: Cultures.En)).GetTenderAsync(2);

			Assert.Equal("111", handlerA.Requests[0].OrganizationItId);
			Assert.Equal("uk", handlerA.Requests[0].Culture);
			Assert.Equal("222", handlerB.Requests[0].OrganizationItId);
			Assert.Equal("en", handlerB.Requests[0].Culture);
		}

		[Fact]
		public void GetApiWrapper_returns_a_fresh_instance_per_call()
		{
			var config = new CommercialApiConfigurations("c", "s");

			var first = CommercialApiWrapper.GetApiWrapper(config);
			var second = CommercialApiWrapper.GetApiWrapper(config);

			Assert.NotSame(first, second);
		}

		[Theory]
		[InlineData(true, null, "https://api.smarttender.biz/")]
		[InlineData(false, null, "https://api-test.smarttender.biz/")]
		[InlineData(true, "https://custom.example/", "https://custom.example/")]
		public void Origin_is_resolved_from_configuration(bool isProduction, string overrideOrigin, string expected)
		{
			var config = new CommercialApiConfigurations("c", "s", isProduction: isProduction)
			{
				OverrideOrigin = overrideOrigin
			};

			Assert.Equal(expected, CommercialApiWrapper.GetApiWrapper(config).Origin);
		}
	}
}
