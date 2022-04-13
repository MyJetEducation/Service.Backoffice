using Newtonsoft.Json;
using Service.Backoffice.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.EducationRetry.Domain.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;

namespace Service.Backoffice.Services
{
	public class RetryDataService : IRetryDataService
	{
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public RetryDataService(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService) => _serverKeyValueService = serverKeyValueService;

		public async ValueTask<RetryDataViewModel> GetData(string userId)
		{
			if (userId.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please select user");

			var countDto = await GetServerKeyValueObject<EducationRetryCountDto>(userId, "education_retry_count");
			var dateDto = await GetServerKeyValueObject<EducationRetryLastDateDto>(userId, "education_retry_lastdate");

			return new RetryDataViewModel
			{
				Count = countDto?.Count ?? 0,
				LastDate = dateDto?.Date
			};
		}

		private async ValueTask<T> GetServerKeyValueObject<T>(string userId, string key) where T : class
		{
			string value = (await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = key
			}))?.Value;

			if (value == null)
				return await ValueTask.FromResult((T) null);

			return JsonConvert.DeserializeObject<T>(value);
		}

		public async ValueTask<RetryDataViewModel> SetCount(string userId, int value)
		{
			if (userId.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please select user");

			CommonGrpcResponse result = await _serverKeyValueService.Service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = "education_retry_count",
						Value = JsonConvert.SerializeObject(new EducationRetryCountDto {Count = value})
					}
				}
			});

			return result.IsSuccess
				? await GetData(userId)
				: new RetryDataViewModel("Error while set retry count.");
		}

		public async ValueTask<RetryDataViewModel> SetDate(string userId, DateTime? value)
		{
			if (value == null)
				return new RetryDataViewModel("Please enter valid date");

			if (userId.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please select user");

			CommonGrpcResponse result = await _serverKeyValueService.Service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = "education_retry_lastdate",
						Value = JsonConvert.SerializeObject(new EducationRetryLastDateDto {Date = value})
					}
				}
			});

			if (!result.IsSuccess)
				return new RetryDataViewModel("Error while set retry date.");

			return await GetData(userId);
		}
	}
}