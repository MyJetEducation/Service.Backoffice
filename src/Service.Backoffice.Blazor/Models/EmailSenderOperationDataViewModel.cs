namespace Service.Backoffice.Blazor.Models
{
	public class EmailSenderOperationDataViewModel : ViewModelBase
	{
		public EmailSenderOperationDataViewModel(string errorText) : base(errorText)
		{
		}

		public EmailSenderOperationDataViewModel()
		{
		}

		public ParamValue[] Items { get; set; }
	}
}