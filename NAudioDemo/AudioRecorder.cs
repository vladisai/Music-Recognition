using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Mixer;
using System.IO;

using NAudio.Dsp;

namespace NAudioDemo
{
    class AudioRecorder
    {
        public bool isRecording;
        public WaveFileWriter writer;
        public WaveIn waveIn;
        public WaveFormat recordingFormat;
    
        public Complex[][] spectData;
        public int spLen;
       

        public void GetData(ref Complex[][] d, ref int l)
        {
            d = spectData;
            l = spLen;
        }

        public AudioRecorder()
        {
            isRecording = false;
            recordingFormat = new WaveFormat(44100, 1);

            spectData = new Complex[1000000][];
        }

        public void StartRecording()
        {
            if (isRecording)
                return;

            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += waveIn_RecordingStopped;
            waveIn.WaveFormat = recordingFormat;
            waveIn.StartRecording();
            
            isRecording = true;

            writer = new WaveFileWriter("demo.wav", recordingFormat);           
        }

        public void StopRecording()
        {
            if (isRecording)
            waveIn.StopRecording();
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            isRecording = false;
            writer.Dispose();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;            
            int bytesRecorded = e.BytesRecorded;
            
            WriteToFile(buffer, bytesRecorded);

            updateSpectData(buffer, bytesRecorded);
        }

        private void WriteToFile(byte[] buffer, int bytesRecorded)
        {
            long maxFileLength = this.recordingFormat.AverageBytesPerSecond*60;

            int toWrite = (int) Math.Min(maxFileLength - writer.Length, bytesRecorded);
            if (toWrite > 0)
            {
                writer.WriteData(buffer, 0, bytesRecorded);
            }
            else
            {
                StopRecording();
            }
        }

        private void updateSpectData(byte[] b, int l)
        {
            Complex[] comData = new Complex[10000];

            int p = 0;
            int k = 1;


            int len = 0;
            for (int i = 0; i < l; i += 2)
            {
                comData[len].X = (b[i+1] << 8) | (b[i]);
                comData[len].X *= (float)FastFourierTransform.HammingWindow(i/2, 4410);
                len++;
            }

            while (k < len)
            {
                p++;
                k *= 2;
            }

            FastFourierTransform.FFT(true, p, comData);

            spectData[spLen] = comData;
            spLen++;
        }
    }
}
