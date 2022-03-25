using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IEmailSenderOperationDataService
	{
		public ValueTask<EmailSenderOperationDataViewModel> GetOperationData();
	}
}