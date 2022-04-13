using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IRetryDataService
	{
		ValueTask<RetryDataViewModel> GetData(string userId);

		ValueTask<RetryDataViewModel> SetCount(string userId, int value);

		ValueTask<RetryDataViewModel> SetDate(string userId, DateTime? value);
	}
}