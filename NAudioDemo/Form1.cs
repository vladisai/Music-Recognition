using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicRecognitionClassLibrary;
using NAudio;
using NAudio.Dsp;
using NAudio.Wave;

using NAudioDemo;

namespace NAudioDemo
{
    public partial class Form1 : Form
    {
        private AudioRecorder recorder;
       
        public Form1()
        {
            InitializeComponent();
            recorder = new AudioRecorder(0);                                          
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int inDevices = WaveIn.DeviceCount;
            for (int i = 0; i < inDevices; i++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(i);
                textBox1.AppendText(i.ToString() + ": " + deviceInfo.ProductName +  "\n");                
            }

            recorder.StartRecording();
        }  

   
        private void button2_Click(object sender, EventArgs e)
        {
            recorder.StopRecording();
            int len = 0;
            byte[] b = recorder.getByteArray(ref len);

            Complex[][] data = dataHandler.GetFFTArray(b, len, 1);
            var hash = new Hash(data);

            var s = new spectrogram(spectrogramPicture.Width, spectrogramPicture.Height);
            s.drawSpectrogram(data);
            s.drawPeaks(hash.getPeakPairs());

            spectrogramPicture.Image = s.image;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {                               
                textBox1.AppendText(openFileDialog1.FileName+"\n");
                int bLen = 0, channels = 0;
                byte[] b = mp3Processor.getByteArray(openFileDialog1.FileName, ref bLen, ref channels);
                Complex[][] comData = dataHandler.GetFFTArray(b, bLen, channels);
                var hash = new Hash(comData);

                var s = new spectrogram(spectrogramPicture.Width, spectrogramPicture.Height);
                s.drawSpectrogram(comData);
                s.drawPeaks(hash.getPeakPairs());

                spectrogramPicture.Image = s.image;
            }
        }

        
    }
}
