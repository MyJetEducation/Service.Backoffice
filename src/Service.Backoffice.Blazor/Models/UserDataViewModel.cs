namespace Service.Backoffice.Blazor.Models
{
	public class UserDataViewModel: ViewModelBase
	{
		public UserDataViewModel(string errorText) : base(errorText)
		{
		}

		public UserDataViewModel()
		{
		}

		public ParamValue[] UserInfoItems { get; set; }

		public ParamValue[] ProfileItems { get; set; }
	}
}