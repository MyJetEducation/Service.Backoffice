using MyJetWallet.Sdk.Postgres;
using Service.Backoffice.Postgres;

namespace Service.EmailSender.Postgres.DesignTime
{
	public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
	{
		public ContextFactory() : base(options => new DatabaseContext(options))
		{
		}
	}
}