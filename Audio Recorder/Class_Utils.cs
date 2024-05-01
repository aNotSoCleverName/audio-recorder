using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio_Wrapper
{
    internal static class Class_Utils
    {
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
