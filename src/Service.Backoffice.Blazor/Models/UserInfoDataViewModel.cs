namespace Service.Backoffice.Blazor.Models
{
	public class UserInfoDataViewModel: ViewModelBase
	{
		public UserInfoDataViewModel(string errorText) : base(errorText)
		{
		}

		public UserInfoDataViewModel()
		{
		}

		public ParamValue[] UserInfoItems { get; set; }

		public ParamValue[] ProfileItems { get; set; }
	}
}