namespace Service.Backoffice.Blazor.Models
{
	public class EmailSenderOperationDataParamValue: ParamValue
	{
		public EmailSenderOperationDataParamValue(string param, string value, string url) : base(param, value)
		{
			Url = url;
		}

		public string Url { get; set; }
	}
}