﻿using Service.Backoffice.Blazor.Models;
using Service.Core.Domain.Extensions;
using Service.KeyValue.Grpc;
using Service.KeyValue.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class KeyValueDataService : IKeyValueDataService
	{
		private readonly IUserInfoService _userInfoService;
		private readonly IKeyValueService _keyValueService;

		public KeyValueDataService(IUserInfoService userInfoService, IKeyValueService keyValueService)
		{
			_userInfoService = userInfoService;
			_keyValueService = keyValueService;
		}

		public async Task<KeyValueDataViewModel> GetKeyValueData(string email)
		{
			if (email.IsNullOrWhiteSpace())
				return new KeyValueDataViewModel("Please enter user email");

			return new KeyValueDataViewModel
			{
				Items = new ParamValue[]
				{
					new ParamValue("p1", "value1"),
					new ParamValue("p2", "value2"),
					new ParamValue("p3", "value3"),
					new ParamValue("p4", "value4"),
					new ParamValue("p5", "value3"),
					new ParamValue("p6", "value4"),
					new ParamValue("p7", "value5"),
				}
			};

			UserInfoResponse userInfoResponse = await _userInfoService.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email});
			UserInfoGrpcModel userInfo = userInfoResponse?.UserInfo;
			if (userInfo == null)
				return new KeyValueDataViewModel($"No user found by email {email}");

			Guid? userId = userInfo.UserId;
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
	}
}