using System;
using System.Net.Http;
using System.Reflection;
using SmartTender.Api;

public class EndpointDescriptionAttribute : Attribute
{
	public const string Get = "GET";
    public const string Post = "POST";
    public const string Patch = "PATCH";
    public const string Put = "PUT";
    public const string Delete = "DELETE";
    public EndpointDescriptionAttribute(string endpoint, string method = null) 
		: this(endpoint, method == null ? HttpMethod.Post : new HttpMethod(method)) {}
    public EndpointDescriptionAttribute(string endpoint, HttpMethod method = null)
    {
        Endpoint = endpoint;
        Method = method ?? HttpMethod.Post;
    }

    public string Endpoint { get; }
    public HttpMethod Method { get; }
}


internal static class EndpointDescriptionAttributeHelper
{
    public static EndpointDescriptionAttribute GetEndpointDescription(this ApiEndpoint apiEndpoint)
    {
        var type = apiEndpoint.GetType();
        var fieldInfo = type.GetField(apiEndpoint.ToString());
        return fieldInfo
            .GetCustomAttribute(typeof(EndpointDescriptionAttribute), false)
            as EndpointDescriptionAttribute;
    }
}