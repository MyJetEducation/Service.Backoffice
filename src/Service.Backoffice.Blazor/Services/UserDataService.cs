using Service.Backoffice.Blazor.Models;
using Service.Core.Domain.Extensions;
using Service.Core.Domain.Models.Constants;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;
using Service.UserProfile.Grpc;
using Service.UserProfile.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class UserDataService : IUserDataService
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IUserProfileService _userProfileService;

		public UserDataService(IUserInfoService userInfoService, IUserProfileService userProfileService)
		{
			_userInfoService = userInfoService;
			_userProfileService = userProfileService;
		}

		public async Task<UserDataViewModel> GetUserData(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new UserDataViewModel("Please enter user email");

			return GetUserData(new UserInfoGrpcModel()
			{
				IpAddress = "192.168.1.1",
				RefreshToken = "238947892374893",
				RefreshTokenExpires = DateTime.Now,
				Role = UserRole.Default,
				UserId = new Guid("0e612663-cac5-4dd5-b81d-3d6a0ff1a83d"),
				UserName = "userName"
			}, new AccountDataGrpcModel
			{
				LastName = "Ivanov",
				FirstName = "Ivan",
				Phone = "3897489374",
				Country = "Russia",
				Gender = "male"
			});

			UserInfoResponse userInfoResponse = await _userInfoService.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});
			UserInfoGrpcModel userInfo = userInfoResponse?.UserInfo;
			if (userInfo == null)
				return new UserDataViewModel($"No user found by email {email}");

			Guid? userId = userInfo.UserId;
			AccountGrpcResponse accountResponse = await _userProfileService.GetAccount(new GetAccountGrpcRequest {UserId = userId});
			AccountDataGrpcModel accountInfo = accountResponse?.Data;
			if (accountInfo == null)
				return new UserDataViewModel($"No user found by email {email}");

			return GetUserData(userInfo, accountInfo);
		}

		private static UserDataViewModel GetUserData(UserInfoGrpcModel userInfo, AccountDataGrpcModel accountInfo) => new()
		{
			UserInfoItems = new[]
			{
				new ParamValue(nameof(userInfo.UserId), userInfo.UserId.ToString()),
				new ParamValue(nameof(userInfo.UserName), userInfo.UserName),
				new ParamValue(nameof(userInfo.Role), userInfo.Role),
				new ParamValue(nameof(userInfo.IpAddress), userInfo.IpAddress),
				new ParamValue(nameof(userInfo.RefreshToken), userInfo.RefreshToken),
				new ParamValue(nameof(userInfo.RefreshTokenExpires), userInfo.RefreshTokenExpires.ToString())
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