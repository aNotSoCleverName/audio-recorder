using NAudio.Utils;
using NAudio.Wave;
using NAudio_Wrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    /// <summary>
    /// This class starts/stops recording sound that comes out of speaker
    /// The code for this class is very similar to Class_Mic
    /// The difference is this class requires Class_Silence
    /// </summary>
    public static class Class_Speaker
    {
        public static bool IsPaused { get; set; }
        public static bool IsMuted { get; set; }
        public static bool IsRecording { get; private set; }

        static string resultPath = "";

        static WasapiLoopbackCapture? speaker;
        static MemoryStream? memoryStream;
        static WaveFileWriter? writer;

        /// <summary>
        /// Returns false if error
        /// </summary>
        public static bool StartRecording()
        {
            if (IsRecording) return false;

            speaker = new WasapiLoopbackCapture();
            speaker.DataAvailable += OnSpeakerDataAvailable;

            memoryStream = new MemoryStream();
            writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream), speaker.WaveFormat);

            try
            {
                Class_Silence.PlaySilence(speaker);
                speaker.StartRecording();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                Class_Silence.StopSilence();
                speaker.StopRecording();

                IsRecording = false;
                return false;
            }

            IsRecording = true;
            return true;
        }

        public static void StopRecording(string inResultPath)
        {
            if (speaker == null) return;

            resultPath = inResultPath;

            Class_Silence.StopSilence();
            speaker.StopRecording();
            IsRecording = false;

            CreateWav();

            writer!.Dispose();
            memoryStream!.Dispose();
            speaker.Dispose();
        }

        private static void OnSpeakerDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (writer == null) return;
            if (IsPaused) return;

            byte[] buffer = e.Buffer;
            if (IsMuted)
                buffer = new byte[e.BytesRecorded];
            
            writer.Write(buffer, 0, e.BytesRecorded);
            writer.Flush();
        }

        private static void CreateWav()
        {
            if (memoryStream == null) return;

            using (FileStream fileStream = File.Create(resultPath))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }
        }
    }
}
