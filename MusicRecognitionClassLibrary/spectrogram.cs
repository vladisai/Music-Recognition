using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Dsp;
using System.Drawing;

namespace MusicRecognitionClassLibrary
{
    public class spectrogram
    {
        
        private Color[] cols;
        public Bitmap image;

        private int originalWidth, originalHeight;

        private int brushWidth,
                    brushHeight,
                    widthStep,
                    heightStep;

        public spectrogram(int width, int height)
        {
            image = new Bitmap(width, height);

            cols = new Color[720];
            for(int i = 0; i<80; i++)
            {
                cols[i] = Color.FromArgb(255, 0, 0, i);
            }
            for(int i = 80; i<128; i++)
            {
                cols[i] = Color.FromArgb(255, i - 80, 0, i);
            }
            for(int i = 128; i<256; i++)
            {
                cols[i] = Color.FromArgb(255, i - 80, 0, 256 - i);
            }
            for(int i = 256; i<456; i++)
            {
                cols[i] = Color.FromArgb(255, Math.Min(255, i - 80), i - 256, 0);
            }
            for(int i = 456; i<456+255; i++)
            {
                cols[i] = Color.FromArgb(255, 255, Math.Min(i - 256, 255), i - 456);
            }
        }

        public void drawSpectrogram(Complex[][] data)
        {         
            Graphics g = Graphics.FromImage(image);

            int width = data.Length;
            if (width == 0)
                return;
            int height = data[0].Length/2;

            originalHeight = height;
            originalWidth = width;

            brushWidth = Math.Max(1, image.Width/width);
            brushHeight = Math.Max(1, image.Height/height);
            widthStep = Math.Max(1, width/image.Width);
            heightStep = Math.Max(1, height/image.Height);

            for(int k = 0, px = 0; k <width; k += widthStep, px += brushWidth)
            {
                for(int i = 0, py = 0; i <height; i += heightStep, py += brushHeight)
                {
                    double sum = 0;                   
                    for (int x = k; x < Math.Min(k + widthStep, width); x++)
                        for (int y = i; y < Math.Min(i + heightStep, height); y++ )
                            sum += Math.Sqrt(data[x][y].X*data[x][y].X + data[x][y].Y*data[x][y].Y);

                    sum /= widthStep*heightStep;
                                     
                    double Magnitude = Math.Log(sum + 1);

                    double step = 9/711.0;

                    int num = Math.Min(719, (int)(Magnitude/step));                                        
                    
                    g.DrawRectangle(new Pen(cols[num]), px, image.Height + 1 - py, brushWidth, brushHeight);
                }
            }
        }

        public void drawPeaks(List<PeaksPair> peaks)
        {
            float stepX = (float) 1.0*brushWidth/widthStep;
            float stepY = (float) 1.0*brushHeight/heightStep;

            Graphics g = Graphics.FromImage(image);

            for(int i = 0; i<peaks.Count; i++)
            {                
                float x1 = peaks[i].position*stepX;
                float y1 = peaks[i].peakFrequency1*stepY;
                float x2 = (peaks[i].position + peaks[i].dTime) * stepX;
                float y2 = peaks[i].peakFrequency2 * stepY;
                g.DrawEllipse(new Pen(Color.Lime, 2), x1+1, image.Height - 1 - y1, 5, 5);
                g.DrawEllipse(new Pen(Color.Lime, 2), x2+1, image.Height - 1 - y2, 5, 5);
                //if (i%10 == 0)
                   //g.DrawLine(new Pen(Color.Magenta, (float)0.5), x1, image.Height - 1 - y1, x2, image.Height - 1 -y2);
            }
        }            
    }
}
