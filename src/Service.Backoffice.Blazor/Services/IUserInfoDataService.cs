using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IUserInfoDataService
	{
		public ValueTask<UserInfoDataViewModel> GetUserData(string email);
	}
}