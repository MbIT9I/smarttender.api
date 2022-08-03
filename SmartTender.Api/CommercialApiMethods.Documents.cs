using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommercialServices.DTO.Classification;
using CommercialServices.DTO.MeasuringUnit;
using CommercialServices.DTO.Tender;
using CommercialServices.DTO.WebHooks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;

namespace SmartTender.Api
{
	public partial class CommercialApiMethods
	{
		public async Task<byte[]> GetTenderComparativeTableAsync(int tenderId)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetTenderComparativeTable, query: tenderId);
			if (CommercialApi.checkResponceStatuses(tenderId, responce)) {
				return await responce.Content.ReadAsByteArrayAsync();
			}
			return null;
		}
		public async Task<byte[]> GetTenderDocumentAsync(int tenderId, int documentId)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.GetTenderDocument, null, tenderId, documentId);
			if (CommercialApi.checkResponceStatuses(new { tenderId, documentId } , responce))
				return await responce.Content.ReadAsByteArrayAsync();
			return null;
		}
		public async Task<int[]> PostTenderDocumentAsync(int tenderId, params IFormFile[] files)
		{
			var responce = await CommercialApi.CallFilesWebRequestAsync(ApiEndpoint.PostTenderDocument, files, tenderId);
			if (CommercialApi.checkResponceStatuses(new { tenderId }, responce))
				return CommercialApi.convertWrappedResponceToDto<int[]>(responce);
			return null;
		}
		public async Task<bool> DeleteTenderDocumentAsync(int tenderId, int documentId)
		{
			var responce = await CommercialApi.CallWebRequestAsync(ApiEndpoint.DeleteTenderDocument, null, tenderId);
			if (CommercialApi.checkResponceStatuses(new { tenderId, documentId }, responce))
				return responce.StatusCode == System.Net.HttpStatusCode.OK;
			return false;
		}
	}
}