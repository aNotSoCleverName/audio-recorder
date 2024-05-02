using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Security.Cryptography.X509Certificates;

namespace NAudio_Wrapper
{
    /// <summary>
    /// This class simply starts/stops recording of both speaker and mic at once
    /// Once recording is stopped, combine the resulting WAVs into one WAV
    /// </summary>
    public static class Class_SpeakerAndMic
    {
        const string tempSpeakerResultPath = ".\\tempSpeaker.wav";
        const string tempMicResultPath = ".\\tempMic.wav";

        static string resultPath = "";

        public static void StartRecording()
        {
            Class_Speaker.StartRecording();
            Class_Mic.StartRecording();
        }

        public static void StopRecording(string inResultPath)
        {
            resultPath = Class_Encoding.GivePathExtension(".wav", inResultPath);

            Class_Speaker.StopRecording(tempSpeakerResultPath);
            Class_Mic.StopRecording(tempMicResultPath);

            CombineSpeakerAndMicWav();
        }

        private static void CombineSpeakerAndMicWav()
        {
            AudioFileReader? speakerReader = TryReadAudioFile(tempSpeakerResultPath);
            AudioFileReader? micReader = TryReadAudioFile(tempMicResultPath);

            // If all null, do nothing
            if (speakerReader == null && micReader == null)
                return;
            // If none null, mix
            else if (speakerReader != null && micReader != null)
            {
                MixingSampleProvider mixer = new MixingSampleProvider(new[] { speakerReader, micReader });
                WaveFileWriter.CreateWaveFile16(resultPath, mixer);

                speakerReader.Close();
                micReader.Close();

                File.Delete(tempSpeakerResultPath);
                File.Delete(tempMicResultPath);
                return;
            }

            speakerReader?.Close();
            micReader?.Close();

            // If one null, rename non-null
            File.Delete(resultPath);
            if (speakerReader == null)
                File.Move(tempMicResultPath, resultPath);
            else
                File.Move(tempSpeakerResultPath, resultPath);

            File.Delete(tempSpeakerResultPath);
            File.Delete(tempMicResultPath);
        }

        private static AudioFileReader? TryReadAudioFile(string inPath)
        {
            try
            {
                return new AudioFileReader(inPath);
            }
            catch
            {
                return null;
            }
        }

    //Private Function CreateWaveFormat(inBitRate As Module_ActionType.Enum_AudioBitRate) As WaveFormat
    //    'Dim sampleRate As Integer = 44100
    //    Dim sampleRate As Integer = 48000
    //    Dim bitDepth As Integer
    //    Dim channel As Integer = 2

    //    Select Case inBitRate
    //        Case Module_ActionType.Enum_AudioBitRate.Low
    //            bitDepth = 16
    //        Case Module_ActionType.Enum_AudioBitRate.Medium
    //            bitDepth = 24
    //        Case Module_ActionType.Enum_AudioBitRate.High
    //            bitDepth = 32
    //    End Select

    //    Return New WaveFormat(sampleRate, bitDepth, channel)
    //End Function
    }
}
