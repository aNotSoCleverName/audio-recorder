﻿using NAudio.Wave;
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

        /// <summary>
        /// Stop recording both speaker and mic, and then combine the files. If combined volume is undesired, change the values for volume multiplier parameters.
        /// </summary>
        /// <param name="inResultPath">File path for the final resulting file.</param>
        /// <param name="inSpeakerVolumeMult">Multiplier for speaker volume. Ranges from 0 to 1. Default is 1.</param>
        /// <param name="inMicVolumeMult">Multiplier for mic volume. Ranges from 0 to 1. Default is 1.</param>
        public static void StopRecording(string inResultPath, float inSpeakerVolumeMult = 1.0f, float inMicVolumeMult = 1.0f)
        {
            // Throw error if volume mult is not in 0-1 range.
            if (!(
                Class_Utility.IsVolumeMultValid(inSpeakerVolumeMult) &&
                Class_Utility.IsVolumeMultValid(inMicVolumeMult)
            ))
            {
                throw new Exception("Volume multiplier must be in the range of 0 to 1");
            }

            resultPath = Class_Encoding.GivePathExtension(".wav", inResultPath);

            Class_Speaker.StopRecording(tempSpeakerResultPath);
            Class_Mic.StopRecording(tempMicResultPath);

            CombineSpeakerAndMicWav(inSpeakerVolumeMult, inMicVolumeMult);
        }

        private static void CombineSpeakerAndMicWav(float inSpeakerVolumeMult, float inMicVolumeMult)
        {
            AudioFileReader? speakerReader = TryReadAudioFile(tempSpeakerResultPath);
            AudioFileReader? micReader = TryReadAudioFile(tempMicResultPath);

            // If all null, do nothing
            if (speakerReader == null && micReader == null)
                return;
            // If none null, mix
            else if (speakerReader != null && micReader != null)
            {
                EqualizeAudioFilesWaveFormat(ref speakerReader, ref micReader);

                speakerReader.Volume = inSpeakerVolumeMult;
                micReader.Volume = inMicVolumeMult;

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

        private static void EqualizeAudioFilesWaveFormat(ref AudioFileReader inAudioReader1, ref AudioFileReader inAudioReader2)
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

            Class_Utility.ResampleAudioFile(inAudioReader1, minWaveFormat, tempEqualizedPath1);
            Class_Utility.ResampleAudioFile(inAudioReader2, minWaveFormat, tempEqualizedPath2);

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
