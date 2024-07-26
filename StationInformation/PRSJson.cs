using System.Collections.Generic;

namespace JSONParser.StationInformation
{
    internal class PRSJson
    {
        public string id { get; set; }
        public string name { get; set; }
        public string identification_code { get; set; }
        public string information { get; set; }
        public string database_code { get; set; }
        public Route route { get; set; }
        public InspectionProcedure inspection_procedure { get; set; }
        public List<GasControlLine> gas_control_lines { get; set; }
    }

    internal class Route
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    internal class InspectionProcedure
    {
        public string id { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    internal class GasControlLine
    {
        public string id { get; set; }
        public string name { get; set; }
        public string identification_code { get; set; }
        public string psd_volume { get; set; }
        public string ssd_volume { get; set; }
        public string pe_min { get; set; }
        public string pe_max { get; set; }
        public string pd_range { get; set; }
        public string pu_range { get; set; }
        public int gcl_number { get; set; }
        public int? start_position { get; set; }
        public InspectionProcedure inspection_procedure { get; set; }
        public List<Boundary> boundaries { get; set; }
    }

    internal class Boundary
    {
        public string script_command_id { get; set; }
        public double? minimum_value { get; set; }
        public double? maximum_value { get; set; }
        public string uom { get; set; }
        public double offset { get; set; }
        public string inspection_point_id { get; set; }
        public string object_id { get; set; }
        public string measure_point_id { get; set; }
    }
}
