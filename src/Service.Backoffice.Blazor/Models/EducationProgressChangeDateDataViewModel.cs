namespace Service.Backoffice.Blazor.Models
{
	public class EducationProgressChangeDateDataViewModel
	{
		public bool Result { get; set; }

		public string ErrorMessage { get; set; }

		public EducationProgressChangeDateDataViewModel(string errorMessage)
		{
			ErrorMessage = errorMessage;
			Result = false;
		}

		public EducationProgressChangeDateDataViewModel()
		{
			Result = true;
		}
	}
}