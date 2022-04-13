using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Backoffice.Models;
using Service.Core.Client.Extensions;
using Service.Grpc;
using Service.MarketProduct.Domain.Models;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Grpc;
using Service.UserTokenAccount.Grpc.Models;

namespace Service.Backoffice.Services
{
	public class UserTokenAccountDataService : IUserTokenAccountDataService
	{
		private readonly IGrpcServiceProxy<IUserTokenAccountService> _userTokenAccountService;
		private readonly IPersonalDataServiceGrpc _personalDataServiceGrpc;

		public UserTokenAccountDataService(IGrpcServiceProxy<IUserTokenAccountService> userTokenAccountService, IPersonalDataServiceGrpc personalDataServiceGrpc)
		{
			_userTokenAccountService = userTokenAccountService;
			_personalDataServiceGrpc = personalDataServiceGrpc;
		}

		public async ValueTask<UserTokenAccountDataViewModel> GetOperations(string userId, DateTime? dateFrom, DateTime? dateTo, int? movement, int? source, int? productType, TableState tableState)
		{
			OperationsGrpcResponse operationsResposne = await _userTokenAccountService.Service.GetOperationsAsync(new GetOperationsGrpcRequest
			{
				UserId = userId,
				Movement = (TokenOperationMovement?) movement,
				ProductType = (MarketProductType?) productType,
				Source = (TokenOperationSource?) source
			});

			IEnumerable<OperationGrpcModel> allItems = (operationsResposne?.Operations ?? Array.Empty<OperationGrpcModel>())
				.WhereIf(dateFrom != null, model => model.Date >= dateFrom)
				.WhereIf(dateTo != null, model => model.Date <= dateTo);

			IOrderedEnumerable<OperationGrpcModel> pageItems = allItems
				.Skip(tableState.Page * tableState.PageSize)
				.Take(tableState.PageSize)
				.OrderByDirection(tableState.SortDirection, GetSortFunc(tableState.SortLabel));

			var viewModels = new List<UserTokenAccountDataOperationViewModel>();

			foreach (OperationGrpcModel operation in pageItems)
			{
				string operationUserId = operation.UserId;

				AccountGrpcResponse accountResponse = await _userTokenAccountService.Service.GetAccountAsync(new GetAccountGrpcRequest {UserId = operationUserId});

				bool isIncome = operation.Movement == TokenOperationMovement.Income;
				string sign = isIncome ? "+" : "-";

				viewModels.Add(new UserTokenAccountDataOperationViewModel
				{
					UserId = operationUserId,
					Date = operation.Date.GetValueOrDefault(),
					Movement = operation.Movement.ToString(),
					ProductType = operation.ProductType.ToString(),
					Source = operation.Source.ToString(),
					Value = $"{sign} {operation.Value:0.00}",
					Info = FormatJsonValue(operation.Info),
					UserName = await GetUserEmail(operationUserId),
					IsIncome = isIncome,
					Total = accountResponse?.Value
				});
			}

			ParamValue[] paramValues = viewModels
				.Select(p => new ParamValue(p.UserId, p.UserName))
				.DistinctBy(p => p.Param)
				.OrderBy(p => p.Value)
				.ToArray();

			return new UserTokenAccountDataViewModel
			{
				Operations = viewModels.ToArray(),
				UserFilter = paramValues,
				TotalItems = allItems.Count()
			};
		}

		private static string FormatJsonValue(string value) => value.IsNullOrWhiteSpace()
			? string.Empty
			: JToken.Parse(value).ToString(Formatting.Indented);

		private async ValueTask<string> GetUserEmail(string userId)
		{
			PersonalDataGrpcResponseContract response = await _personalDataServiceGrpc.GetByIdAsync(new GetByIdRequest() {Id = userId});

			return response?.PersonalData?.Email;
		}

		private static Func<OperationGrpcModel, object> GetSortFunc(string sortLabel) =>
			sortLabel switch
			{
				nameof(OperationGrpcModel.Date) => o => o.Date,
				nameof(OperationGrpcModel.Info) => o => o.Info,
				nameof(OperationGrpcModel.Movement) => o => o.Movement,
				nameof(OperationGrpcModel.ProductType) => o => o.ProductType,
				nameof(OperationGrpcModel.Source) => o => o.Source,
				nameof(OperationGrpcModel.UserId) => o => o.UserId,
				nameof(OperationGrpcModel.Value) => o => o.Value,
				_ => o => o.Date
			};
	}
}