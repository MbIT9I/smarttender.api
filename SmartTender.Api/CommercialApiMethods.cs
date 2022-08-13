using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommercialServices.DTO.Classification;
using CommercialServices.DTO.ExternalApi;
using CommercialServices.DTO.MeasuringUnit;
using CommercialServices.DTO.Question;
using CommercialServices.DTO.Tender;
using CommercialServices.DTO.WebHooks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;

namespace SmartTender.Api
{
	public partial class CommercialApiMethods
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
		public async Task<List<BidDiscussionDto>> GetTenderDiscussionsAsync(int tenderId)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetTenderDiscussions, query: tenderId);
			if (CommercialApi.checkResponceStatuses(tenderId, responce))
				return CommercialApi.convertResponceToDto<List<BidDiscussionDto>>(responce);
			return new List<BidDiscussionDto>();
		}
	}
}