using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommercialServices.DTO.Classification;
using CommercialServices.DTO.Criteria;
using CommercialServices.DTO.MeasuringUnit;
using CommercialServices.DTO.Organization;
using CommercialServices.DTO.Tender;
using CommercialServices.DTO.WebHooks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;

namespace SmartTender.Api
{
	public partial class CommercialApiMethods
	{
		public async Task<List<WebHookDto>> GetWebHooksAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetWebHooks);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<WebHookDto>>(responce, new StringEnumConverter(new ConstantCaseNamingStrategy()));
			return new List<WebHookDto>();
		}
		public async Task<WebHookDto> PostWebHookAsync(WebHookInputDto input)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.PostWebHooks, input);
			if (CommercialApi.checkResponceStatuses(input, responce))
				return CommercialApi.convertResponceToDto<WebHookDto>(responce, new StringEnumConverter(new ConstantCaseNamingStrategy()));
			return null;
		}
		public async Task<WebHookDto> PutWebHookAsync(Guid id, WebHookEditDto input)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.PutWebHooks, input, query: id);
			if (CommercialApi.checkResponceStatuses(input, responce))
				return CommercialApi.convertResponceToDto<WebHookDto>(responce, new StringEnumConverter(new ConstantCaseNamingStrategy()));
			return null;
		}
		public async Task<bool> DeleteWebHookAsync(Guid id, string code)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.DeleteWebHooks, code, query: id);
			return CommercialApi.checkResponceStatuses(id, responce);
		}
	}
}