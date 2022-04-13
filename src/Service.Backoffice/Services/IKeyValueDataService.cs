using Service.Backoffice.Models;

namespace Service.Backoffice.Services
{
	public interface IKeyValueDataService
	{
		public ValueTask<KeyValueDataViewModel> GetKeyValueData(string userId);
	}
}