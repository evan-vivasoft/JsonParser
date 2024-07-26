using Autofac;
using JSONParser.InspectionProcedure;
using JSONParser.RequestHandler;
using JSONParser.StationInformation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONParser.InformationService
{
    internal class InformationService : IInformationService, IDisposable
    {
        #region Member variables and getters
        private IRequestHandler _requestHandler;
        private bool disposedValue;

        private IRequestHandler RequestHandler
        {
            get
            {
                if (_requestHandler is null)
                {
                    _requestHandler = new RequestHandler.RequestHandler(new System.Net.Http.HttpClient());
                }

                return _requestHandler;
            }
        }

        private string StationInformationUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("StationInformationAPIUrl");
            }
        }

        private string StationInfoFile
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("StationInformationJsonDir");
            }
        }

        private string InspectionProcedureFile
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("InspectionProcedureJsonDir");
            }
        }

        private string InspectionProcedureUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("InspectionProcedureAPIUrl");
            }
        }

        #endregion

        #region Member Methods
        private void CreateFileIfNotExist(string fileName)
        {
            if (!File.Exists(fileName))
            {
                System.IO.File.Create(fileName);
            }
        }
        #endregion

        #region Interface Implementation
        public async Task StoreStationInformation(string baseUrl, Dictionary<string, string> reqHeader)
        {
            try
            {
                CreateFileIfNotExist(StationInfoFile);

                string prsData = await RequestHandler.GetAsyncString(baseUrl.TrimEnd('/') + ":8000/" + StationInformationUrl, reqHeader);

                using (StreamWriter writer = new StreamWriter(StationInfoFile))
                {
                    writer.WriteLine(prsData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task StoreInspectionProcedure(string baseUrl, Dictionary<string, string> reqHeader)
        {
            try
            {
                CreateFileIfNotExist(InspectionProcedureFile);

                string ipData = await RequestHandler.GetAsyncString(baseUrl.TrimEnd('/') + ":8000/" + InspectionProcedureUrl, reqHeader);

                using (StreamWriter writer = new StreamWriter(InspectionProcedureFile))
                {
                    writer.WriteLine(ipData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Dispose Object
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
        // ~InformationService()
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
