using Autofac;
using JSONParser.InspectionProcedure;
using JSONParser.InspectionResults;
using JSONParser.InspectionResults.Model;
using JSONParser.PlexorInformation;
using JSONParser.RequestHandler;
using JSONParser.StationInformation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JSONParser.InformationManager
{
    public class InformationManager: IInformationManager
    {
        private static readonly Lazy<IInformationManager> _instance = new Lazy<IInformationManager>(() => new InformationManager());
        public static IInformationManager Instance => _instance.Value;
        private Dictionary<Guid, Measurement> _linkIdToMeasurementData;
        private List<InspectionResult> _resultEntity;
        private IRequestHandler _requestHandler;

        private MeasurementReport _measurementReport;
        private InformationManager() 
        {
            _linkIdToMeasurementData = new Dictionary<Guid, Measurement>();
            _resultEntity = new List<InspectionResult>();
        }

        public List<InspectionProcedureEntityJsonParserProject> GetInspectionProcedure
        {
            get
            {
                var jsonLine = "";
                CreateFileIfNotExist(ConfigurationManager.AppSettings.Get("InspectionProcedureJsonPath"));
                using (StreamReader reader = new StreamReader(ConfigurationManager.AppSettings.Get("InspectionProcedureJsonPath")))
                {
                    jsonLine = reader.ReadToEnd();
                }
                jsonLine =
                       string.IsNullOrEmpty(jsonLine)
                           ? "[]"
                           : jsonLine;
                using (var adapter = new InspectionProcedureAdapter(jsonLine))
                {
                    return adapter.GetJsonToXml;
                };
            }
        }

        public async Task<List<InspectionProcedureEntityJsonParserProject>> GetInspectionProcedures()
        {
            var response =
                await HttpRequestHanlder.GetAsync<List<InspectionProcedureEntityJsonParserProject>>("http://api.plexor.online:8000/api/v1/inspector-device/");
            return response.ToList();
        }
        private IRequestHandler HttpRequestHanlder
        {
            get
            {
                return _requestHandler ?? Program.Container.Resolve<IRequestHandler>();
            }
        }

        public List<PRSEntityJson> GetPRSInformation
        {
            get
            {
                CreateFileIfNotExist(ConfigurationManager.AppSettings.Get("StationInformationJsonPath"));
                var jsonLine = "";
                using (StreamReader reader = new StreamReader(ConfigurationManager.AppSettings.Get("StationInformationJsonPath")))
                {
                    jsonLine = reader.ReadToEnd();
                }
                jsonLine =
                       string.IsNullOrEmpty(jsonLine)
                           ? "[]"
                           : jsonLine;
                using (var adapter = new PRSEntityAdapter(jsonLine))
                {
                    return adapter.GetPRSEntityListForPRSJson;
                }
            }
        }

        public List<PlexorEntity> GetPlexorInformation
        {
            get
            {
                var jsonLine = "";
                CreateFileIfNotExist(ConfigurationManager.AppSettings.Get("PlexorInformationJsonPath"));
                using (StreamReader reader = new StreamReader(ConfigurationManager.AppSettings.Get("PlexorInformationJsonPath")))
                {
                    jsonLine = reader.ReadToEnd();
                }
                jsonLine =
                       string.IsNullOrEmpty(jsonLine)
                           ? "[]"
                           : jsonLine;
                using (var adapter = new PlexorDeviceEntityAdapter(jsonLine))
                {
                    return adapter.GetPlexorEntityListFromJson;
                }
            }
        }

        private void SaveInspectionResultWithMeasurementData()
        {
            List<InspectionResultJSON> savedResults = GetExistingSavedResults;
            
            var result = 
                new ResultEntityAdapter(_resultEntity)
                .GetResultJSONFromResultIPC
                .ToList();

            savedResults.AddRange(result);

            try
            {
                using (StreamWriter sw = new StreamWriter(ConfigurationManager.AppSettings.Get("ResultJsonFilePath"), false, Encoding.UTF8))
                {
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true,
                    };
                    options.Converters.Add(new DoubleInfinityConverter());
                    sw.Write(JsonSerializer.Serialize(savedResults, options));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while converting result and measurement data to json object");
                Console.WriteLine(ex.ToString());

                throw new InvalidDataException($"Message: {ex.Message }; Source: {ex.Source}; StackTrace: {ex.StackTrace}");
            }
        }

        internal class DoubleInfinityConverter : JsonConverter<double>
        {
            public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetDouble();

            public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    writer.WriteNullValue();
                    return;
                }
                writer.WriteNumberValue(value);
            }
        }

        public void StoreInspectionResult(List<InspectionResult> resultReport)
        {
            _resultEntity =
                resultReport
                .Select(r => r)
                .Where(r => r.Status == (int) InspectionStatus.Completed)
                .ToList();
        }

        public void StoreMeasurementResult (MeasurementReport measurementReport)
        {
            _measurementReport = measurementReport;
            _measurementReport.Measurements.ForEach(measurement =>
            {
                if (_linkIdToMeasurementData.ContainsKey(measurement.LinkId))
                {
                    _linkIdToMeasurementData[measurement.LinkId] = measurement;
                }
                else
                {
                    _linkIdToMeasurementData.Add(measurement.LinkId, measurement);
                }
            });
        }

        public void StoreFullResultWithMeasurement()
        {
            _resultEntity
                .ForEach(r =>
                {
                    r.Results.ForEach(singleResult =>
                    {
                        var linkId = singleResult.LinkId;
                        _linkIdToMeasurementData.TryGetValue(linkId, out Measurement measurement);
                        var maybeFprData = GetFPRDataFromMeasurementValue(measurement);
                        singleResult.FprData = maybeFprData;
                    });
                });
            SaveInspectionResultWithMeasurementData();
        }

        private FPRData GetFPRDataFromMeasurementValue(Measurement measurement)
        {
            return new FPRData
            {
                SampleRate = measurement.SampleRate,
                Interval = measurement.Interval,
                CountTotal = measurement.Data.MeasurementValues.Count + measurement.Data.ExtraMeasurementValues.Count,
                StartAt = measurement.Data.StartTime,
                EndAt = measurement.Data.EndTime,
                Uom = Helper.GetEnumValueFromDescription<UnitOfMeasurement>(measurement.Data.Unit),
                Data = measurement.Data.MeasurementValues,
                ExtraData = measurement.Data.ExtraMeasurementValues
            };
        }

        private List<InspectionResultJSON> GetExistingSavedResults
        {
            get
            {
                var jsonLine = "";

                try
                {
                    using (StreamReader reader = new StreamReader(ConfigurationManager.AppSettings.Get("ResultJsonFilePath")))
                    {
                        jsonLine = reader.ReadToEnd();
                    }

                    jsonLine =
                        string.IsNullOrEmpty(jsonLine)
                            ? "[]"
                            : jsonLine;

                    return JsonSerializer.Deserialize<List<InspectionResultJSON>>(jsonLine);
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(ex.Message);
                }
            }
        }

        private void CreateFileIfNotExist(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
        }
    }
}
