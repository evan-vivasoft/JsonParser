
using System;
using System.Collections.Generic;

namespace JSONParser.InspectionResults.Model
{
    public class FPRData
    {
        public double SampleRate { get; set; }
        public double Interval { get; set; }
        public int CountTotal { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public UnitOfMeasurement Uom {  get; set; }
        public List<MeasurementValue> Data { get; set; }
        public List<MeasurementValue> ExtraData { get; set; }
    }
}
