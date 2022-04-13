using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.KeyValue.Grpc;
using Service.KeyValue.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class KeyValueDataService : IKeyValueDataService
	{
		private readonly IKeyValueService _keyValueService;

		public KeyValueDataService(IKeyValueService keyValueService)
		{
			_keyValueService = keyValueService;
		}

		public async ValueTask<KeyValueDataViewModel> GetKeyValueData(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new KeyValueDataViewModel("Please enter user email");

			string userId = await GetUserId(email);
			if (userId == null)
				return new KeyValueDataViewModel($"No user found by email {email}");

			KeysGrpcResponse keysResponse = await _keyValueService.GetKeys(new GetKeysGrpcRequest {UserId = userId});
			string[] keys = keysResponse?.Keys;
			if (keys.IsNullOrEmpty())
				return new KeyValueDataViewModel($"No keys found by email {email}");

			ItemsGrpcResponse itemsResponse = await _keyValueService.Get(new ItemsGetGrpcRequest {UserId = userId, Keys = keys});
			KeyValueGrpcModel[] items = itemsResponse?.Items;
			if (items.IsNullOrEmpty())
				return new KeyValueDataViewModel($"No items found by email {email}");

			return new KeyValueDataViewModel
			{
				Items = items?.Select(model => new ParamValue(model.Key, model.Value)).ToArray()
			};
		}

		private async ValueTask<string> GetUserId(string email)
		{
			return null;
		}
	}
}