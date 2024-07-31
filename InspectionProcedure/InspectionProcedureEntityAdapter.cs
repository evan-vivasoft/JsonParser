using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JSONParser.InspectionProcedure.ScriptCommandFactory;

namespace JSONParser.InspectionProcedure
{
    internal class InspectionProcedureAdapter: IDisposable
    {
        #region Member Variables
        private readonly List<InspectionProcedureJson> _inspectionProcedureJson;
        private bool disposedValue;
        #endregion

        #region Constructor
        public InspectionProcedureAdapter(string jsonStr) 
        {
            try
            {
                this._inspectionProcedureJson = JsonSerializer.Deserialize<List<InspectionProcedureJson>>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't parse inspection procedure. {ex.Message}");
            }
        }
        #endregion

        public List<InspectionProcedureEntityJsonParserProject> GetJsonToXml
        {
            get
            {
                List<InspectionProcedureEntityJsonParserProject> XmlData = new List<InspectionProcedureEntityJsonParserProject>();
                foreach (var item in this._inspectionProcedureJson)
                {
                    var xmlItem = GetXMLItemFromJsonItem(item);
                    XmlData.Add(xmlItem);
                }

                return XmlData;
            }
        }

        #region Helper Methods
        private InspectionProcedureEntityJsonParserProject GetXMLItemFromJsonItem(InspectionProcedureJson jsonData)
        {
            InspectionProcedureEntityJsonParserProject item = new InspectionProcedureEntityJsonParserProject
            {
                Name = jsonData.name,
                Version = jsonData.version,
                InspectionSequence = new List<ScriptCommandEntityBase>()
            };
            int sequenceNumberToAdd = 0;
            foreach (var section in jsonData.sections)
            {
                var sectionName = section.name;
                var sequenceNumber = section.sequence;

                if (section.sub_sections.Count > 0)
                {
                    // then we will get a list of script command list inside script_commands property
                    foreach (var subSectionList in section.sub_sections)
                    {
                        var scriptCommand2 = new ScriptCommand2Entity
                        {
                            SequenceNumber = sequenceNumber + sequenceNumberToAdd,
                            Section = sectionName,
                            SubSection = subSectionList.name
                        };

                        if (subSectionList.script_commands.Any())
                        {
                            item.InspectionSequence.Add(scriptCommand2);
                            // it will be a list of script command list

                            subSectionList.script_commands
                                .SelectMany(commands => commands)
                                .Select(command => ScriptCommanFactoryProvider.GetScriptCommandEntity(command, ++sequenceNumberToAdd))
                                .ToList()
                                .ForEach(item.InspectionSequence.Add);
                        }
                    }
                }
            }
            return item;
        }
        #endregion

        #region Dispose
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
        // ~InspectionProcedureAdapter()
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
