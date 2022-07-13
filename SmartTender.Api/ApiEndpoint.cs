namespace SmartTender.Api
{
	public enum ApiEndpoint
	{
		[EndpointDescription("/commercial/tender", EndpointDescriptionAttribute.Post)]
		PostTender,
		[EndpointDescription("/commercial/tender", EndpointDescriptionAttribute.Put)]
		PutTender,
		[EndpointDescription("/commercial/tender/{0}", EndpointDescriptionAttribute.Get)]
		GetTender,
		[EndpointDescription("/commercial/tender/{0}/discussions", EndpointDescriptionAttribute.Get)]
		GetTenderDiscussions,
		[EndpointDescription("/commercial/tender/{0}/comparsiontable", EndpointDescriptionAttribute.Get)]
		GetTenderComparativeTable,
		[EndpointDescription("/commercial/tender/{0}/documents/{1}", EndpointDescriptionAttribute.Get)]
		GetTenderDocument,
		[EndpointDescription("/commercial/tender/{0}/documents/{1}", EndpointDescriptionAttribute.Delete)]
		DeleteTenderDocument,
		[EndpointDescription("/commercial/tender/{0}/documents", EndpointDescriptionAttribute.Post)]
		PostTenderDocument,
		[EndpointDescription("/commercial/organization/availableorganizations", EndpointDescriptionAttribute.Get)]
		GetAvailableOrganizations,
		[EndpointDescription("?/commercial?/criteria?/cards", EndpointDescriptionAttribute.Get)]
		GetCriteriaCards,
		[EndpointDescription("?/commercial/organization/biddingtypes", EndpointDescriptionAttribute.Get)]
		GetBiddingTypes,
		[EndpointDescription("?/commercial/organization/prolongrules", EndpointDescriptionAttribute.Get)]
		GetProlongationRules,
		[EndpointDescription("?/commercial/organization/currencies", EndpointDescriptionAttribute.Get)]
		GetCurrencies,
		[EndpointDescription("/commercial/tender/contact", EndpointDescriptionAttribute.Get)]
		GetAvailableContacts,
		[EndpointDescription("/commercial/unit", EndpointDescriptionAttribute.Get)]
		GetUnits,
		[EndpointDescription("/commercial/webhooks", EndpointDescriptionAttribute.Get)]
		GetWebHooks,
		[EndpointDescription("/commercial/webhooks", EndpointDescriptionAttribute.Post)]
		PostWebHooks,
		[EndpointDescription("/commercial/webhooks/{0}", EndpointDescriptionAttribute.Put)]
		PutWebHooks,
		[EndpointDescription("/commercial/webhooks/{0}", EndpointDescriptionAttribute.Delete)]
		DeleteWebHooks,
		[EndpointDescription("/commercial/categories", EndpointDescriptionAttribute.Get)]
		GetClassifications
	}
}