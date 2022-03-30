using Service.MarketProduct.Domain.Models;

namespace Service.Backoffice.Blazor.Models
{
	public class MarketProductItemDataViewModel
	{
		public MarketProductType ProductType { get; set; }

		public DateTime? Date { get; set; }

		public bool Disabled { get; set; }

		public string Price { get; set; }
		
		public decimal PriceValue { get; set; }
		
		public int Priority { get; set; }

		public MarketProductCategory Category { get; set; }
	}
}