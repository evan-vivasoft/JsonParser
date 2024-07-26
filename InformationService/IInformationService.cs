using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser.InformationService
{
    public interface IInformationService : IDisposable
    {
        Task StoreStationInformation(string baseUrl, Dictionary<string, string> reqHeader);
        Task StoreInspectionProcedure(string baseUrl, Dictionary<string, string> reqHeader);
    }
}
