using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Extensions;
using Service.KeyValue.Grpc;
using Service.KeyValue.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class KeyValueDataService : IKeyValueDataService
	{
		private readonly IKeyValueService _keyValueService;

		public KeyValueDataService(IKeyValueService keyValueService) => _keyValueService = keyValueService;

		public async ValueTask<KeyValueDataViewModel> GetKeyValueData(string userId)
		{
			if (userId.IsNullOrWhiteSpace())
				return new KeyValueDataViewModel("Please select user");

			KeysGrpcResponse keysResponse = await _keyValueService.GetKeys(new GetKeysGrpcRequest {UserId = userId});
			string[] keys = keysResponse?.Keys;
			if (keys.IsNullOrEmpty())
				return new KeyValueDataViewModel("No keys found for user");

			ItemsGrpcResponse itemsResponse = await _keyValueService.Get(new ItemsGetGrpcRequest {UserId = userId, Keys = keys});
			KeyValueGrpcModel[] items = itemsResponse?.Items;
			if (items.IsNullOrEmpty())
				return new KeyValueDataViewModel("No items found for user");

			return new KeyValueDataViewModel
			{
				Items = items?.Select(model => new ParamValue(model.Key, model.Value)).ToArray()
			};
		}
	}
}