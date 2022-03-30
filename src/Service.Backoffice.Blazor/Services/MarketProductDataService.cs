using MudBlazor;
using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.MarketProduct.Domain.Models;
using Service.MarketProduct.Grpc;
using Service.MarketProduct.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class MarketProductDataService : IMarketProductDataService
	{
		private readonly IGrpcServiceProxy<IMarketProductService> _marketProductService;

		public MarketProductDataService(IGrpcServiceProxy<IMarketProductService> marketProductService) => _marketProductService = marketProductService;

		public async ValueTask<MarketProductDataViewModel> GetProducts(int? productType, int? category, DateTime? dateFrom, DateTime? dateTo, bool? withDisabled, TableState tableState)
		{
			ProductListGrpcResponse productList = await _marketProductService.Service.GetProductListAsync(new GetProductListGrpcRequest
			{
				ProductTypes = productType != null ? new[] {(MarketProductType) productType} : null,
				ProductCategory = category != null ? (MarketProductCategory) category : null,
				WithDisabled = withDisabled
			});

			IEnumerable<MarketProductGrpcModel> allItems = (productList?.Products ?? Array.Empty<MarketProductGrpcModel>())
				.WhereIf(dateFrom != null, model => model.Date >= dateFrom)
				.WhereIf(dateTo != null, model => model.Date <= dateTo);

			IOrderedEnumerable<MarketProductGrpcModel> pageItems = allItems
				.Skip(tableState.Page * tableState.PageSize)
				.Take(tableState.PageSize)
				.OrderByDirection(tableState.SortDirection, GetSortFunc(tableState.SortLabel));

			var viewModels = new List<MarketProductItemDataViewModel>();

			foreach (MarketProductGrpcModel product in pageItems)
			{
				viewModels.Add(new MarketProductItemDataViewModel
				{
					ProductType = product.ProductType,
					Date = product.Date,
					Disabled = product.Disabled,
					Price = $"{product.Price:0.00}",
					PriceValue = product.Price.GetValueOrDefault(),
					Category = product.Category
				});
			}

			return new MarketProductDataViewModel
			{
				Products = viewModels.ToArray(),
				TotalItems = allItems.Count()
			};
		}

		private static Func<MarketProductGrpcModel, object> GetSortFunc(string sortLabel) =>
			sortLabel switch
			{
				nameof(MarketProductGrpcModel.Date) => o => o.Date,
				nameof(MarketProductGrpcModel.Category) => o => o.Category,
				nameof(MarketProductGrpcModel.Disabled) => o => o.Disabled,
				nameof(MarketProductGrpcModel.Price) => o => o.Price,
				nameof(MarketProductGrpcModel.ProductType) => o => o.ProductType,
				_ => o => o.ProductType
				};

		public async ValueTask<UpdateProductResultViewModel> UpdateProduct(MarketProductItemDataViewModel product)
		{
			if (product == null)
				return UpdateProductResultViewModel.ErrorResult("Product is not set!");

			if (product.PriceValue <= 0)
				return UpdateProductResultViewModel.ErrorResult("Invalid price value!");

			CommonGrpcResponse result = await _marketProductService.Service.UpdateProductAsync(new UpdateProductGrpcRequest
			{
				ProductType = product.ProductType,
				Price = product.PriceValue,
				Disabled = product.Disabled,
				Category = product.Category
			});

			return result.IsSuccess == false
				? UpdateProductResultViewModel.ErrorResult("Error while updating market product item!")
				: UpdateProductResultViewModel.SavedResult();
		}
	}
}