namespace Service.Backoffice.Blazor.Models
{
	public class EmailSenderOperationParamValue: ParamValue
	{
		public EmailSenderOperationParamValue(string param, string value, string url) : base(param, value)
		{
			Url = url;
		}

		public string Url { get; set; }
	}
}