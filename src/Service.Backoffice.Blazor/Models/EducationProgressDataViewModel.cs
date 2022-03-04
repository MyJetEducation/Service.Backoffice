using Service.EducationProgress.Domain.Models;

namespace Service.Backoffice.Blazor.Models
{
	public class EducationProgressDataViewModel : ViewModelBase
	{
		public EducationProgressDataViewModel(string errorText) : base(errorText)
		{
		}

		public EducationProgressDataViewModel()
		{
		}

		public int TotalProgress { get; set; }

		public EducationProgressDto[] Items { get; set; }
	}
}