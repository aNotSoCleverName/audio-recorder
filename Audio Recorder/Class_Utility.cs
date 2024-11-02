using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    internal class Class_Utility
    {
        internal static void ResampleAudioFile(AudioFileReader inAudioReader, WaveFormat inTargetWaveFormat, string inTargetFilePath)
        {
            MediaFoundationResampler resampler = new MediaFoundationResampler(inAudioReader, inTargetWaveFormat);
            WaveFileWriter.CreateWaveFile(inTargetFilePath, resampler);
        }

        internal static bool IsVolumeMultValid(float inVolumeMult)
        {
            return inVolumeMult >= 0.0f && inVolumeMult <= 1.0f;
        }
    }
}
