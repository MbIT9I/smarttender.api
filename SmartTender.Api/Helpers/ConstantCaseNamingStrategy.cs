using Newtonsoft.Json.Serialization;
using Panic.StringUtils;

namespace SmartTender.Api
{
	public sealed class ConstantCaseNamingStrategy : NamingStrategy
	{
		protected override string ResolvePropertyName(string name) => StringUtils.ToConstantCase(name);
	}
}