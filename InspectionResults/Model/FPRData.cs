
using System.Collections.Generic;

namespace JSONParser.InspectionResults.Model
{
    public class FPRData
    {
        public double SampleRate { get; set; }
        public double Interval { get; set; }
        public int CountTotal { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public UnitOfMeasurement Uom {  get; set; }
        public List<MeasurementValue> Data { get; set; }
        public List<MeasurementValue> ExtraData { get; set; }
    }
}
