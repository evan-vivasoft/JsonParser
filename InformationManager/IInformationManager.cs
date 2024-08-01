using Inspector.POService.StationInformation;
using System.Collections.Generic;
using Inspector.POService.InspectionProcedure;
using Inspector.POService.InspectionResults.Model;
using System.Threading.Tasks;
using Inspector.POService.PlexorInformation;

namespace Inspector.POService.InformationManager
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
