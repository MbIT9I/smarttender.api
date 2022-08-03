using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SmartTender.Api
{
	public class CommercialApiWrapper
	{
		private static CommercialApiWrapper _apiWrapperInstance;
		private int _count = 0;
		private const string PublicApiOrigin = "https://api.smarttender.biz/";
		private const string PublicApiOriginTest = "https://api-test.smarttender.biz/";

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
		private HttpClient _httpClient;
		public HttpClient HttpClient
		{
			get
			{
				if (_httpClient == null)
				{
					_httpClient = new HttpClient()
					{
						BaseAddress = new Uri(Origin),
					};
					if (_commercialApiConfigurations.OverrideOrganizationCode.HasValue)
					{
						_httpClient.DefaultRequestHeaders.Add("OrganizationItId", $"{_commercialApiConfigurations.OverrideOrganizationCode.Value}");
					}
					_httpClient.DefaultRequestHeaders.Add("culture",
						(_commercialApiConfigurations.Culture.HasValue ? _commercialApiConfigurations.Culture.Value : Cultures.Uk).ToString().ToLower());
				}
				return _httpClient;
			}
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
			return _apiWrapperInstance ?? (_apiWrapperInstance = new CommercialApiWrapper(config));
		}


		public async Task<HttpResponseMessage> CallWebRequestAsync(string method, string endpoint, object dto = null, params object[] query) {
			var httpMessage = new HttpRequestMessage(new HttpMethod(method), query.Any() ? string.Format(endpoint, query) : endpoint);
			if (dto != null)
				httpMessage.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
			Logger?.Debug(
				new
				{
					Method = new HttpMethod(method),
					Uri = (query.Any() ? string.Format(endpoint, query) : endpoint),
					httpMessage.Content,
					query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);
			HttpResponseMessage responce;
			do
			{
				_count++;
				if (_count > 2)
				{
					_count = 0;
					return null;
				}
				httpMessage.Headers.Authorization =
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
						await CommercialApiAuthorizationWrapper.GetAccessTokenAsync(_commercialApiConfigurations, _count <= 1));
				responce = await HttpClient.SendAsync(httpMessage);
			} while (responce.StatusCode == HttpStatusCode.Unauthorized);
			_count = 0;
			return responce;
		}

		internal async Task<HttpResponseMessage> CallWebRequestAsync(HttpRequestMessage httpMessage) {
			HttpResponseMessage responce;
			do
			{
				_count++;
				if (_count > 2)
				{
					_count = 0;
					return null;
				}
				httpMessage.Headers.Authorization =
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
						await CommercialApiAuthorizationWrapper.GetAccessTokenAsync(_commercialApiConfigurations, _count <= 1));
				responce = await HttpClient.SendAsync(httpMessage);
			} while (responce.StatusCode == HttpStatusCode.Unauthorized);
			_count = 0;
			return responce;
		}
		internal async Task<HttpResponseMessage> CallWebRequestAsync(ApiEndpoint endpoint, object dto = null, params object[] query)
		{
			var endpointDesc = endpoint.GetEndpointDescription();

			var httpMessage = new HttpRequestMessage(endpointDesc.Method, query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint);
			if (dto != null)
				httpMessage.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

			Logger?.Debug(
				new
				{
					endpointDesc.Method,
					Uri = (query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint),
					httpMessage.Content,
					query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);
			return await CallWebRequestAsync(httpMessage);
		}
		internal async Task<HttpResponseMessage> CallFilesWebRequestAsync(ApiEndpoint endpoint, IFormFile[] files, params object[] query)
		{
			var endpointDesc = endpoint.GetEndpointDescription();

			var httpMessage = new HttpRequestMessage(endpointDesc.Method, query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint);
			
			Logger?.Debug(
				new
				{
					endpointDesc.Method,
					Uri = (query.Any() ? string.Format(endpointDesc.Endpoint, query) : endpointDesc.Endpoint),
					httpMessage.Content,
					query,
					OverrideOrganizationCode = _commercialApiConfigurations.OverrideOrganizationCode,
					Culture = _commercialApiConfigurations.Culture
				}
			);
			if (files != null && files.Any())
			{
				var payload = new MultipartFormDataContent();
				foreach(var file in files) {
					payload.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
				}
				httpMessage.Content = payload;
			}
			return await CallWebRequestAsync(httpMessage);
		}
		// string serializeToQuery<T>(T par)
		// {
		//     var properties = from p in typeof(T).GetProperties()
		//                      where p.GetValue(par, null) != null
		//                      select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(par, null).ToString());

		//     // queryString will be set to "Id=1&State=26&Prefix=f&Index=oo"                  
		//    return properties.Any() ? $"?{String.Join("&", properties.ToArray())}" : string.Empty;
		// }

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

		internal DTO convertResponceToDto<DTO>(HttpResponseMessage responce, params JsonConverter[] additionalConvertors) where DTO : class, new()
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

		internal DTO convertWrappedResponceToDto<DTO>(HttpResponseMessage responce, params JsonConverter[] additionalConvertors) {
			using (var dataStream = responce.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
			{
				using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
				{
					var result = reader.ReadToEnd();
					return JsonConvert.DeserializeObject<DTO>($@"{{ ""data"": {result} }}", additionalConvertors);
				}
			}
		}

		internal class Wrapper<T> {
			[JsonProperty("data")]
			T Data { get; set; }
		}
	}
}