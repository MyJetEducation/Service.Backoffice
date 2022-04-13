using Service.Backoffice.Blazor.Models;
using Service.Education.Structure;

namespace Service.Backoffice.Blazor.Services
{
	public interface IEducationProgressDataService
	{
		ValueTask<EducationProgressDataViewModel> GetProgress(string userId);

		ValueTask<EducationProgressDataViewModel> ClearProgress(string userId, EducationTutorial? tutorial, int? unit, int? task);

		ValueTask ClearAll(string userId, ClearProgressFlags clear);
		
		ValueTask<EducationProgressChangeDateDataViewModel> ChangeTaskDate(string userId, EducationTutorial changeTutorial, int changeUnit, int changeTask, DateTime? changeDate);
	}
}