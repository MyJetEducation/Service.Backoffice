using Service.Backoffice.Blazor.Models;

namespace Service.Backoffice.Blazor.Services
{
	public interface IEmailSenderOperationService
	{
		public ValueTask<EmailSenderOperationDataViewModel> GetOperationData();
	}
}