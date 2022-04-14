using MyJetWallet.Sdk.ServiceBus;
using Newtonsoft.Json;
using Service.Backoffice.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.ServiceBus.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Service.Backoffice.Services
{
	public class EducationProgressDataService : IEducationProgressDataService
	{
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;
		private readonly IEducationProgressService _educationProgressService;
		private readonly IServiceBusPublisher<ClearEducationProgressServiceBusModel> _clearProgressPublisher;
		private readonly IServiceBusPublisher<ClearEducationUiProgressServiceBusModel> _clearUiProgressPublisher;

		public EducationProgressDataService(IEducationProgressService educationProgressService,
			IServiceBusPublisher<ClearEducationProgressServiceBusModel> clearProgressPublisher,
			IServiceBusPublisher<ClearEducationUiProgressServiceBusModel> clearUiProgressPublisher,
			IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService)
		{
			_educationProgressService = educationProgressService;
			_clearProgressPublisher = clearProgressPublisher;
			_clearUiProgressPublisher = clearUiProgressPublisher;
			_serverKeyValueService = serverKeyValueService;
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
				Key = Program.Settings.ServerKeyValueEducationProgressKey,
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

		public async ValueTask ClearAll(string userId, ClearProgressFlags clearFlags)
		{
			if (userId.IsNullOrWhiteSpace())
				return;

			if (clearFlags.NeedClearProgressFlags)
				await _clearProgressPublisher.PublishAsync(new ClearEducationProgressServiceBusModel
				{
					UserId = userId,
					ClearProgress = clearFlags.Progress,
					ClearAchievements = clearFlags.Achievements,
					ClearHabits = clearFlags.Habits,
					ClearKnowledge = clearFlags.Knowledge,
					ClearRetry = clearFlags.Retry,
					ClearSkills = clearFlags.Skills,
					ClearStatuses = clearFlags.Statuses,
					ClearUserTime = clearFlags.UserTime
				});

			if (clearFlags.UiProgress)
				await _clearUiProgressPublisher.PublishAsync(new ClearEducationUiProgressServiceBusModel
				{
					UserId = userId
				});
		}

		public async ValueTask<EducationProgressChangeDateDataViewModel> ChangeTaskDate(string userId, EducationTutorial tutorial, int unit, int task, DateTime? date)
		{
			if (userId.IsNullOrWhiteSpace())
				return EducationProgressChangeDateDataViewModel.Error("Please select user");

			if (date == null)
				return EducationProgressChangeDateDataViewModel.Error("Date is not valid");

			string educationProgressKey = Program.Settings.ServerKeyValueEducationProgressKey;

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