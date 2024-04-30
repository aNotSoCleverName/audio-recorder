using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    /// <summary>
    /// This class starts/stops recording sound that comes out of mic
    /// The code for this class is very similar to Class_Speaker
    /// The difference is Class_Speaker requires Class_Silence
    /// </summary>
    public class Class_Mic
    {
        public static bool IsPaused { get; set; }
        public static bool IsMuted { get; set; }
        public static bool IsRecording { get; private set; }

        static string resultPath = "";

        static WaveInEvent? mic;
        static MemoryStream? memoryStream;
        static WaveFileWriter? writer;

        /// <summary>
        /// Returns false if error (probably no mic found)
        /// </summary>
        public static bool StartRecording()
        {
            if (IsRecording) return false;

            mic = new WaveInEvent();
            mic.DataAvailable += OnMicDataAvailable;

            memoryStream = new MemoryStream();
            writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream), mic.WaveFormat);

            try
            {
                mic.StartRecording();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                mic.StopRecording();

                IsRecording = false;
                return false;
            }

            IsRecording = true;
            return true;
        }

        public static void StopRecording(string inResultPath)
        {
            if (mic == null) return;

            resultPath = inResultPath;

            mic.StopRecording();
            IsRecording = false;

            CreateWav();

            writer!.Dispose();
            memoryStream!.Dispose();
            mic.Dispose();
        }

        private static void OnMicDataAvailable(object? sender, WaveInEventArgs e)
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
