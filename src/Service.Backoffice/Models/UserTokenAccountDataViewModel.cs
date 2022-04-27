namespace Service.Backoffice.Models
{
	public class UserTokenAccountDataViewModel: ViewModelBase
	{
		public UserTokenAccountDataViewModel(string errorText) : base(errorText)
		{
		}

		public UserTokenAccountDataViewModel()
		{
		}

		public int TotalItems { get; set; }

		public string UserName { get; set; }
		public decimal? UserTotal { get; set; }
		public string UserId { get; set; }

		public UserTokenAccountDataOperationViewModel[] Operations { get; set; }
	}
}