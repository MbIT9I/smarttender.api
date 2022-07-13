using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommercialServices.DTO.Classification;
using CommercialServices.DTO.MeasuringUnit;
using CommercialServices.DTO.Tender;
using CommercialServices.DTO.WebHooks;
using Newtonsoft.Json.Converters;

namespace SmartTender.Api
{
	public class CommercialApiMethods
	{
		public CommercialApiMethods(CommercialApiConfigurations config)
		{
			CommercialApi = CommercialApiWrapper.GetApiWrapper(config);
		}

		internal CommercialApiWrapper CommercialApi { get; }

		public async Task<TenderCreateResultDto<LotCreateResultDto>> PostTenderAsync(TenderCreateDto<LotCreateDto> tender)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.PostTender, tender);
			if (CommercialApi.checkResponceStatuses(tender, responce))
				return CommercialApi.convertResponceToDto<TenderCreateResultDto<LotCreateResultDto>>(responce);
			return null;

		}
		public async Task<TenderCreateResultDto<LotCreateResultDto>> UpdateTenderAsync(TenderUpdateDto<LotUpdateDto> tender)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.PutTender, tender);
			if (CommercialApi.checkResponceStatuses(tender, responce))
				return CommercialApi.convertResponceToDto<TenderCreateResultDto<LotCreateResultDto>>(responce);
			return null;
		}
		public async Task<TenderDto> GetTenderAsync(int tenderId)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetTender, query: tenderId);
			if (CommercialApi.checkResponceStatuses(tenderId, responce))
				return CommercialApi.convertResponceToDto<TenderDto>(responce);
			return null;
		}
		public async Task<List<TenderContactDto>> GetAvialableContactPersonAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetAvailableContacts);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<TenderContactDto>>(responce);
			return new List<TenderContactDto>();
		}

		public async Task<List<MeasuringUnitAvailableDto>> GetAvialableUnitsAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetUnits);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<MeasuringUnitAvailableDto>>(responce);
			return new List<MeasuringUnitAvailableDto>();
		}

		public async Task<List<ClassificationTreeDto>> GetAvialableClassificationsAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetClassifications);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<ClassificationTreeDto>>(responce);
			return new List<ClassificationTreeDto>();
		}

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