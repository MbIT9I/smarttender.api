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
		public async Task<List<CriteriaCardDto>> GetAvialableCriteriaCardsAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetCriteriaCards);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<CriteriaCardDto>>(responce);
			return new List<CriteriaCardDto>();
		}
		public async Task<List<BiddingTypeDto>> GetAvialableBiddingTypesAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetBiddingTypes);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<BiddingTypeDto>>(responce);
			return new List<BiddingTypeDto>();
		}
		public async Task<List<ProlongationRuleDto>> GetAvialableProlongationRulesAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetProlongationRules);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<ProlongationRuleDto>>(responce);
			return new List<ProlongationRuleDto>();
		}
		public async Task<List<CurrencyDto>> GetAvialableCurrenciesAsync()
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetProlongationRules);
			if (CommercialApi.checkResponceStatuses<string>(null, responce))
				return CommercialApi.convertResponceToDto<List<CurrencyDto>>(responce);
			return new List<CurrencyDto>();
		}
	}
}