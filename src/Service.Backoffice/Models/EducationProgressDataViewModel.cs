using Service.Education.Structure;

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

		public EducationProgressTaskViewModel[] Items { get; set; }
	}

	public class EducationProgressTaskViewModel
	{
		public EducationTutorial Tutorial { get; set; }

		public int Unit { get; set; }

		public int Task { get; set; }

		public int? Value { get; set; }

		public DateTime? Date { get; set; }

		public bool HasProgress => Value != null;
	}
}