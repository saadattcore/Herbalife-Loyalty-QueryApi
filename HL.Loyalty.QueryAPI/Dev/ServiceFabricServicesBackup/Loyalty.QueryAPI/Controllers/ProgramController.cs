using System.Web.Http;
using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.ProgramProvider;
using HL.Loyalty.Providers.QueryAPI;

namespace Loyalty.QueryAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class ProgramController : ApiController
    {
        private IProgramProvider _provider;

        public ProgramController(IProgramProvider provider)
        {
            _provider = provider;
        }

        public IHttpActionResult Get(int id)
        {
            return Ok();
        }

        public WrapperResult<ProgramModel> Get(string DistributorId, string Locale)
        {

            WrapperResult<ProgramModel> wrapperResult = _provider.GetProgram(DistributorId, Locale);
            return wrapperResult;
        }

    }
}