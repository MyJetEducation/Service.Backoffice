using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.EmailSender.Domain.Models;

namespace Service.Backoffice.Postgres
{
	public class DatabaseContext : MyDbContext
	{
		public const string Schema = "education";
		private const string OperationsTableName = "operations";

		public DatabaseContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<OperationEntity> Operations { get; set; }

		public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
		{
			MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

			return new DatabaseContext(options.Options);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(Schema);

			SetUserInfoEntityEntry(modelBuilder);

			base.OnModelCreating(modelBuilder);
		}

		private static void SetUserInfoEntityEntry(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OperationEntity>().ToTable(OperationsTableName);
			modelBuilder.Entity<OperationEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
			modelBuilder.Entity<OperationEntity>().Property(e => e.Date).IsRequired();
			modelBuilder.Entity<OperationEntity>().Property(e => e.Value);
			modelBuilder.Entity<OperationEntity>().HasKey(e => e.Id);
		}
	}
}