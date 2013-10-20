using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;

using NAudio.Dsp;

namespace MusicRecognitionClassLibrary
{
    public class mp3Processor
    {
        public static int maxLen = 80000000;

        public static byte[] getByteArray(string file, ref int len, ref int channels)
        {
            byte[] buf = new byte[maxLen];                                                    
            WaveStream mp3Reader = new Mp3FileReader(file);
            channels = mp3Reader.WaveFormat.Channels;
            int read = mp3Reader.Read(buf, 0, buf.Length);
            len = read;
            return buf;            
        }       

    }
}
