/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.POService.InspectionResults.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class InspectionProcedure
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        public Guid InspectionProcedureId { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="InspectionProcedure"/> class from being created.
        /// Required for XML Serialization
        /// </summary>
        public InspectionProcedure()
        {
            Name = string.Empty;
            Version = string.Empty;
            InspectionProcedureId = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionProcedure"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        public InspectionProcedure(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
