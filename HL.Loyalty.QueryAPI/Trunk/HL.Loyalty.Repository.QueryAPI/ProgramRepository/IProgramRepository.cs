using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Repository.QueryAPI.ProgramRepository
{
    public interface IProgramRepository
    {
        WrapperResult<ProgramModel> GetProgram(string distributorId, string locale);
    }
}
