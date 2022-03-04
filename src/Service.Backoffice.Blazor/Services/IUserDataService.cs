using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IUserDataService
	{
		public ValueTask<UserDataViewModel> GetUserData(string email);
	}
}