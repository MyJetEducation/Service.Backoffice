using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Backoffice.Blazor.Models;
using Service.Backoffice.Postgres;
using Service.Core.Domain.Extensions;

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
						string value = FormatValue(e.Value);
						return new ParamValue(datetime, value);
					}).ToArray()
				};
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);

				return new EmailSenderOperationDataViewModel(exception.Message);
			}
		}

		private static string FormatValue(string value) => value.IsNullOrWhiteSpace()
			? string.Empty
			: JToken.Parse(value).ToString(Formatting.Indented);
	}
}