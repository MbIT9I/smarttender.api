#region DTO
using System;
using System.Collections.Generic;
using CommercialServices.DTO.Attachment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

	public class CategoryDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public string Code { get; set; }

		/// <summary>
		/// Scheme code
		/// </summary>
		[JsonProperty("schemeId")]
		public string SchemeCode { get; set; }

		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Shema title
		/// </summary>
		[JsonProperty("schemeTitle")]
		public string SchemeTitle { get; set; }

		/// <summary>
		/// SmartTender identifier
		/// </summary>
		[JsonProperty("codeSmartTender")]
		public string SmartTenderCode { get; set; }
	}

	public class BiddingTypeShortDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("code")]
		public string Code { get; set; }
		/// <summary>
		/// Public tender
		/// </summary>
		[JsonProperty("isPublic")]
		public bool IsPublic { get; set; }
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }
	}

	public class OrganizerDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public int Code { get; set; }
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>
		/// Usreou
		/// </summary>
		[JsonProperty("usreou")]
		public string Usreou { get; set; }
	}

	public class PeriodToDto
	{
		/// <summary>
		/// From date
		/// </summary>
		[JsonProperty("startDate")]
		public DateTime? StartDate { get; set; }

		/// <summary>
		/// To date
		/// </summary>
		[JsonProperty("endDate")]
		public DateTime? EndDate { get; set; }
	}

	public class CurrencyShortDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public int Code { get; set; }
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>
		/// Designation
		/// </summary>
		[JsonProperty("designation")]
		public string Designation { get; set; }
	}

	public class ContactPersonDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public string Code { get; set; }
		/// <summary>
		/// Phone
		/// </summary>
		[JsonProperty("telephone")]
		public string Telephone { get; set; }
		/// <summary>
		/// Name
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>
		/// Surname
		/// </summary>
		[JsonProperty("lastName")]
		public string LastName { get; set; }
		/// <summary>
		/// Patronymic
		/// </summary>
		[JsonProperty("patronymic")]
		public string Patronymic { get; set; }
	}

	public class LotDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public int LotId { get; set; }

		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("gost")]
		public string Gost { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		[JsonProperty("quantity")]
		public decimal Quantity { get; set; }

		/// <summary>
		/// Unit
		/// </summary>
		[JsonProperty("unit")]
		public UnitDto Unit { get; set; }

		/// <summary>
		/// Initial amount
		/// </summary>
		[JsonProperty("initialAmount")]
		public decimal InitialAmount { get; set; }

		/// <summary>
		/// Minimal step
		/// </summary>
		[JsonProperty("minimalStep")]
		public decimal MinimalStepPercent { get; set; }

		/// <summary>
		/// Analogs
		/// </summary>
		[JsonProperty("analogs")]
		public List<AnalogDto> Analogs { get; set; }
	}

	public class BidDto
	{
		/// <summary>
		/// Bidder information
		/// </summary>
		[JsonProperty("bidderInfo")]
		public BidderInfoDto BidderInfo { get; set; }

		/// <summary>
		/// Bid details
		/// </summary>
		[JsonProperty("bidDetails")]
		public List<BidDetailDto> BidDetails { get; set; }

		/// <summary>
		/// Tender criteria
		/// </summary>
		[JsonProperty("criteria")]
		public List<CriteriaDto> CriteriaValues { get; set; }

		/// <summary>
		/// Attachments
		/// </summary>
		[JsonProperty("attachments")]
		public List<AttachmentDto> Attachments { get; set; }

		[JsonIgnore]
		public int Id { get; set; }
	}

	public class UnitDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("id")]
		public int UnitId { get; set; }
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>
		/// UN/CEFACT
		/// </summary>
		[JsonProperty("uncefact")]
		public string Uncefact { get; set; }
	}

	public class AnalogDto
	{
		/// <summary>
		/// Identifier
		/// </summary>
		[JsonProperty("analogId")]
		public int AnalogId { get; set; }
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }
	}

	public class BidderInfoDto
	{
		/// <summary>
		/// Usreou
		/// </summary>
		[JsonProperty("usreou")]
		public string Usreou { get; set; }

		/// <summary>
		/// Organization name
		/// </summary>
		[JsonProperty("organizationName")]
		public string OrganizationName { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		[JsonProperty("email")]
		public string Email { get; set; }

		/// <summary>
		/// Full name
		/// </summary>
		[JsonProperty("fullName")]
		public string FullName { get; set; }

		/// <summary>
		/// Phone
		/// </summary>
		[JsonProperty("phoneNumber")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Organization Address
		/// </summary>
		[JsonProperty("organizationAddress")]
		public string OrganizationAddress { get; set; }
	}

	public class BidDetailDto
	{
		/// <summary>
		/// Lot identifier
		/// </summary>
		[JsonProperty("lotId")]
		public int LotId { get; set; }

		/// <summary>
		/// Analog identifier
		/// </summary>
		[JsonProperty("analogId")]
		public int? AnalogId { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		[JsonProperty("quantity")]
		public decimal Quantity { get; set; }

		/// <summary>
		/// Unit
		/// </summary>
		[JsonProperty("unit")]
		public UnitDto Unit { get; set; }

		/// <summary>
		/// Price
		/// </summary>
		[JsonProperty("price")]
		public decimal Price { get; set; }

		/// <summary>
		/// Reduced price
		/// </summary>
		[JsonProperty("reducedPrice")]
		public decimal ReducedPrice { get; set; }

		/// <summary>
		/// Currency
		/// </summary>
		[JsonProperty("currencyId")]
		public int CurrencyId { get; set; }

		/// <summary>
		/// Bid submit time
		/// </summary>
		[JsonProperty("timeSupply")]
		[JsonConverter(typeof(IsoDateTimeConverter))]
		public DateTime TimeSupplyBids { get; set; }

		/// <summary>
		/// Delivery condition
		/// </summary>
		[JsonProperty("termsDelivery")]
		public string TermsDelivery { get; set; }

		/// <summary>
		/// Payment condition
		/// </summary>
		[JsonProperty("termsPayment")]
		public string TermsPayment { get; set; }

		/// <summary>
		/// Delivery term
		/// </summary>
		[JsonProperty("deliveryTime")]
		public decimal DeliveryTime { get; set; }

		/// <summary>
		///  Warranty
		/// </summary>
		[JsonProperty("warrantyYears")]
		public decimal WarrantyYears { get; set; }

		/// <summary>
		/// selected bid
		/// </summary>
		[JsonProperty("selectedBid")]
		public bool SelectedBid { get; set; }

		/// <summary>
		/// Additional info
		/// </summary>
		[JsonProperty("additionalInfo")]
		public string AdditionalInfo { get; set; }

	}

	public class CriteriaDto
	{
		/// <summary>
		/// Title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Value
		/// </summary>
		[JsonProperty("value")]
		public string Value { get; set; }

		/// <summary>
		/// Attachments
		/// </summary>
		[JsonProperty("attachments")]
		public List<AttachmentDto> Attachments { get; set; }

		[JsonIgnore]
		public string Code { get; set; }
	}
}
#endregion