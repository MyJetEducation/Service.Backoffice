namespace Service.Backoffice.Blazor.Models
{
	public class UserTokenAccountDataViewModel: ViewModelBase
	{
		public UserTokenAccountDataViewModel(string errorText) : base(errorText)
		{
		}

		public UserTokenAccountDataViewModel()
		{
		}

		public ParamValue[] UserFilter { get; set; }
		
		public int TotalItems { get; set; }

		public UserTokenAccountDataOperationViewModel[] Operations { get; set; }
	}
}