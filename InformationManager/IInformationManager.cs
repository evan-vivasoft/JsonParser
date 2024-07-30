using JSONParser.StationInformation;
using System.Collections.Generic;
using JSONParser.InspectionProcedure;
using JSONParser.InspectionResults.Model;
using System.Threading.Tasks;
using JSONParser.PlexorInformation;

namespace JSONParser.InformationManager
{
    public interface IInformationManager
    {
        List<InspectionProcedureEntityJsonParserProject> GetInspectionProcedure { get; }
        List<PRSEntityJson> GetPRSInformation { get; }
        List<PlexorEntity> GetPlexorInformation { get; }
        void StoreInspectionResult(List<InspectionResult> resultReport);
        void StoreMeasurementResult(MeasurementReport measurementReport);
        void StoreFullResultWithMeasurement();
        Task<List<InspectionProcedureEntityJsonParserProject>> GetInspectionProcedures();
    }
}
