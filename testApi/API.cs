using CommercialServices.DTO.Tender;
using SmartTender.Api;

public static class CommercialApi
{
    private static CommercialApiMethods _api;

	internal static TenderDto GetTender(int v)
	{
		return _api.GetTenderAsync(v).GetAwaiter().GetResult();
	}

	static CommercialApi()
    {
        _api = new CommercialApiMethods(new CommercialApiConfigurations(
            "procurement.develop.api",
            "79834896-79d2-4cf7-ac20-9eefb38c4474",
            queryScopes: "smarttender.outerapi",
            isProduction: false
        ){ Logger = new Logger(), OverrideOrganizationCode = 212834, Culture = Cultures.En });
    }

    public static void PostTender(TenderCreateDto<LotCreateDto> createTenderDto)
    {
       var postedTender = _api.PostTenderAsync(createTenderDto).GetAwaiter().GetResult();
        
    }

	 public static List<CommercialServices.DTO.MeasuringUnit.MeasuringUnitAvailableDto> GetUnits()
    {
        var units = _api.GetAvialableUnitsAsync().GetAwaiter().GetResult();
		return units;
	}
}

public class Logger : ITLogger
{
	public static bool ShowDebugMessage { get; set; } = false;

	public void AddMultiEvents(object obj, string message)
	{
		if (ShowDebugMessage) {
			Console.WriteLine(message);
			Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
			Console.WriteLine();
		}
	}

	public void Debug(object obj)
	{
		if (ShowDebugMessage) {
			Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
			Console.WriteLine();
		}
	}

	public void LogError(object obj, string message)
	{
		if (ShowDebugMessage) {
			Console.WriteLine(message);
			Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
			Console.WriteLine();
		}
	}

	public void LogEvent(object obj, string message)
	{
		if (ShowDebugMessage) {
			Console.WriteLine(message);
			Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
			Console.WriteLine();
		}
	}

	public void LogMultiEvents(string message)
	{
		if (ShowDebugMessage) {
			Console.WriteLine(message);
			Console.WriteLine();
		}
	}
}