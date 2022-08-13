using System.Collections.Generic;
using CommercialServices.DTO.ExternalApi;
using Newtonsoft.Json;

namespace SmartTender.Api
{
	public class TenderDto
	{
		/// <summary>
		/// Tender identifier
		/// </summary>
		[JsonProperty("id")]
		public int TenderId { get; set; }

		/// <summary>
		/// Tender number
		/// </summary>
		[JsonProperty("number")]
		public string TenderNumber { get; set; }

		/// <summary>
		/// Tender title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Tender status
		/// </summary>
		[JsonProperty("status")]
		public string TenderStatus { get; set; }

		/// <summary>
		/// Category
		/// </summary>
		[JsonProperty("category")]
		public CategoryDto Category { get; set; }

		/// <summary>
		/// Type tender
		/// </summary>
		[JsonProperty("biddingType")]
		public BiddingTypeShortDto BiddingType { get; set; }

		/// <summary>
		/// Organization
		/// </summary>
		[JsonProperty("organization")]
		public OrganizerDto Organization { get; set; }

		/// <summary>
		/// Tender period
		/// </summary>
		[JsonProperty("tenderPeriod")]
		public PeriodToDto TenderPeriod { get; set; }

		/// <summary>
		/// Delivery period
		/// </summary>
		[JsonProperty("deliveryPeriod")]
		public PeriodToDto DeliveryPeriod { get; set; }

		/// <summary>
		/// Prolong rule code
		/// </summary>
		[JsonProperty("prolongationRule")]
		public string ProlongationRule { get; set; }

		/// <summary>
		/// Currency
		/// </summary>
		[JsonProperty("currency")]
		public CurrencyShortDto Currency { get; set; }

		/// <summary>
		/// With Vat sign
		/// </summary>
		[JsonProperty("vatInclude")]
		public bool VatInclude { get; set; }

		/// <summary>
		/// Obligation to submit bids for all lots
		/// </summary>
		[JsonProperty("allLotsRequiredBid")]
		public bool AllLotsRequiredBid { get; set; }

		/// <summary>
		/// Contact person
		/// </summary>
		[JsonProperty("contactPerson")]
		public ContactPersonDto ContactPerson { get; set; }

		/// <summary>
		/// Lots
		/// </summary>
		[JsonProperty("lots")]
		public List<LotDto> Lots { get; set; }

		/// <summary>
		/// Bids
		/// </summary>
		[JsonProperty("bids")]
		public List<BidDto> Bids { get; set; }

		/// <summary>
		/// Attachments
		/// </summary>
		[JsonProperty("attachments")]
		public List<AttachmentDto> Attachments { get; set; }
	}
}