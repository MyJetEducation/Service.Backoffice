namespace Service.Backoffice.Blazor.Models
{
	public class UidParamValue
	{
		public UidParamValue(Guid? id, string name)
		{
			Id = id;
			Name = name;
		}

		public Guid? Id { get; set; }

		public string Name { get; set; }
	}
}