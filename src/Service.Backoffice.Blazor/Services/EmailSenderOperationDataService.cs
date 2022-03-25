using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Backoffice.Blazor.Models;
using Service.Backoffice.Postgres;
using Service.Core.Client.Extensions;
using Service.EmailSender.Domain.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class EmailSenderOperationDataService : IEmailSenderOperationDataService
	{
		private readonly ILogger<EmailSenderOperationDataService> _logger;
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

		public EmailSenderOperationDataService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<EmailSenderOperationDataService> logger)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
			_logger = logger;
		}

		public async ValueTask<EmailSenderOperationDataViewModel> GetOperationData()
		{
			try
			{
				var entities = await DatabaseContext.Create(_dbContextOptionsBuilder)
					.Operations
					.OrderByDescending(entity => entity.Date)
					.Select(entity => new {entity.Date, entity.Value})
					.Take(20)
					.ToArrayAsync();

				return new EmailSenderOperationDataViewModel
				{
					Items = entities.Select(e =>
					{
						string datetime = $"{e.Date:dd.MM.yyyy}{Environment.NewLine}{e.Date:HH:mm:ss}";
						string content = e.Value;
						string value = FormatValue(content);
						string url = GetUrl(content);
						return new EmailSenderOperationDataParamValue(datetime, value, url);
					}).ToArray()
				};
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);

				return new EmailSenderOperationDataViewModel(exception.Message);
			}
		}

		private static string GetUrl(string contents)
		{
			var email = JsonConvert.DeserializeObject<EmailModel>(contents);

			string addr = null;
			switch (email?.Subject)
			{
				case "Registration Confirm":
					addr = "register-confirm";
					break;
				case "Recovery Password":
					addr = "password-recovery";
					break;
				case "Change Email":
					addr = "change-email";
					break;
			}

			if (addr == null)
				return null;

			var hashData = ((JObject) email.Data).ToObject<HashEmailDataModel>();

			return hashData == null
				? null
				: $"https://web.dfnt.work/{addr}?hash={hashData.Hash}";
		}

		private static string FormatValue(string value) => value.IsNullOrWhiteSpace()
			? string.Empty
			: JToken.Parse(value).ToString(Formatting.Indented);
	}
}