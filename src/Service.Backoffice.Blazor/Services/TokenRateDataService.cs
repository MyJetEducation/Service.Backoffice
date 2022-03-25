using Service.Backoffice.Blazor.Models;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.TokenRate.Grpc;
using Service.TokenRate.Grpc.Models;

namespace Service.Backoffice.Blazor.Services
{
	public class TokenRateDataService : ITokenRateDataService
	{
		private readonly IGrpcServiceProxy<ITokenRateService> _tokenRateService;

		public TokenRateDataService(IGrpcServiceProxy<ITokenRateService> tokenRateService) => _tokenRateService = tokenRateService;

		public async ValueTask<TokenRateDataViewModel> GetData()
		{
			TokenRateGrpcResponse rate = await _tokenRateService.Service.GetTokenRate();

			decimal? rateValue = rate?.Value;
			if (rateValue == null)
				return new TokenRateDataViewModel("Can't get current token rate value");

			return new TokenRateDataViewModel
			{
				Value = rateValue.Value
			};
		}

		public async ValueTask<TokenRateDataViewModel> SetData(decimal value)
		{
			CommonGrpcResponse result = await _tokenRateService.TryCall(srv => srv.SetTokenRate(new SetTokenRateGrpcRequest
			{
				Value = value
			}));

			if (result?.IsSuccess == false)
				return new TokenRateDataViewModel("Can't set new token rate value");

			return await GetData();
		}
	}
}