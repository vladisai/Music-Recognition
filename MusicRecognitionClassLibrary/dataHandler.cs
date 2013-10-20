using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Dsp;

namespace MusicRecognitionClassLibrary
{
    public class dataHandler
    {
        public static int mxLen = 100000;

        public static int bRate = 4096;
             

        public static Complex[][] GetFFTArray(byte[] b, int len, int channels)
        {

            Complex[][] result;

            int step = bRate*2*channels;
            result = new Complex[len / step][];
                 
            for(int i = 0; i+step<=len; i+=step)
            {
                Complex[] comData = new Complex[bRate];
                int comLen = 0;

                for(int j = i, cur = 0; j<i+step; j+=2*channels, cur++)
                {
                    
                    float chSum = 0;
                    for(int k = 0; k<channels; k++)
                    {
                        chSum += (b[j + k*2 + 1] << 8) | (b[j + k*2]);
                    }
                    chSum /= channels;

                    comData[comLen].X = (float)(chSum * FastFourierTransform.HammingWindow(cur, bRate));
                    comData[comLen].Y = 0;
                    comLen++;
                }
              
                FastFourierTransform.FFT(true, 12, comData);

                result[i / step] = comData;
                
            }

            return result;

        }

    }
}
