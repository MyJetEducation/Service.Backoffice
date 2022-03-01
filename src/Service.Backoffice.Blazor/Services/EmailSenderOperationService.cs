using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Backoffice.Blazor.Models;
using Service.Backoffice.Postgres;
using Service.Core.Client.Extensions;
using Service.EmailSender.Domain.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class EmailSenderOperationService : IEmailSenderOperationService
	{
		private readonly ILogger<EmailSenderOperationService> _logger;
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

		public EmailSenderOperationService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<EmailSenderOperationService> logger)
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
					.ToArrayAsync();

				return new EmailSenderOperationDataViewModel
				{
					Items = entities.Select(e =>
					{
						string datetime = $"{e.Date:dd.MM.yyyy}{Environment.NewLine}{e.Date:HH:mm:ss}";
						string content = e.Value;
						string value = FormatValue(content);
						string url = GetUrl(content);
						return new EmailSenderOperationParamValue(datetime, value, url);
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

			if (email is not {Subject: "Registration Confirm"}) 
				return null;

			HashEmailDataModel hashData = ((JObject) email.Data).ToObject<HashEmailDataModel>();
			if (hashData == null)
				return null;

			return $"https://web.dfnt.work/register-confirm?hash={hashData.Hash}";
		}

		private static string FormatValue(string value) => value.IsNullOrWhiteSpace()
			? string.Empty
			: JToken.Parse(value).ToString(Formatting.Indented);
	}
}