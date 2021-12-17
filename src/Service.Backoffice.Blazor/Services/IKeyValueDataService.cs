using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IKeyValueDataService
	{
		public Task<KeyValueDataViewModel> GetKeyValueData(string email);
	}
}