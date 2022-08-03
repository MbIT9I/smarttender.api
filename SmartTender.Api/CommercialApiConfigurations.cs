namespace SmartTender.Api
{
	public class CommercialApiConfigurations
	{
		private const string PublicAuthOrigin = "https://smartid.smarttender.biz/login";
		private const string PublicAuthOriginTest = "https://smartid-test.smarttender.biz/2.0/login";

		public CommercialApiConfigurations(string clientId, string clientSecret, string queryScopes = null, bool isProduction = true)
		{
			Authority = isProduction ? PublicAuthOrigin : PublicAuthOriginTest;
			ClientId = clientId;
			ClientSecret = clientSecret;
			QueryScopes = queryScopes;
			IsProductionMode = isProduction;
		}
		internal bool IsProductionMode { get; }
		public string Authority { get; }
		public string ClientId { get; }
		public string ClientSecret { get; }
		public string QueryScopes { get; }
		public ITLogger Logger { get; set; }

		public int? OverrideOrganizationCode { get; set; }
		public Cultures? Culture { get; set; }
		public string OverrideOrigin { get; set; }
	}
}