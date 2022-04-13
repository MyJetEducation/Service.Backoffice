using Newtonsoft.Json;
using Service.Backoffice.Models;
using Service.Backoffice.Settings;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.Grpc;
using Service.KeyValue.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
using KeyValueGetKeysGrpcRequest = Service.KeyValue.Grpc.Models.GetKeysGrpcRequest;
using KeyValueItemsGetGrpcRequest = Service.KeyValue.Grpc.Models.ItemsGetGrpcRequest;
using KeyValueItemsPutGrpcRequest = Service.KeyValue.Grpc.Models.ItemsPutGrpcRequest;
using KeyValueKeyValueGrpcModel = Service.KeyValue.Grpc.Models.KeyValueGrpcModel;

namespace Service.Backoffice.Services
{
	public class EducationProgressDataService : IEducationProgressDataService
	{
		private readonly IEducationProgressService _educationProgressService;
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;
		private readonly IKeyValueService _keyValueService;

		public EducationProgressDataService(IEducationProgressService educationProgressService,
			IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService,
			IKeyValueService keyValueService)
		{
			_educationProgressService = educationProgressService;
			_serverKeyValueService = serverKeyValueService;
			_keyValueService = keyValueService;
		}

		public async ValueTask<EducationProgressDataViewModel> GetProgress(string userId) => userId.IsNullOrWhiteSpace() 
			? new EducationProgressDataViewModel("Please select user") 
			: await GetProgressByUser(userId);

		public async ValueTask<EducationProgressDataViewModel> ClearProgress(string userId, EducationTutorial? tutorial, int? unit, int? task)
		{
			if (userId.IsNullOrWhiteSpace())
				return new EducationProgressDataViewModel("Please select user");

			CommonGrpcResponse response = await _educationProgressService.InitProgressAsync(new InitEducationProgressGrpcRequest
			{
				UserId = userId,
				Tutorial = tutorial,
				Unit = unit,
				Task = task
			});

			return response.IsSuccess 
				? await GetProgressByUser(userId) 
				: new EducationProgressDataViewModel($"Error occured while clearing progress for user {userId}");
		}

		public async ValueTask<EducationProgressDataViewModel> GetProgressByUser(string userId)
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

		public async ValueTask ClearAll(string userId, ClearProgressFlags clear)
		{
			if (userId.IsNullOrWhiteSpace())
				return;

			if (clear.Progress)
				await ClearProgress(userId, null, null, null);

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

		private async Task ClearServerKeyValues(string userId, IEnumerable<string> keys) => await _serverKeyValueService.Service.Delete(new ItemsDeleteGrpcRequest
		{
			UserId = userId,
			Keys = keys.ToArray()
		});

		private async Task ClearUiProgress(string userId)
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

		public async ValueTask<EducationProgressChangeDateDataViewModel> ChangeTaskDate(string userId, EducationTutorial tutorial, int unit, int task, DateTime? date)
		{
			if (userId.IsNullOrWhiteSpace())
				return EducationProgressChangeDateDataViewModel.Error("Please select user");

			if (date == null)
				return EducationProgressChangeDateDataViewModel.Error("Date is not valid");

			string educationProgressKey = Program.Settings.ServerKeyValueKeys.EducationProgressKey;

			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = educationProgressKey
			});

			if (response == null)
				return EducationProgressChangeDateDataViewModel.Error($"Error occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {userId}");

			EducationProgressDto[] dtos = JsonConvert.DeserializeObject<EducationProgressDto[]>(response.Value);
			if (dtos == null)
				return EducationProgressChangeDateDataViewModel.Error($"Error (2) occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {userId}");

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

			return saveResponse.IsSuccess 
				? new EducationProgressChangeDateDataViewModel() 
				: EducationProgressChangeDateDataViewModel.Error($"Error (3) occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {userId}");
		}
	}
}