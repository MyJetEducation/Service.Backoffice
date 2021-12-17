﻿namespace Service.Backoffice.Blazor.Models
{
	public class KeyValueDataViewModel : ViewModelBase
	{
		public KeyValueDataViewModel(string errorText) : base(errorText)
		{
		}

		public KeyValueDataViewModel()
		{
		}

		public ParamValue[] Items { get; set; }
	}
}