using Newtonsoft.Json;
using Service.Backoffice.Blazor.Models;
using Service.Backoffice.Blazor.Settings;
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
using JsonSerializer = System.Text.Json.JsonSerializer;
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

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new EducationProgressDataViewModel($"No user found by email {email}");

			return await GetProgressByUser(userId);
		}

		public async ValueTask<EducationProgressDataViewModel> ClearProgress(string email, EducationTutorial? tutorial, int? unit, int? task)
		{
			if (email.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Please enter user email");

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new EducationProgressDataViewModel($"No user found by email {email}");

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
				Key = Program.Settings.ServerKeyValueKeys.EducationProgressKey,
				UserId = userId
			});

			string value = response.Value;
			if (value.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Error occured while retrieving education progress for user");

			EducationProgressGrpcResponse progress = await _educationProgressService.GetProgressAsync(new GetEducationProgressGrpcRequest {UserId = userId});

			return new EducationProgressDataViewModel
			{
				Items = JsonSerializer.Deserialize<EducationProgressDto[]>(value),
				TaskScore = (progress?.TaskScore).GetValueOrDefault(),
				TestScore = (progress?.TestScore).GetValueOrDefault()
			};
		}

		public async ValueTask ClearAll(string email, ClearProgressFlags clear)
		{
			if (email.IsNullOrWhiteSpace())
				return;

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return;

			if (clear.Progress)
				await ClearProgress(email, null, null, null);

			if (clear.UiProgress)
				await ClearUiProgress(userId);

			var keysList = new List<string>();
			void AddKeys(Func<ServerKeyValueKeysSettingsModel, string> value) => keysList.AddRange(value.Invoke(Program.Settings.ServerKeyValueKeys).Split(","));

			if (clear.Achievements)
				AddKeys(k => k.AchievementKeys);
			if (clear.Statuses)
				AddKeys(k => k.StatusKeys);
			if (clear.Habits)
				AddKeys(k => k.HabitKeys);
			if (clear.Skills)
				AddKeys(k => k.SkillKeys);
			if (clear.Knowledge)
				AddKeys(k => k.KnowledgeKeys);
			if (clear.UserTime)
				AddKeys(k => k.UserTimeKeys);
			if (clear.Retry)
				AddKeys(k => k.RetryKeys);

			if (keysList.Any())
				await ClearServerKeyValues(userId, keysList);
		}

		private async Task ClearServerKeyValues(Guid? userId, IEnumerable<string> keys) => await _serverKeyValueService.Service.Delete(new ItemsDeleteGrpcRequest
		{
			UserId = userId,
			Keys = keys.ToArray()
		});

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

		private async ValueTask<Guid?> GetUserId(string email)
		{
			UserInfoResponse userInfoResponse = await _userInfoService.Service.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});

			return userInfoResponse?.UserInfo?.UserId;
		}

		public async ValueTask<EducationProgressChangeDateDataViewModel> ChangeTaskDate(string email, EducationTutorial tutorial, int unit, int task, DateTime? date)
		{
			if (email.IsNullOrWhiteSpace())
				return new EducationProgressChangeDateDataViewModel("Please enter user email");
			
			if (date == null)
				return new EducationProgressChangeDateDataViewModel("Date is not valid");

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new EducationProgressChangeDateDataViewModel($"No user found by email {email}");

			string educationProgressKey = Program.Settings.ServerKeyValueKeys.EducationProgressKey;

			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = educationProgressKey
			});

			if (response == null)
				return new EducationProgressChangeDateDataViewModel($"Error occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {email}");

			EducationProgressDto[] dtos = JsonConvert.DeserializeObject<EducationProgressDto[]>(response.Value);
			if (dtos == null)
				return new EducationProgressChangeDateDataViewModel($"Error (2) occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {email}");

			EducationProgressDto dto = dtos.Where(dto => dto.Tutorial == tutorial)
				.Where(dto => dto.Unit == unit)
				.First(dto => dto.Task == task);

			dto.Date = date;

			CommonGrpcResponse saveResponse = await _serverKeyValueService.Service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = educationProgressKey,
						Value = JsonConvert.SerializeObject(dtos)
					}
				}
			});

			if (!saveResponse.IsSuccess)
				return new EducationProgressChangeDateDataViewModel($"Error (3) occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {email}");

			return new EducationProgressChangeDateDataViewModel();
		}
	}
}