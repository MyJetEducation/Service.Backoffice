namespace Service.Backoffice.Models
{
	public class RetryDataViewModel : ViewModelBase
	{
		public RetryDataViewModel(string errorText) : base(errorText)
		{
		}

		public RetryDataViewModel()
		{
		}

		public int Count { get; set; }
		public DateTime? LastDate { get; set; }
	}
}