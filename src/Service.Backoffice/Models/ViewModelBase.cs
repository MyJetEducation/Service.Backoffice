namespace Service.Backoffice.Models
{
	public abstract class ViewModelBase
	{
		protected ViewModelBase()
		{
		}

		protected ViewModelBase(string errorText)
		{
			ErrorText = errorText;
		}

		public string ErrorText { get; set; }
	}
}