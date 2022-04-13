using Service.Backoffice.Models;

namespace Service.Backoffice.Services
{
	public interface IUserResolver
	{
		ValueTask<ParamValue[]> GetUsers(string searchStr);
	}
}