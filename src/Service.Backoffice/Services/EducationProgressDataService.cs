using MyJetWallet.Sdk.ServiceBus;
using Service.Backoffice.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.Backoffice.Services
{
	public class EducationProgressDataService : IEducationProgressDataService
	{
		private readonly IEducationProgressService _educationProgressService;
		private readonly IServiceBusPublisher<ClearEducationProgressServiceBusModel> _clearProgressPublisher;
		private readonly IServiceBusPublisher<ClearEducationUiProgressServiceBusModel> _clearUiProgressPublisher;

		public EducationProgressDataService(IEducationProgressService educationProgressService,
			IServiceBusPublisher<ClearEducationProgressServiceBusModel> clearProgressPublisher,
			IServiceBusPublisher<ClearEducationUiProgressServiceBusModel> clearUiProgressPublisher)
		{
			_educationProgressService = educationProgressService;
			_clearProgressPublisher = clearProgressPublisher;
			_clearUiProgressPublisher = clearUiProgressPublisher;
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
			ProgressDataGrpcResponse response = await _educationProgressService.GetProgressData(new GetProgressDataGrpcRequest
			{
				UserId = userId
			});

			if (response == null)
				return new EducationProgressDataViewModel("Error occured while retrieving education progress for user");

			EducationProgressGrpcResponse progress = await _educationProgressService.GetProgressAsync(new GetEducationProgressGrpcRequest {UserId = userId});

			return new EducationProgressDataViewModel
			{
				Items = response.Items.Select(model => new EducationProgressTaskViewModel
				{
					Tutorial = model.Tutorial,
					Unit = model.Unit,
					Task = model.Task,
					Value = model.Value,
					Date = model.Date
				}).ToArray(),
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

			CommonGrpcResponse saveResponse = await _educationProgressService.ChangeTaskDate(new ChangeTaskDateGrpcRequest
			{
				UserId = userId,
				Tutorial = tutorial,
				Unit = unit,
				Task = task,
				Date = date.Value
			});

			return saveResponse.IsSuccess
				? new EducationProgressChangeDateDataViewModel()
				: EducationProgressChangeDateDataViewModel.Error($"Error (3) occured while setting new date to tutorial/unit/task {tutorial}/{unit}/{task} for user {userId}");
		}
	}
}