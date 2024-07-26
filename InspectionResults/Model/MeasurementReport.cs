using System;
using System.Collections.Generic;
namespace JSONParser.InspectionResults.Model
{
    /// <summary>
    /// Model representing a measurement result file
    /// </summary>
    public class MeasurementReport
    {
        #region Properties

        /// <summary>
        /// Gets or sets Sample Rate
        /// </summary>
        /// <value>
        /// Sample Rate
        /// </value>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets Interval
        /// </summary>
        /// <value>
        /// Interval
        /// </value>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or sets Measure Count
        /// </summary>
        /// <value>
        /// Total number of measurements
        /// </value>
        public int MeasureCount { get; set; }

        /// <summary>
        /// Gets or sets UOM
        /// </summary>
        /// <value>
        /// Unit of Measurement
        /// </value>
        public UnitOfMeasurement UOM { get; set; }

        /// <summary>
        /// Gets or sets PRSId
        /// </summary>
        /// <value>
        /// PRS id
        /// </value>
        public Guid PRSId { get; set; }

        /// <summary>
        /// Gets or sets GCLId
        /// </summary>
        /// <value>
        /// GCL id
        /// </value>
        public Guid GCLId { get; set; }

        /// <summary>
        /// Gets or sets the measurements.
        /// </summary>
        /// <value>
        /// The measurements.
        /// </value>
        public List<Measurement> Measurements { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        public string PrsName { get; set; }

        /// <summary>
        /// Gets or sets the name of the GCL.
        /// </summary>
        /// <value>
        /// The name of the GCL.
        /// </value>
        public string GclName { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        /// <value>
        /// The start date time.
        /// </value>
        public string StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        /// <value>
        /// The end date time.
        /// </value>
        public string EndDateTime { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>

    }

    public class Data
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum value time stamp.
        /// </summary>
        /// <value>
        /// The maximum value time stamp.
        /// </value>
        public DateTime MaxValueTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>
        /// The measurement values.
        /// </value>
        public List<MeasurementValue> MeasurementValues { get; set; }

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>
        /// The measurement values.
        /// </value>
        public List<MeasurementValue> ExtraMeasurementValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [report io status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [report io status]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportIoStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [maximum value registered].
        /// </summary>
        /// <value>
        /// <c>true</c> if [maximum value registered]; otherwise, <c>false</c>.
        /// </value>
        public bool MaxValueRegistered { get; set; }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="measurementUnit">The measurement unit.</param>
        #endregion Constructors

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
    }

    public class MeasurementValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the io status.
        /// </summary>
        /// <value>
        /// The io status.
        /// </value>
        public int IoStatus { get; set; }

        public DateTime Time { get; set; }
        public string ExtraData { get; set; }
    }

    public class Measurement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Data Data { get; set; }

        /// <summary>
        /// Gets or sets the LinkId.
        /// </summary>
        /// <value>
        /// LinkId. This id is used for mapping result data and measuement data after completion of inspection
        /// </value>
        public Guid LinkId { get; set; }

        /// <summary>
        /// Gets or sets a Sample Rate
        /// </summary>
        /// <value>
        /// Sample Rate
        /// </value>
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a Interval
        /// </summary>
        /// <value>
        /// Interval
        /// </value>
        public double Interval { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
    }
}
