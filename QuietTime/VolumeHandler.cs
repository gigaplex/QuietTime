using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace QuietTime
{
    class VolumeHandler : IDisposable
    {
        private MMDevice audioDevice;

        public VolumeHandler()
        {
            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                audioDevice = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
        }

        public bool Mute
        {
            set {
                if (value != audioDevice.AudioEndpointVolume.Mute)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Setting mute state to {0}", value));
                    audioDevice.AudioEndpointVolume.Mute = value;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Mute state is already {0}", value));
                }
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (audioDevice != null)
                    {
                        audioDevice.Dispose();
                        audioDevice = null;
                    }
                }

                disposedValue = true;
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
