using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Core.Client.Extensions;

namespace Service.Backoffice.Blazor.Services
{
	public static class ServiceHelper
	{
		public static string FormatJsonValue(string value) => value.IsNullOrWhiteSpace()
			? string.Empty
			: JToken.Parse(value).ToString(Formatting.Indented);
	}
}