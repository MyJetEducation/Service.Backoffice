using MudBlazor;
using Service.Backoffice.Models;

namespace Service.Backoffice.Services
{
	public interface IUserTokenAccountDataService
	{
		ValueTask<UserTokenAccountDataViewModel> GetOperations(string userId, DateTime? dateFrom, DateTime? dateTo, int? movement, int? source, int? productType, TableState tableState);
	}
}