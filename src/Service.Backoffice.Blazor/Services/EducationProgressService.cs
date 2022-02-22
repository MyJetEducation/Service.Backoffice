using System.Text.Json;
using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class EducationProgressService : IEducationProgressService
	{
		private readonly EducationProgress.Grpc.IEducationProgressService _educationProgressService;
		private readonly IUserInfoService _userInfoService;
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public EducationProgressService(EducationProgress.Grpc.IEducationProgressService educationProgressService,
			IUserInfoService userInfoService,
			IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService)
		{
			_educationProgressService = educationProgressService;
			_userInfoService = userInfoService;
			_serverKeyValueService = serverKeyValueService;
		}

		public async ValueTask<EducationProgressDataViewModel> GetProgress(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Please enter user email");

			UserInfoGrpcModel userInfo = await GetUserId(email);
			if (userInfo == null)
				return new EducationProgressDataViewModel($"No user found by email {email}");

			return await GetProgressByUser(userInfo.UserId);
		}

		private async ValueTask<UserInfoGrpcModel> GetUserId(string email)
		{
			UserInfoResponse userInfoResponse = await _userInfoService.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});

			return userInfoResponse?.UserInfo;
		}

		public async ValueTask<EducationProgressDataViewModel> ClearProgress(string email, EducationTutorial? tutorial, int? unit, int? task)
		{
			if (email.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Please enter user email");

			UserInfoGrpcModel userInfo = await GetUserId(email);
			if (userInfo == null)
				return new EducationProgressDataViewModel($"No user found by email {email}");

			Guid? userId = userInfo.UserId;

			CommonGrpcResponse response = await _educationProgressService.InitProgressAsync(new InitEducationProgressGrpcRequest
			{
				UserId = userId,
				Tutorial = tutorial,
				Unit = unit,
				Task = task
			});

			if (!response.IsSuccess)
				return new EducationProgressDataViewModel($"Error occured while clearing progress for user {email}");

			return await GetProgressByUser(userId);
		}

		public async ValueTask<EducationProgressDataViewModel> GetProgressByUser(Guid? userId)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				Key = Program.Settings.KeyEducationProgress,
				UserId = userId
			});

			string value = response.Value;
			if (value.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Error occured while retrieving education progress for user");

			EducationProgressDto[] dtos = JsonSerializer.Deserialize<EducationProgressDto[]>(value);

			return new EducationProgressDataViewModel
			{
				Items = dtos
			};
		}
	}
}