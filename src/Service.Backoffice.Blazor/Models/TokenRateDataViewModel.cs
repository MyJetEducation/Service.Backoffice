namespace Service.Backoffice.Blazor.Models
{
	public class TokenRateDataViewModel : ViewModelBase
	{
		public TokenRateDataViewModel(string errorText) : base(errorText)
		{
		}

		public TokenRateDataViewModel()
		{
		}

		public decimal Value { get; set; }
		public DateTime? Date { get; set; }
	}
}