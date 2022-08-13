// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
Logger.ShowDebugMessage = false;
var res = CommercialApi.GetTender(5992539);


Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(res));