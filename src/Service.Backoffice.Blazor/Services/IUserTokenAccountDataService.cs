using MudBlazor;
using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IUserTokenAccountDataService
	{
		ValueTask<UserTokenAccountDataViewModel> GetOperations(string userId, DateTime? dateFrom, DateTime? dateTo, int? movement, int? source, int? productType, TableState tableState);
	}
}