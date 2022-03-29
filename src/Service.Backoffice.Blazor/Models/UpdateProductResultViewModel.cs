namespace Service.Backoffice.Blazor.Models
{
	public class UpdateProductResultViewModel
	{
		public bool Saved { get; set; }

		public string Error { get; set; }

		public static UpdateProductResultViewModel ErrorResult(string error) => new()
		{
			Error = error
		};

		public static UpdateProductResultViewModel SavedResult() => new()
		{
			Saved = true
		};
	}
}