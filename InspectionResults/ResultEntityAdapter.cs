using Inspector.POService.InspectionResults.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspector.POService.InspectionResults
{
    internal class ResultEntityAdapter : IDisposable
    {
        #region Member Variables
        private bool disposedValue;

        List<InspectionResult> _resultFromIPC;
        #endregion

        #region Constructor

        public ResultEntityAdapter (List<InspectionResult> resultFromIPC)
        {
            _resultFromIPC = resultFromIPC;
        }
        #endregion

        //<summary>
        //  Get JSON Compatible result processed from Inspector PC
        //</summary>

        public List<InspectionResultJSON> GetResultJSONFromResultIPC
        {
            get
            {
                List<InspectionResultJSON> results= new List<InspectionResultJSON> ();

                foreach (var result in _resultFromIPC)
                {
                    var resultJSON = GetJsonCompatibleInspectionRepository(result);
                    results.Add(resultJSON);
                }
                return results;
            }
        }

        #region Helper methods

        private InspectionResultJSON GetJsonCompatibleInspectionRepository (InspectionResult result)
        {
            return (
                new InspectionResultJSON
                {
                    status = result.Status.ToString() ?? null,
                    prs_id = result.PRSId,
                    prs_identification = result.PRSIdentification,
                    prs_name = result.PRSName,
                    prs_code = result.PRSCode,
                    gas_control_line_name = result.GasControlLineName,
                    gcl_id = result.GCLId,
                    gcl_identification = result.GCLIdentification,
                    gcl_code = result.GCLCode,
                    measurement_equipment = GetMeasurementEquipmentObjFromMeasurementStr(result.Measurement_Equipment),
                    inspection_procedure_name = result.InspectionProcedure.Name,
                    inspection_procedure_id = result.InspectionProcedureId ?? Guid.Empty,
                    inspection_procedure_version = result.InspectionProcedure.Version,
                    start_date = result.DateTimeStamp.StartDate,
                    start_time = result.DateTimeStamp.StartTime,
                    end_time = result.DateTimeStamp.EndTime,
                    time_zone = result.DateTimeStamp.TimeSettings.TimeZone,
                    dst = result.DateTimeStamp.TimeSettings.DST,
                    start_position = result.StartPosition,
                    amount_measurements = result.Results.Count,
                    results = GetResultJSON(result.Results)
                }
            );
        }
        private List<ResultObj> GetResultJSON (List<Result> results)
        {
            List<ResultObj> resultJson = new List<ResultObj> ();

            foreach (Result result in results)
            {
                ResultObj resultObj = GetInspectionResultObj(result);
                resultJson.Add (resultObj);
            }
            return resultJson;
        }

        private ResultObj GetInspectionResultObj(Result result)
        {
            return (
                new ResultObj
                {
                    script_command_id = result.ScriptCommandId ?? Guid.Empty,
                    object_id = result.ObjectID,
                    maximum_value = result.MaximumValue ?? null,
                    minimum_value = result.MinimumValue ?? null,
                    uom = (result.Uom).ToString(),
                    offset = result.Offset ?? null,
                    measure_point_id = result.MeasurePointID ?? null,
                    time = result.Time,
                    text = result.Text,
                    lists = result.List,
                    measure_value = new MeasureValueType
                    {
                        value = result.MeasureValue.Value,
                        uom = (result.MeasureValue.UOM).ToString(),
                    },
                    fpr_data = GetFprCompatibleData(result.FprData)
                }
            );
        }

        private FPRType GetFprCompatibleData(FPRData fpr)
        {
            return new FPRType
            {
                sample_rate = fpr.SampleRate,
                interval = fpr.Interval,
                count_total = fpr.CountTotal,
                start_at = fpr.StartAt,
                end_at = fpr.EndAt,
                uom = (fpr.Uom).ToString(),
                data = GetCompatibleMeasuredValues(fpr.Data),
                extra_data = GetCompatibleMeasuredValues(fpr.ExtraData)
            };
        }

        private List<MeasuredValues> GetCompatibleMeasuredValues (List<MeasurementValue> values)
        {
            return (
                values
                    .Select(v => new MeasuredValues
                    {
                        value = v.Value,
                        time = v.Time,
                        extra_data = v.ExtraData
                    })
                    .ToList()
            );
        }

        private MeasurementEquipmentJson GetMeasurementEquipmentObjFromMeasurementStr (MeasurementEquipment measureEquipment)
        {
            if (measureEquipment == null)
            {
                return null;
            }

            String[] splittedMeasurementStrList = !String.IsNullOrEmpty(measureEquipment.ID_DM1) ? measureEquipment.ID_DM1.Split (',') : new string[] { };

            if (splittedMeasurementStrList.Length != 0 && splittedMeasurementStrList.Length != 3)
            {
                throw new ArgumentException($"Measurement string must be consisted of 3 substrings. Provided dm: {measureEquipment.ID_DM1}");
            }

            MenometerType menometer1 = null;

            if (splittedMeasurementStrList.Length == 3)
            {
                menometer1 = new MenometerType
                {
                    type = splittedMeasurementStrList[0],
                    serial_number = splittedMeasurementStrList[2],
                };
            }

            splittedMeasurementStrList = !String.IsNullOrEmpty(measureEquipment.ID_DM2) ? measureEquipment.ID_DM2.Split(',') : new string[] { };

            if (splittedMeasurementStrList.Length != 0 && splittedMeasurementStrList.Length != 3)
            {
                throw new ArgumentException($"Measurement string must be consisted of 3 substrings. Provided dm: {measureEquipment.ID_DM2}");
            }

            MenometerType menometer2 = null;

            if (splittedMeasurementStrList.Length == 3)
            {
                menometer2 = new MenometerType
                {
                    type = splittedMeasurementStrList[0],
                    serial_number = splittedMeasurementStrList[2],
                };
            }

            return new MeasurementEquipmentJson
            {
                id_dm1 = menometer1,
                id_dm2 = menometer2,
                bt_address = measureEquipment.BT_Address
            };
        }
        #endregion

        #region Dispose Object
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
        // ~ResultEntityAdapter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
