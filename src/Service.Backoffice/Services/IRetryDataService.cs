using Service.Backoffice.Models;

namespace Service.Backoffice.Services
{
	public interface IRetryDataService
	{
		ValueTask<RetryDataViewModel> GetData(string userId);

		ValueTask<RetryDataViewModel> SetCount(string userId, int value);

		ValueTask<RetryDataViewModel> SetDate(string userId, DateTime? value);
	}
}