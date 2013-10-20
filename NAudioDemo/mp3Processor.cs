using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;

using NAudio.Dsp;

namespace NAudioDemo
{
    class mp3Processor
    {
        private Byte[] buf;        
        private int len;

        private Complex[] comData;
        private int comLen;

        private const int maxLen = 100000000;

        public mp3Processor(string file)
        {
            buf = new byte[maxLen];                                                    

            WaveStream mp3Reader = new Mp3FileReader(file);
            int read = mp3Reader.Read(buf, 0, buf.Length);
            len = read;            
        }

        public void processData(ref Complex[][] res, ref int resLen)
        {

            resLen = 0;

           
            int channels = 2;
            int pbs = 4096*2*channels;
            
            for(int i = 0; i<len/pbs; i++)
            {

                comData = new Complex[10000];
                comLen = 0;

                for (int j = i*pbs, cur = 0; j < (i+1)*pbs; j+=2*channels, cur++)
                {
                    float sum = 0;
                    for (int k = 0; k < channels; k++)
                        sum += (buf[j + 2 * k + 1] << 8) | (buf[j + 2 * k]);
                    sum /= channels;
                    comData[comLen].X = sum;
                    comData[comLen].X *= (float)FastFourierTransform.HammingWindow(cur, 4096);
                    comData[comLen].Y = 0;                   
                    comLen++;
                }
             
                FastFourierTransform.FFT(true, 12, comData);
                                    
                res[resLen] = comData;
                resLen++;
            }
            resLen++;
            resLen--;
        }
               
    }
}
