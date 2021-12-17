﻿using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IUserDataService
	{
		public Task<UserDataViewModel> GetUserData(string email);
	}
}