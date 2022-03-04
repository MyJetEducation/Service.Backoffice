namespace Service.Backoffice.Blazor.Models
{
	public class ClearProgressFlags
	{
		public bool Progress { get; set; }
		public bool UiProgress { get; set; }
		public bool Achievements { get; set; }
		public bool Statuses { get; set; }
		public bool Habits { get; set; }
		public bool Skills { get; set; }
		public bool Knowledge { get; set; }
		public bool UserTime { get; set; }
		public bool Retry { get; set; }
	}
}