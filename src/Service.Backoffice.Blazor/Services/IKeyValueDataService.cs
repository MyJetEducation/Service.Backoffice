using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IKeyValueDataService
	{
		public ValueTask<KeyValueDataViewModel> GetKeyValueData(string userId);
	}
}