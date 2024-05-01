using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    public static class Class_Encoding
    {
        public enum Enum_Encoding
        {
            wav,
            aac,
            mp3,
        }

        /// <summary>
        /// Converts WAV file to other encoding
        /// </summary>
        public static void ConvertWavToOtherEncoding(string inWavFilePath, Enum_Encoding inEncoding)
        {
            MediaFoundationApi.Startup();

            WaveFileReader reader = new WaveFileReader(inWavFilePath);
            string wavFilePathWithoutExtension = Path.GetFullPath(inWavFilePath) + Path.GetFileNameWithoutExtension(inWavFilePath);

            switch (inEncoding)
            {
                case Enum_Encoding.aac:
                    MediaFoundationEncoder.EncodeToWma(reader, wavFilePathWithoutExtension + ".aac");
                    break;
                case Enum_Encoding.mp3:
                    MediaFoundationEncoder.EncodeToMp3(reader, wavFilePathWithoutExtension + ".mp3");
                    break;
                default:
                    throw new Exception("Encoding not supported");
            }
        }

        /// <summary>
        /// Give extension to given path if path has no extension, or extension is different
        /// Example: inTargetExtension = .wav
        ///          inPath = .\\name.wav
        /// Do nothing because inPath already has the right extension
        /// 
        /// Example: inTargetExtension = .wav
        ///          inPath = .\\name.mp3 [OR] .\\name
        /// Add .wav to the path, so it becomes
        /// .\\name.mp3.wav [OR] .\\name.wav
        /// </summary>
        internal static string GivePathExtension(string inTargetExtension, string inPath)
        {
            string resultPath = inPath;
            string extension = Path.GetExtension(inPath);

            if (extension == null ||
                !extension.Equals(inTargetExtension))
            {
                resultPath += extension;
            }

            return resultPath;
        }
    }
}
