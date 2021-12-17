using Autofac;

// ReSharper disable UnusedMember.Global

namespace Service.Backoffice.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBackofficeClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BackofficeClientFactory(grpcServiceUrl);
        }
    }
}
