Using example
```c#
public static class CommercialApi
{
    private static CommercialApiMethods _api;

    static CommercialApi()
    {
        _api = new CommercialApiMethods(new CommercialApiConfigurations(
            "CLIENT_ID",
            "CLIENT_SECRET",
            queryScopes: "smarttender.outerapi",
            isProduction: false
        ));
    }

    public static void PostTender(TenderCreateDto<LotCreateDto> createTenderDto)
    {
       var postedTender = _api.PostTenderAsync(createTenderDto).GetAwaiter().GetResult();
        
    }
}
```
