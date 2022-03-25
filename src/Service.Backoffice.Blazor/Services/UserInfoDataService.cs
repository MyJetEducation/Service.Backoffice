using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Grpc;
using Service.UserAccount.Grpc;
using Service.UserAccount.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class UserInfoDataService : IUserInfoDataService
	{
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;
		private readonly IGrpcServiceProxy<IUserAccountService> _userProfileService;

		public UserInfoDataService(IGrpcServiceProxy<IUserInfoService> userInfoService, IGrpcServiceProxy<IUserAccountService> userProfileService)
		{
			_userInfoService = userInfoService;
			_userProfileService = userProfileService;
		}

		public async ValueTask<UserInfoDataViewModel> GetUserData(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new UserInfoDataViewModel("Please enter user email");

			UserInfoResponse userInfoResponse = await _userInfoService.Service.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});
			UserInfoGrpcModel userInfo = userInfoResponse?.UserInfo;
			if (userInfo == null)
				return new UserInfoDataViewModel($"No user found by email {email}");

			Guid? userId = userInfo.UserId;
			AccountGrpcResponse accountResponse = await _userProfileService.Service.GetAccount(new GetAccountGrpcRequest {UserId = userId});
			AccountDataGrpcModel accountInfo = accountResponse?.Data;
			if (accountInfo == null)
				return new UserInfoDataViewModel($"No user found by email {email}");

			return GetUserData(userInfo, accountInfo);
		}

		private static UserInfoDataViewModel GetUserData(UserInfoGrpcModel userInfo, AccountDataGrpcModel accountInfo) => new()
		{
			UserInfoItems = new[]
			{
				new ParamValue(nameof(userInfo.UserId), userInfo.UserId.ToString()),
				new ParamValue(nameof(userInfo.UserName), userInfo.UserName),
				new ParamValue(nameof(userInfo.Role), userInfo.Role),
			},
			ProfileItems = new[]
			{
				new ParamValue(nameof(accountInfo.FirstName), accountInfo.FirstName),
				new ParamValue(nameof(accountInfo.LastName), accountInfo.LastName),
				new ParamValue(nameof(accountInfo.Country), accountInfo.Country),
				new ParamValue(nameof(accountInfo.Gender), accountInfo.Gender),
				new ParamValue(nameof(accountInfo.Phone), accountInfo.Phone)
			}
		};
	}
}