using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspector.POService.PlexorInformation
{
    public class PlexorJson
    {
        public string name { get; set; }
        public string serial_number { get; set; }
        public string bluetooth_address { get; set; }
        public string pn {  get; set; }
        public DateTime calibration_date { get; set; }
    }
}
