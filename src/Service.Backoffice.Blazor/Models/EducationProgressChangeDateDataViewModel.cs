namespace Service.Backoffice.Blazor.Models
{
	public class EducationProgressChangeDateDataViewModel
	{
		public bool Result { get; set; }

		public string ErrorMessage { get; set; }

		public EducationProgressChangeDateDataViewModel()
		{
			Result = true;
		}

		public static EducationProgressChangeDateDataViewModel Error(string errorMessage) => new()
		{
			ErrorMessage = errorMessage,
			Result = false
		};
	}
}