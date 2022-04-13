using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IUserResolver
	{
		ValueTask<ParamValue[]> GetUsers(string searchStr);
	}
}