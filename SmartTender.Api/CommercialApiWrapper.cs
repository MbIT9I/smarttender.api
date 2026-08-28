using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CommercialServices.DTO.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SmartTender.Api
{
	public class CommercialApiWrapper
	{
		private const string PublicApiOrigin = "https://api.smarttender.biz/";
		private const string PublicApiOriginTest = "https://api-test.smarttender.biz/";

		// A single shared HttpClient for the whole process. It carries no per-call state
		// (no BaseAddress, no default headers) — everything is set on each HttpRequestMessage.
		private static readonly HttpClient _sharedHttpClient = new HttpClient();

		public string Origin
		{
			get
			{
				if (string.IsNullOrEmpty(_commercialApiConfigurations.OverrideOrigin)) {
					return _commercialApiConfigurations.IsProductionMode ? PublicApiOrigin : PublicApiOriginTest;
				}
				return _commercialApiConfigurations.OverrideOrigin;
			}
		}

		public HttpClient HttpClient
		{
			get { return HttpClientOverride ?? _sharedHttpClient; }
		}

		// Test seam only. When null the process-wide shared client is used.
		internal HttpClient HttpClientOverride { get; set; }

		// Test seam only. When null the bearer token is taken from CommercialApiAuthorizationWrapper.
		internal Func<CommercialApiConfigurations, bool, Task<string>> AccessTokenProvider { get; set; }

		private Task<string> ResolveAccessTokenAsync(bool force)
		{
			var provider = AccessTokenProvider ?? CommercialApiAuthorizationWrapper.GetAccessTokenAsync;
			return provider(_commercialApiConfigurations, force);
		}

		public ITLogger Logger
		{
			get
			{
				return _commercialApiConfigurations.Logger;
			}
		}
		private CommercialApiConfigurations _commercialApiConfigurations { get; }

		private CommercialApiWrapper(CommercialApiConfigurations config)
		{
			_commercialApiConfigurations = config;
		}

		public static CommercialApiWrapper GetApiWrapper(CommercialApiConfigurations config)
		{
			// A fresh wrapper per configuration. The previous process-wide singleton froze
			// the first config forever and shared a mutable retry counter across all calls.
			return new CommercialApiWrapper(config);
		}

		public Task<HttpResponseMessage> CallWebRequestAsync(string method, string endpoint, object dto = null, params object[] query)
		{
			var uri = query.Any() ? string.Format(endpoint, query) : endpoint;
			var body = dto == null ? null : JsonConvert.SerializeObject(dto);

			Logger?.Debug(
				new
				{
					Method = method,
					Uri = uri,
					Content = body,
					Query = query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);

			return SendWithRetryAsync(() =>
			{
				var message = new HttpRequestMessage(new HttpMethod(method), uri);
				if (body != null)
					message.Content = new StringContent(body, Encoding.UTF8, "application/json");
				return message;
			});
		}

		internal Task<HttpResponseMessage> CallWebRequestAsync(ApiEndpoint endpoint, object dto = null, params object[] query)
		{
			var endpointDesc = endpoint.GetEndpointDescription();
			var uri = query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint;
			var body = dto == null ? null : JsonConvert.SerializeObject(dto);

			Logger?.Debug(
				new
				{
					Method = endpointDesc.Method.Method,
					Uri = uri,
					Content = body,
					Query = query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);

			return SendWithRetryAsync(() =>
			{
				var message = new HttpRequestMessage(endpointDesc.Method, uri);
				if (body != null)
					message.Content = new StringContent(body, Encoding.UTF8, "application/json");
				return message;
			});
		}

		internal async Task<HttpResponseMessage> CallFilesWebRequestAsync(ApiEndpoint endpoint, IFormFile[] files, params object[] query)
		{
			var endpointDesc = endpoint.GetEndpointDescription();
			var uri = query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint;

			// Buffer each file once. The request (and its multipart content) is rebuilt on every
			// send attempt, so we cannot rely on a one-shot Stream from OpenReadStream().
			var buffered = new List<BufferedFile>();
			if (files != null)
			{
				foreach (var file in files)
				{
					if (file == null)
						continue;
					using (var memory = new MemoryStream())
					using (var source = file.OpenReadStream())
					{
						await source.CopyToAsync(memory).ConfigureAwait(false);
						buffered.Add(new BufferedFile { Content = memory.ToArray(), Name = "file", FileName = file.FileName });
					}
				}
			}

			Logger?.Debug(
				new
				{
					Method = endpointDesc.Method.Method,
					Uri = uri,
					Files = buffered.Count,
					Query = query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);

			return await SendWithRetryAsync(() =>
			{
				var message = new HttpRequestMessage(endpointDesc.Method, uri);
				if (buffered.Count > 0)
				{
					var payload = new MultipartFormDataContent();
					foreach (var file in buffered)
						payload.Add(new ByteArrayContent(file.Content), file.Name, file.FileName);
					message.Content = payload;
				}
				return message;
			}).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends the request built by <paramref name="requestFactory"/>. On a 401 it retries
		/// exactly once with a forced token refresh. A brand new HttpRequestMessage is built for
		/// every attempt — an HttpRequestMessage can only be sent once.
		/// </summary>
		private async Task<HttpResponseMessage> SendWithRetryAsync(Func<HttpRequestMessage> requestFactory)
		{
			HttpResponseMessage responce = null;

			for (var attempt = 0; attempt < 2; attempt++)
			{
				var request = requestFactory();

				if (request.RequestUri == null || !request.RequestUri.IsAbsoluteUri)
				{
					var relative = request.RequestUri == null ? string.Empty : request.RequestUri.OriginalString;
					request.RequestUri = new Uri(new Uri(Origin), relative);
				}

				ApplyDefaultHeaders(request);

				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
					await ResolveAccessTokenAsync(force: attempt > 0).ConfigureAwait(false));

				responce = await HttpClient.SendAsync(request).ConfigureAwait(false);

				if (responce.StatusCode != HttpStatusCode.Unauthorized)
					break;

				if (attempt == 0)
					responce.Dispose();
			}

			Logger?.Debug(
				new
				{
					IsSuccessStatusCode = responce.IsSuccessStatusCode,
					StatusCode = responce.StatusCode,
					ReasonPhrase = responce.ReasonPhrase,
					Headers = responce.Headers,
					RequestMessage = responce.RequestMessage
				});

			return responce;
		}

		private void ApplyDefaultHeaders(HttpRequestMessage request)
		{
			if (_commercialApiConfigurations.OverrideOrganizationCode.HasValue)
				request.Headers.TryAddWithoutValidation("OrganizationItId",
					$"{_commercialApiConfigurations.OverrideOrganizationCode.Value}");

			request.Headers.TryAddWithoutValidation("culture",
				(_commercialApiConfigurations.Culture.HasValue ? _commercialApiConfigurations.Culture.Value : Cultures.Uk).ToString().ToLower());
		}

		private class BufferedFile
		{
			public byte[] Content { get; set; }
			public string Name { get; set; }
			public string FileName { get; set; }
		}

		internal bool checkResponceStatuses<T>(T request, HttpResponseMessage responce)
		{
			if (responce == null)
			{
				Logger?.LogError(new { request }, "Network Error!");
				return false;
			}
			if (!responce.IsSuccessStatusCode)
			{
				string result;
				using (var dataStream = responce.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
				{
					using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
					{
						result = reader.ReadToEnd();
					}
				}
				if (responce.StatusCode == HttpStatusCode.BadRequest)
				{
					Logger?.LogError(new { request, responce }, $"Error occurred by executing query!\r\n{result}");
					return false;
				}
				if (responce.StatusCode == HttpStatusCode.Forbidden)
				{
					Logger?.LogError(new { request, responce }, $"You don't have permissions to call this method or to this entry!\r\n{result}");
					return false;
				}
				if (new[] { HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError }.Contains(responce.StatusCode))
				{
					Logger?.LogError(new { request, responce }, $"At this time service unavailable. Try later!\r\n{result}");
					return false;
				}
				Logger?.LogError(new { request, responce }, $"Something went wrong!\r\n{result}");
				return false;
			}
			return true;
		}

		internal DTO convertResponceToDto<DTO>(HttpResponseMessage responce, params JsonConverter[] additionalConvertors)
		{
			using (var dataStream = responce.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
			{
				using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
				{
					var result = reader.ReadToEnd();
					return JsonConvert.DeserializeObject<DTO>(result, additionalConvertors);
				}
			}
		}

		internal DTO convertResultResponceToDto<DTO>(HttpResponseMessage responce, params JsonConverter[] additionalConvertors) where DTO : class, new()
		{
			using (var dataStream = responce.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
			{
				using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
				{
					var result = reader.ReadToEnd();
					Logger.Debug(new { result, responce.StatusCode, responce.ReasonPhrase });
					return JsonConvert.DeserializeObject<WrapperExecutionResultBase<DTO>>(result, additionalConvertors).Result;
				}
			}
		}

		internal DTO convertWrappedResponceToDto<DTO>(HttpResponseMessage responce, params JsonConverter[] additionalConvertors) {
			using (var dataStream = responce.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
			{
				using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
				{
					var result = reader.ReadToEnd();
					return JsonConvert.DeserializeObject<DTO>($@"{{ ""result"": {result} }}", additionalConvertors);
				}
			}
		}

		internal class Wrapper<T> {
			[JsonProperty("result")]
			T Data { get; set; }
		}

		public class WrapperExecutionResultBase<TDto>
		{
			[JsonProperty("result")]
			public TDto Result { get; set; }
			[JsonProperty("success")]
			public bool Success { get; private set; }
			[JsonProperty("message")]
			public string Message { get; set; }
			[JsonProperty("errorCode")]
			public string ErrorCode { get; private set; }
		}
	}
}
