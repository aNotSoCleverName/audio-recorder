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
                EqualizeAudioFiles(ref speakerReader, ref micReader);

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

        private static void EqualizeAudioFiles(ref AudioFileReader inAudioReader1, ref AudioFileReader inAudioReader2)
        {
            // Don't equalize if already equal
            if (inAudioReader1.WaveFormat.SampleRate == inAudioReader2.WaveFormat.SampleRate &&
                inAudioReader1.WaveFormat.BitsPerSample == inAudioReader2.WaveFormat.BitsPerSample &&
                inAudioReader1.WaveFormat.Channels == inAudioReader2.WaveFormat.Channels)
            {
                return;
            }


            // Get minimum format
            int sampleRate = Math.Min(inAudioReader1.WaveFormat.SampleRate, inAudioReader2.WaveFormat.SampleRate);
            int bitDepth = Math.Min(inAudioReader1.WaveFormat.BitsPerSample, inAudioReader2.WaveFormat.BitsPerSample);
            int channelCount = Math.Min(inAudioReader1.WaveFormat.Channels, inAudioReader2.WaveFormat.Channels);
            WaveFormat minWaveFormat = new WaveFormat(sampleRate, bitDepth, channelCount);

            string tempUnequalizedPath1 = inAudioReader1.FileName;
            string tempUnequalizedPath2 = inAudioReader2.FileName;

            string tempEqualizedPath1 = ".\\equalized1.wav";
            string tempEqualizedPath2 = ".\\equalized2.wav";

            ResampleAudioFile(inAudioReader1, minWaveFormat, tempEqualizedPath1);
            ResampleAudioFile(inAudioReader2, minWaveFormat, tempEqualizedPath2);

            // Delete unequalized files, then rename equalized files to the name that unequalized files had.
            {
                inAudioReader1.Close();
                inAudioReader2.Close();

                File.Delete(tempUnequalizedPath1);
                File.Delete(tempUnequalizedPath2);

                File.Move(tempEqualizedPath1, tempUnequalizedPath1);
                File.Move(tempEqualizedPath2, tempUnequalizedPath2);

                inAudioReader1 = new AudioFileReader(tempUnequalizedPath1);
                inAudioReader2 = new AudioFileReader(tempUnequalizedPath2);
            }
        }

        private static void ResampleAudioFile(AudioFileReader inAudioReader, WaveFormat inTargetWaveFormat, string inTargetFilePath)
        {
            MediaFoundationResampler resampler = new MediaFoundationResampler(inAudioReader, inTargetWaveFormat);
            WaveFileWriter.CreateWaveFile(inTargetFilePath, resampler);
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
    }
}
