using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// Returns error message
        /// </summary>
        public static string ConvertWavToOtherEncoding(string inWavFilePath, Enum_Encoding inTargetEncoding)
        {
            MediaFoundationApi.Startup();

            WaveFileReader reader = new WaveFileReader(inWavFilePath);

            string targetFilePath = "";
            try
            {
                switch (inTargetEncoding)
                {
                    case Enum_Encoding.wav:
                        return "";
                    case Enum_Encoding.aac:
                        targetFilePath = Path.ChangeExtension(inWavFilePath, ".aac");
                        MediaFoundationEncoder.EncodeToWma(reader, targetFilePath);
                        break;
                    case Enum_Encoding.mp3:
                        targetFilePath = Path.ChangeExtension(inWavFilePath, ".mp3");
                        MediaFoundationEncoder.EncodeToMp3(reader, targetFilePath);
                        break;
                    default:
                        throw new Exception("Encoding not supported by DLL");
                }
            }
            catch(COMException)     // Audio codec not installed on computer
            {
                reader.Close();
                File.Delete(targetFilePath);

                MediaFoundationApi.Shutdown();
                return "This computer does not have the codec for: " + inTargetEncoding.ToString() + "\nThe audio is saved as WAV instead.";
            }

            reader.Close();
            File.Delete(inWavFilePath);

            MediaFoundationApi.Shutdown();

            return "";
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
