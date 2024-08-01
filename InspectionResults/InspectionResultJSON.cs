using System;
using System.Collections.Generic;

namespace Inspector.POService.InspectionResults
{
    internal class InspectionResultJSON
    {
        public string status {  get; set; }
        public Guid prs_id { get; set; }
        public string prs_name { get; set; }
        public string prs_identification { get; set; }
        public string prs_code { get; set; }
        public Guid gcl_id { get; set; }
        public string gas_control_line_name { get; set; }
        public string gcl_identification { get; set; }
        public string gcl_code { get;set; }
        public Guid inspection_procedure_id {  get; set; }
        public string inspection_procedure_name { get;set; }
        public string inspection_procedure_version { get; set; }
        public MeasurementEquipmentJson measurement_equipment { get; set; }
        public string start_date { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string time_zone { get; set; }
        public string dst {  get; set; }
        public int? start_position { get; set; }
        public int amount_measurements { get; set; }

        public List<ResultObj> results { get; set; }
    }

    internal class ResultObj
    {
        public Guid script_command_id { get; set; }
        public string object_id { get; set; }
        public string measure_point_id { get; set; }
        public double? maximum_value { get; set; }
        public double? minimum_value { get; set; }
        public double? offset { get; set; }
        public string uom { get; set; }
        public string time { get; set; }
        public MeasureValueType measure_value { get; set; }
        public string text { get; set; }
        public List<string> lists { get; set; }
        public FPRType fpr_data { get; set; }
    }

    internal class FPRType
    {
        public double sample_rate { get; set; }
        public double interval { get; set; }
        public int count_total { get; set; }
        public DateTime start_at { get; set; }
        public DateTime? end_at { get; set; }
        public string uom { get; set; }
        public List<MeasuredValues> data { get; set; }
        public List<MeasuredValues> extra_data { get; set; }
    }

    internal class MeasuredValues
    {
        public double value { get; set; }
        public DateTime time { get; set; }
        public string extra_data { get; set; }
    }

    internal class MeasureValueType
    {
        public double value { get; set; }
        public string uom { get; set; }
    }
    internal class MeasurementEquipmentJson
    {
        public MenometerType id_dm1 { get; set; }
        public MenometerType id_dm2 { get; set; }
        public string bt_address { get; set; }
    }

    internal class MenometerType
    {
        public string type { get; set; }
        public string serial_number { get; set; }
    }
}