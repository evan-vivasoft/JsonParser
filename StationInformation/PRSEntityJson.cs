using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JSONParser.StationInformation
{
    public class PRSEntityJson
    {
        public Guid Id { get; set; }
        public string Route { get; set; }
        public string PRSCode { get; set; }
        public string PRSName { get; set; }
        public string PRSIdentification { get; set; }
        public string Information { get; set; }
        public string InspectionProcedure { get; set; }

        public Guid InspectionProcedureId { get; set; }

        public List<PRSObjectJson> PRSObjects { get; set; }
        public List<GasControlLineEntityJson> GasControlLines { get; set; }

        public PRSEntityJson()
        {
            this.PRSObjects = new List<PRSObjectJson>();
            this.GasControlLines = new List<GasControlLineEntityJson>();
        }
    }
    public enum UnitOfMeasurementJson
    {
        /// <summary>
        /// Value not set
        /// </summary>
        [Description("Unset")]
        UNSET,

        [Description("-")]
        [System.Xml.Serialization.XmlEnumAttribute("-")]
        Item,

        [Description("mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar")]
        ItemMbar,

        [Description("bar")]
        [System.Xml.Serialization.XmlEnumAttribute("bar")]
        ItemBar,

        [Description("Pa")]
        [System.Xml.Serialization.XmlEnumAttribute("Pa")]
        ItemPa,

        [Description("hPa")]
        [System.Xml.Serialization.XmlEnumAttribute("hPa")]
        ItemhPa,

        [Description("kPa")]
        [System.Xml.Serialization.XmlEnumAttribute("kPa")]
        ItemkPa,

        [Description("MPa")]
        [System.Xml.Serialization.XmlEnumAttribute("MPa")]
        ItemMPa,

        [Description("kg/cm2")]
        [System.Xml.Serialization.XmlEnumAttribute("kg/cm2")]
        Itemkgcm2,

        [Description("kg/m2")]
        [System.Xml.Serialization.XmlEnumAttribute("kg/m2")]
        Itemkgm2,

        [Description("mmHg")]
        [System.Xml.Serialization.XmlEnumAttribute("mmHg")]
        ItemmmHg,

        [Description("cmHg")]
        [System.Xml.Serialization.XmlEnumAttribute("cmHg")]
        ItemcmHg,

        [Description("mHg")]
        [System.Xml.Serialization.XmlEnumAttribute("mHg")]
        ItemmHg,

        [Description("inHg")]
        [System.Xml.Serialization.XmlEnumAttribute("inHg")]
        IteminHg,

        [Description("mmH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("mmH2o")]
        ItemmmH2o,

        [Description("cmH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("cmH2o")]
        ItemcmH2o,

        [Description("mH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("mH2o")]
        ItemmH2o,

        [Description("inH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("inH2o")]
        IteminH2o,

        [Description("ftH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("ftH2o")]
        ItemftH2o,

        [Description("psi")]
        [System.Xml.Serialization.XmlEnumAttribute("psi")]
        Itempsi,

        [Description("lb/in2")]
        [System.Xml.Serialization.XmlEnumAttribute("lb/in2")]
        Itemlbin2,

        [Description("lb/ft2")]
        [System.Xml.Serialization.XmlEnumAttribute("lb/ft2")]
        Itemlbft2,

        [Description("torr")]
        [System.Xml.Serialization.XmlEnumAttribute("torr")]
        Itemtorr,

        [Description("atm")]
        [System.Xml.Serialization.XmlEnumAttribute("atm")]
        Itematm,

        [Description("mbar/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar/min")]
        ItemMbarMin,

        [Description("Pa/min")]
        [System.Xml.Serialization.XmlEnumAttribute("Pa/min")]
        ItemPamin,

        [Description("(kg/cm2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(kg/cm2)/min")]
        Itemkgcm2min,

        [Description("mmHg/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mmHg/min")]
        ItemmmHgmin,

        [Description("inHg/min")]
        [System.Xml.Serialization.XmlEnumAttribute("inHg/min")]
        IteminHgmin,

        [Description("mmH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mmH2o/min")]
        ItemmmH2omin,

        [Description("inH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("inH2o/min")]
        IteminH2omin,

        [Description("ftH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("ftH2o/min")]
        ItemftH2omin,

        [Description("(lb/in2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(lb/in2)/min")]
        Itemlbin2min,

        [Description("(lb/ft2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(lb/ft2)/min")]
        Itemlbft2min,

        [Description("dm3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("dm3/h")]
        ItemDm3h,

        [Description("ft3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("ft3/h")]
        Itemft3h,

        [Description("m3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("m3/h")]
        Itemm3h,

        [Description("l/h")]
        [System.Xml.Serialization.XmlEnumAttribute("l/h")]
        Itemlh,

        [Description("gph")]
        [System.Xml.Serialization.XmlEnumAttribute("gph")]
        Itemgph,

        [Description("in3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("in3/h")]
        Itemin3h,

        [Description("°C")]
        [System.Xml.Serialization.XmlEnumAttribute("°C")]
        ItemC,

        [Description("°F")]
        [System.Xml.Serialization.XmlEnumAttribute("°F")]
        ItemF,

        [Description("°K")]
        [System.Xml.Serialization.XmlEnumAttribute("°K")]
        ItemK,

        [Description("°Re")]
        [System.Xml.Serialization.XmlEnumAttribute("°Re")]
        ItemRe
    }

    /// <summary>
    /// TypeRangeDM
    /// </summary>
    public enum TypeRangeDMJson
    {
        /// <summary>
        /// Value not set
        /// </summary>
        [Description("Unset")]
        UNSET,

        /// <summary>
        /// No value or unknown value
        /// </summary>
        [Description("-")]
        [System.Xml.Serialization.XmlEnumAttribute("-")]
        Item,

        [Description("0..25 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..25mbar")]
        Item025mbar,

        [Description("0..70 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..70mbar")]
        Item070mbar,

        [Description("0..70 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..70mbar")]
        Item070mbar1,

        [Description("0..200 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..200mbar")]
        Item0200mbar,

        [Description("0..300 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..300mbar")]
        Item0300mbar,

        [Description("0..500 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..500mbar")]
        Item0500mbar,

        [Description("0..1000 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..1000mbar")]
        Item01000mbar,

        [Description("0..1100 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..1100mbar")]
        Item01100mbar,

        [Description("0..2000 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..2000mbar")]
        Item02000mbar,

        [Description("0..7500 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..7500mbar")]
        Item07500mbar,

        [Description("0..10 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..10bar")]
        Item010bar,

        [Description("0..17 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..17bar")]
        Item017bar,

        [Description("0..35 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..35bar")]
        Item035bar,

        [Description("0..90 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..90bar")]
        Item090bar,
    }
    /// <summary>
    /// PRSPRSObjects
    /// </summary>
    public class PRSObjectJson
    {
        public string ObjectName { get; set; }
        public string ObjectID { get; set; }
        public string MeasurePoint { get; set; }
        public string MeasurePointID { get; set; }
        public int? FieldNo { get; set; }
        public Guid InspectionPointId { get; set; }
    }

    public class GasControlLineEntityJson
    {
        public Guid Id { get; set; }
        public string PRSName { get; set; }
        public string PRSIdentification { get; set; }
        public string GasControlLineName { get; set; }
        public string PeMin { get; set; }
        public string PeMax { get; set; }
        public string VolumeVA { get; set; }
        public string VolumeVAK { get; set; }
        public TypeRangeDMJson PaRangeDM { get; set; }
        public TypeRangeDMJson PeRangeDM { get; set; }
        public string GCLIdentification { get; set; }
        public string GCLCode { get; set; }
        public string InspectionProcedure { get; set; }
        public Guid InspectionProcedureId { get; set; }
        public int? StartPosition { get; set; }
        public int FSDStart { get; set; }
        public List<GCLObjectJson> GCLObjects;

        public GasControlLineEntityJson()
        {
            this.GCLObjects = new List<GCLObjectJson>();
        }
    }

    /// <summary>
    /// TypeObjectID
    /// </summary>
    public class GCLObjectJson
    {
        public string ObjectName { get; set; }
        public string ObjectID { get; set; }
        public string MeasurePoint { get; set; }
        public string MeasurePointID { get; set; }
        public int? FieldNo { get; set; }

        public Guid InspectionPointId { get; set; }
        public TypeObjectIDBoundariesJson Boundaries { get; set; }

        public GCLObjectJson()
        {
        }
    }

    /// <summary>
    /// TypeObjectIDBoundaries
    /// </summary>
    public class TypeObjectIDBoundariesJson
    {
        public double ValueMax { get; set; }
        public double ValueMin { get; set; }
        public UnitOfMeasurementJson UOV { get; set; }
        public Guid? ScriptCommandId { get; set; }
        public double? Offset { get; set; }
    }
}
