using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONParser.PlexorInformation
{
    public class PlexorDeviceEntityAdapter : IDisposable
    {
        private List<PlexorJson> _allPlexorDevice = new List<PlexorJson>();
        private bool disposedValue;

        public PlexorDeviceEntityAdapter(string jsonLine)
        {
            try
            {
                _allPlexorDevice = JsonSerializer.Deserialize<List<PlexorJson>>(jsonLine);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't parse plexor information. {ex.Message}");
            }
        }

        public List<PlexorEntity> GetPlexorEntityListFromJson
        {
            get
            {
                return (
                    _allPlexorDevice
                    .Select(device => GetPlexorEntityFromJson(device))
                    .ToList()
                );
            }
        }

        private PlexorEntity GetPlexorEntityFromJson(PlexorJson plexorJson)
        {
            return new PlexorEntity
            {
                Name = plexorJson.name,
                SerialNumber = plexorJson.serial_number,
                BlueToothAddress = plexorJson.bluetooth_address,
                PN = plexorJson.pn,
                CalibrationDate = plexorJson.calibration_date
            };
        }

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
        // ~PlexorDeviceEntityAdapter()
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
    }
}
