using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IRetryDataService
	{
		ValueTask<RetryDataViewModel> GetData(string email);

		ValueTask<RetryDataViewModel> SetCount(string email, int value);

		ValueTask<RetryDataViewModel> SetDate(string email, DateTime? value);
	}
}