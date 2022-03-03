using Newtonsoft.Json;
using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.EducationRetry.Domain.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class RetryService : IRetryService
	{
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public RetryService(IGrpcServiceProxy<IUserInfoService> userInfoService, IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService)
		{
			_userInfoService = userInfoService;
			_serverKeyValueService = serverKeyValueService;
		}

		public async ValueTask<RetryDataViewModel> GetData(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please enter user email");

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new RetryDataViewModel($"No user found by email {email}");

			var countDto = await GetServerKeyValueObject<EducationRetryCountDto>(userId, "education_retry_count");
			var dateDto = await GetServerKeyValueObject<EducationRetryLastDateDto>(userId, "education_retry_lastdate");

			return new RetryDataViewModel
			{
				Count = countDto?.Count ?? 0,
				LastDate = dateDto?.Date
			};
		}

		private async ValueTask<T> GetServerKeyValueObject<T>(Guid? userId, string key) where T : class
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

		public async ValueTask<RetryDataViewModel> SetCount(string email, int value)
		{
			if (email.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please enter user email");

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new RetryDataViewModel($"No user found by email {email}");

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

			if (!result.IsSuccess)
				return new RetryDataViewModel("Error while set retry count.");

			return await GetData(email);
		}

		public async ValueTask<RetryDataViewModel> SetDate(string email, DateTime? value)
		{
			if (value == null)
				return new RetryDataViewModel("Please enter valid date");

			if (email.IsNullOrWhiteSpace())
				return new RetryDataViewModel("Please enter user email");

			Guid? userId = await GetUserId(email);
			if (userId == null)
				return new RetryDataViewModel($"No user found by email {email}");

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

			return await GetData(email);
		}

		private async ValueTask<Guid?> GetUserId(string email)
		{
			UserInfoResponse userInfoResponse = await _userInfoService.Service.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});

			return userInfoResponse?.UserInfo?.UserId;
		}
	}
}