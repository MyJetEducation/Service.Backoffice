using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;

namespace Service.Backoffice.Client
{
	[UsedImplicitly]
	public class BackofficeClientFactory : MyGrpcClientFactory
	{
		public BackofficeClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
		{
		}
	}
}