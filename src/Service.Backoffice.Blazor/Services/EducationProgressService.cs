using System.Text.Json;
using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.Models;
using Service.Grpc;
using Service.KeyValue.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;
using KeyValueGetKeysGrpcRequest = Service.KeyValue.Grpc.Models.GetKeysGrpcRequest;
using KeyValueItemsGetGrpcRequest = Service.KeyValue.Grpc.Models.ItemsGetGrpcRequest;
using KeyValueItemsPutGrpcRequest = Service.KeyValue.Grpc.Models.ItemsPutGrpcRequest;
using KeyValueKeyValueGrpcModel = Service.KeyValue.Grpc.Models.KeyValueGrpcModel;

namespace Service.Backoffice.Blazor.Services
{
	public class EducationProgressService : IEducationProgressService
	{
		private readonly EducationProgress.Grpc.IEducationProgressService _educationProgressService;
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;
		private readonly IKeyValueService _keyValueService;

		public EducationProgressService(EducationProgress.Grpc.IEducationProgressService educationProgressService,
			IGrpcServiceProxy<IUserInfoService> userInfoService,
			IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService,
			IKeyValueService keyValueService)
		{
			_educationProgressService = educationProgressService;
			_userInfoService = userInfoService;
			_serverKeyValueService = serverKeyValueService;
			_keyValueService = keyValueService;
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
			UserInfoResponse userInfoResponse = await _userInfoService.Service.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});

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

		public async Task ClearAll(string email, bool clearProgress, bool clearUiProgress, bool clearAchievements, bool clearStatuses, bool clearHabits, bool clearSkills, bool clearKnowledge, bool clearUserTime)
		{
			if (email.IsNullOrWhiteSpace())
				return;

			UserInfoGrpcModel userInfo = await GetUserId(email);
			if (userInfo == null)
				return;

			Guid? userId = userInfo.UserId;

			if (clearProgress)
				await ClearProgress(email, null, null, null);

			if (clearUiProgress)
				await ClearUiProgress(userId);

			var clearKeys = new List<string>();
			if (clearAchievements)
				clearKeys.AddRange(new[] {"user_achievement", "user_new_achievement", "user_new_achievement_tutorial", "education_retry_used_count", "user_new_achievement_unit"});
			if (clearStatuses)
				clearKeys.AddRange(new[] {"user_status", "tasks_100_prc"});
			if (clearHabits)
				clearKeys.AddRange(new[] {"user_habit"});
			if (clearSkills)
				clearKeys.AddRange(new[] {"user_skill"});
			if (clearKnowledge)
				clearKeys.AddRange(new[] {"user_knowledge"});
			if (clearUserTime)
				clearKeys.AddRange(new[] {"user_day_time", "user_time"});
			if (clearKeys.Any())
				await ClearServerKeyValues(userId, clearKeys);
		}

		private async Task ClearServerKeyValues(Guid? userId, IEnumerable<string> keys)
		{
			await _serverKeyValueService.Service.Delete(new ItemsDeleteGrpcRequest
			{
				UserId = userId,
				Keys = keys.ToArray()
			});
		}

		private async Task ClearUiProgress(Guid? userId)
		{
			string[] keys = (await _keyValueService.GetKeys(new KeyValueGetKeysGrpcRequest
			{
				UserId = userId
			}))?.Keys ?? Array.Empty<string>();

			string[] menuKeys = keys.Where(s => s.StartsWith("progressMenu")).ToArray();
			if (menuKeys.IsNullOrEmpty())
				return;

			KeyValueKeyValueGrpcModel[] items = (await _keyValueService.Get(new KeyValueItemsGetGrpcRequest
			{
				UserId = userId,
				Keys = menuKeys
			}))?.Items;

			if (items == null)
				return;

			foreach (KeyValueKeyValueGrpcModel item in items)
				item.Value = item.Value.Replace("\"valid\":true", "\"valid\":false");

			await _keyValueService.Put(new KeyValueItemsPutGrpcRequest
			{
				UserId = userId,
				Items = items
			});
		}
	}
}