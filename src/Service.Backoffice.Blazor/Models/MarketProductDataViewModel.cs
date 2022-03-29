namespace Service.Backoffice.Blazor.Models
{
	public class MarketProductDataViewModel: ViewModelBase
	{
		public MarketProductDataViewModel(string errorText) : base(errorText)
		{
		}

		public MarketProductDataViewModel()
		{
		}

		public int TotalItems { get; set; }

		public MarketProductItemDataViewModel[] Products { get; set; }
	}
}