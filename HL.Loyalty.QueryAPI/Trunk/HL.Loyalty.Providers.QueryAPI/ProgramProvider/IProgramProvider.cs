using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI.ProgramProvider
{
    public interface IProgramProvider
    {

        WrapperResult<ProgramModel> GetProgram(string distributorId, string countryCode);

    }
}
