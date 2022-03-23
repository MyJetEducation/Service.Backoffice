using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface ITokenRateDataService
	{
		ValueTask<TokenRateDataViewModel> GetData();

		ValueTask<TokenRateDataViewModel> SetData(decimal value);
	}
}