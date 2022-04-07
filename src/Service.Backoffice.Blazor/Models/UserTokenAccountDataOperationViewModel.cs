namespace Service.Backoffice.Blazor.Models
{
	public class UserTokenAccountDataOperationViewModel
	{
		public string UserName { get; set; }

		public string UserId { get; set; }

		public DateTime Date { get; set; }

		public string Movement { get; set; }

		public string Source { get; set; }

		public string ProductType { get; set; }

		public string Value { get; set; }

		public string Info { get; set; }

		public bool IsIncome { get; set; }
		
		public bool ShowJson { get; set; }

		public decimal? Total { get; set; }
	}
}