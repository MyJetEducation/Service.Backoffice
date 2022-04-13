using Service.EducationProgress.Domain.Models;

namespace Service.Backoffice.Models
{
	public class EducationProgressDataViewModel : ViewModelBase
	{
		public EducationProgressDataViewModel(string errorText) : base(errorText)
		{
		}

		public EducationProgressDataViewModel()
		{
		}

		public int TaskScore { get; set; }
		public int TestScore { get; set; }

		public EducationProgressDto[] Items { get; set; }
	}
}