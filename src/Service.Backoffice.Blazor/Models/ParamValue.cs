namespace Service.Backoffice.Blazor.Models
{
	public class ParamValue
	{
		public ParamValue(string param, string value)
		{
			Param = param;
			Value = value;
		}

		public string Param { get; set; }

		public string Value { get; set; }
	}
}