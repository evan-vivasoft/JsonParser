using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace JSONParser.StationInformation
{
    internal class PRSEntityAdapter:IDisposable
    {
        private string[] _allowedPressureValues = {
            "0..25 mbar",
            "0..70 mbar",
            "0..70 mbar",
            "0..200 mbar",
            "0..300 mbar",
            "0..500 mbar",
            "0..1000 mbar",
            "0..1100 mbar",
            "0..2000 mbar",
            "0..7500 mbar",
            "0..10 bar",
            "0..17 bar",
            "0..35 bar",
            "0..90 bar"
        };

        private string[] _allowedUOVUnits = {
            "-",
            "mbar",
            "bar",
            "Pa",
            "hPa",
            "kPa",
            "MPa",
            "kg/cm2",
            "kg/m2",
            "mmHg",
            "cmHg",
            "mHg",
            "inHg",
            "mmH2o",
            "cmH2o",
            "mH2o",
            "inH2o",
            "ftH2o",
            "psi",
            "lb/in2",
            "lb/ft2",
            "torr",
            "atm",
            "mbar/min",
            "Pa/min",
            "(kg/cm2)/min",
            "mmHg/min",
            "inHg/min",
            "mmH2o/min",
            "inH2o/min",
            "ftH2o/min",
            "(lb/in2)/min",
            "(lb/ft2)/min",
            "dm3/h",
            "ft3/h",
            "m3/h",
            "l/h",
            "gph",
            "in3/h",
            "°C",
            "°F",
            "°K",
            "°Re"
        };


        private readonly List<PRSJson> _allPrs = new List<PRSJson>();
        private bool disposedValue;

        public PRSEntityAdapter(string jsonLine) 
        {
            try
            {
                this._allPrs = JsonSerializer.Deserialize<List<PRSJson>>(jsonLine);
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't parse prs information. {e.Message}");
            }
        }

        private bool IsJSONDataValid
        {
            get
            {
                List<GasControlLine> allGasControlLines = 
                    this._allPrs.SelectMany(prs => prs.gas_control_lines).ToList();
            
                var maybeInvalidGCL = 
                    allGasControlLines
                    .Where(gcl => 
                        Array.IndexOf(this._allowedPressureValues, gcl.pd_range) == -1
                        || Array.IndexOf(this._allowedPressureValues, gcl.pu_range) == -1)
                    .ToList();

                if (maybeInvalidGCL != null && maybeInvalidGCL.Count > 0)
                {
                    string invalidGCLJson = JsonSerializer.Serialize(maybeInvalidGCL[0]);
                    throw new DataException($"Invalid pd_range/pu_range provided. Given invalid gcl: {invalidGCLJson}");
                }

                var maybeInvalidUOV =
                    allGasControlLines
                    .SelectMany(gcl => gcl.boundaries)
                    .ToList()
                    .Where(boundary => boundary.uom != null && Array.IndexOf(this._allowedUOVUnits, boundary.uom) == -1)
                    .ToList();
            
                if (maybeInvalidUOV != null && maybeInvalidUOV.Count > 0)
                {
                    string invalidUOV = JsonSerializer.Serialize(maybeInvalidUOV[0]);
                    throw new DataException($"Invalid pd_range/pu_range provided. Given invalid gcl: {invalidUOV}");
                }

                return true;
            }
        }

        public List<PRSEntityJson> GetPRSEntityListForPRSJson
        {
            get
            {
                if (this.IsJSONDataValid)
                {
                    return (
                        this._allPrs
                            .Select(prs => this.GetPRSEntityByJSON(prs))
                            .ToList()
                    );
                }
                return null;
            }
        }

        private PRSEntityJson GetPRSEntityByJSON(PRSJson jsonObject)
        {
            if (jsonObject == null)
            {
                throw new ArgumentNullException("Null jsonObject provided");
            }

            PRSEntityJson prsEntity = new PRSEntityJson
            {
                Id = Guid.Parse(jsonObject.id),
                Route = jsonObject.route?.name,
                PRSCode = jsonObject.database_code,
                PRSName = jsonObject.name,
                PRSIdentification = jsonObject.identification_code,
                Information = jsonObject.information,
                InspectionProcedure = jsonObject.inspection_procedure?.name,
                GasControlLines = new List<GasControlLineEntityJson>(),
                InspectionProcedureId = jsonObject.inspection_procedure != null ? Guid.Parse(jsonObject.inspection_procedure.id) : Guid.Empty
            };

            if (jsonObject.gas_control_lines != null && jsonObject.gas_control_lines.Any())
            {
                prsEntity.GasControlLines =
                    jsonObject
                    .gas_control_lines
                    .Select(gcl => 
                    new GasControlLineEntityJson
                    {
                        Id = Guid.Parse(gcl.id),
                        PRSName = prsEntity.PRSName,
                        PRSIdentification = gcl.identification_code,
                        GasControlLineName = gcl.name,
                        PeMin = gcl.pe_min,
                        PeMax = gcl.pe_max,
                        VolumeVA = gcl.psd_volume,
                        VolumeVAK = gcl.ssd_volume,
                        PaRangeDM = Helper.GetEnumValueFromDescription<TypeRangeDMJson>(gcl.pd_range.Replace(" ", "")),
                        PeRangeDM = Helper.GetEnumValueFromDescription<TypeRangeDMJson>(gcl.pu_range.Replace(" ", "")),
                        GCLIdentification = gcl.identification_code,
                        GCLCode = jsonObject.database_code,
                        InspectionProcedure = gcl.inspection_procedure?.name,
                        InspectionProcedureId = gcl.inspection_procedure != null ? Guid.Parse(gcl.inspection_procedure.id) : Guid.Empty,
                        StartPosition = gcl.start_position,
                        GCLObjects = this.GetGCLObjectsByBoundary(gcl.boundaries)
                    })
                    .ToList();
            }

            return prsEntity;
        }

        private List<GCLObjectJson> GetGCLObjectsByBoundary(List<Boundary> boundaries)
        {
            if (boundaries.Count == 0) return null;
            if (boundaries == null)
            {
                throw new ArgumentNullException("Null boundaries provided");
            }

            return (
                boundaries
                .Where(boundary => boundary != null)
                .Select(boundary => new GCLObjectJson
                {
                    ObjectID = "",
                    ObjectName = "",
                    MeasurePoint = "",
                    MeasurePointID = boundary.measure_point_id ?? "",
                    InspectionPointId = Guid.Parse(boundary.inspection_point_id),
                    Boundaries = new TypeObjectIDBoundariesJson
                    {
                        ValueMax = boundary.maximum_value ?? double.NaN,
                        ValueMin = boundary.minimum_value ?? double.NaN,
                        UOV =  !string.IsNullOrEmpty(boundary.uom) ? Helper.GetEnumValueFromDescription<UnitOfMeasurementJson>(boundary.uom) : UnitOfMeasurementJson.UNSET,
                        ScriptCommandId = Guid.Parse(boundary.script_command_id),
                        Offset = boundary.offset
                    }
                })
                .ToList()
            );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~JSONToEntityConverter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
