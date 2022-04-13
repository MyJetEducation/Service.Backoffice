using Service.Backoffice.Models;
using Service.Core.Client.Extensions;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;
using Service.PersonalData.Grpc.Models;

namespace Service.Backoffice.Services
{
	public class UserResolver : IUserResolver
	{
		private readonly IPersonalDataServiceGrpc _personalDataServiceGrpc;

		public UserResolver(IPersonalDataServiceGrpc personalDataServiceGrpc) => _personalDataServiceGrpc = personalDataServiceGrpc;

		public async ValueTask<ParamValue[]> GetUsers(string searchStr)
		{
			if (searchStr.IsNullOrWhiteSpace())
				return Array.Empty<ParamValue>();

			PersonalDataBatchResponseContract response = await _personalDataServiceGrpc.SearchAsync(new SearchRequest
			{
				SearchText = searchStr
			});

			IEnumerable<PersonalDataGrpcModel> personalDatas = response?.PersonalDatas;

			return personalDatas == null
				? Array.Empty<ParamValue>()
				: response.PersonalDatas.Select(model => new ParamValue(model.Id, model.Email))
					.DistinctBy(value => value.Param)
					.ToArray();
		}
	}
}