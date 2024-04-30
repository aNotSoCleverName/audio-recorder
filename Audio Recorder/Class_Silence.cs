using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    /// <summary>
    /// When using WasapiLoopbackCapture to record speaker audio (see Class_Speaker), it stops recording when there's no sound
    /// This class is used to provide silence that keeps WasapiLoopbackCapture running
    /// </summary>
    internal static class Class_Silence
    {
        static WaveOutEvent? silenceOutputDevice;

        internal static void PlaySilence(WasapiLoopbackCapture inSpeaker)
        {
            SilenceProvider silence = new SilenceProvider(inSpeaker.WaveFormat);

            silenceOutputDevice = new WaveOutEvent();
            silenceOutputDevice.PlaybackStopped += OnSilenceStopped;
            silenceOutputDevice.Init(silence.ToSampleProvider());
            silenceOutputDevice.Play();
        }

        internal static void StopSilence()
        {
            if (silenceOutputDevice == null) return;

            silenceOutputDevice.Stop();
        }

        private static void OnSilenceStopped(object? sender, StoppedEventArgs e)
        {
            if (silenceOutputDevice == null) return;

            silenceOutputDevice.Dispose();
            silenceOutputDevice = null;
        }
    }
}
