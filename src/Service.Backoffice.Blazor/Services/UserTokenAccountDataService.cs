using MudBlazor;
using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Grpc;
using Service.MarketProduct.Domain.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Grpc;
using Service.UserTokenAccount.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class UserTokenAccountDataService : IUserTokenAccountDataService
	{
		private readonly IGrpcServiceProxy<IUserTokenAccountService> _userTokenAccountService;
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;

		public UserTokenAccountDataService(IGrpcServiceProxy<IUserTokenAccountService> userTokenAccountService, IGrpcServiceProxy<IUserInfoService> userInfoService)
		{
			_userTokenAccountService = userTokenAccountService;
			_userInfoService = userInfoService;
		}

		public async ValueTask<UserTokenAccountDataViewModel> GetOperations(Guid? userId, DateTime? dateFrom, DateTime? dateTo, int? movement, int? source, int? productType, TableState tableState)
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
				Guid? operationUserId = operation.UserId;

				AccountGrpcResponse accountResponse = await _userTokenAccountService.Service.GetAccountAsync(new GetAccountGrpcRequest {UserId = operationUserId});

				UserInfoResponse userInfo = await _userInfoService.Service.GetUserInfoByIdAsync(new UserInfoRequest {UserId = operationUserId});
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
					Info = ServiceHelper.FormatJsonValue(operation.Info),
					UserName = userInfo?.UserInfo?.UserName,
					IsIncome = isIncome,
					Total = accountResponse?.Value
				});
			}

			UidParamValue[] paramValues = viewModels
				.Select(p => new UidParamValue(p.UserId, p.UserName))
				.DistinctBy(p => p.Id)
				.OrderBy(p => p.Name)
				.ToArray();

			return new UserTokenAccountDataViewModel
			{
				Operations = viewModels.ToArray(),
				UserFilter = paramValues,
				TotalItems = allItems.Count()
			};
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