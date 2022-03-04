using Service.Backoffice.Blazor.Models;
using Service.Education.Structure;

namespace Service.Backoffice.Blazor.Services
{
	public interface IEducationProgressService
	{
		ValueTask<EducationProgressDataViewModel> GetProgress(string email);

		ValueTask<EducationProgressDataViewModel> ClearProgress(string email, EducationTutorial? tutorial, int? unit, int? task);

		ValueTask ClearAll(string email, ClearProgressFlags clear);
		
		ValueTask<EducationProgressChangeDateDataViewModel> ChangeTaskDate(string email, EducationTutorial changeTutorial, int changeUnit, int changeTask, DateTime? changeDate);
	}
}