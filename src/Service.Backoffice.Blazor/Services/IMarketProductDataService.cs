using MudBlazor;
using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IMarketProductDataService
	{
		ValueTask<MarketProductDataViewModel> GetProducts(int? productType, DateTime? dateFrom, DateTime? dateTo, bool? withDisabled, TableState tableState);

		ValueTask<UpdateProductResultViewModel> UpdateProduct(MarketProductItemDataViewModel product);
	}
}