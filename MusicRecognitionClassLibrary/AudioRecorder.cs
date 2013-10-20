using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Mixer;
using System.IO;

using NAudio.Dsp;

namespace MusicRecognitionClassLibrary
{
    public class AudioRecorder
    {
        public bool isRecording;
        public WaveFileWriter writer;
        public WaveIn waveIn;
        public WaveFormat recordingFormat;
    
        public byte[] b;
        public int bLen;

        private int device;        
        private const int mxSize = 50000000;

        public static string[] getAvailibleDevices()
        {
            int waveInDevices = WaveIn.DeviceCount;
            string[] res = new string[waveInDevices];
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                res[waveInDevice] = "Device " + waveInDevice.ToString() + ":" + deviceInfo.ProductName + "," + deviceInfo.Channels.ToString() + " channels";
            }
            return res;
        }      

        public AudioRecorder(int device)
        {           
            isRecording = false;
            recordingFormat = new WaveFormat(44100, 1);         

            this.device = device;
        }

        public void StartRecording()
        {
            if (isRecording)
                return;

            b = new byte[mxSize];
            bLen = 0;

            waveIn = new WaveIn();
            waveIn.DeviceNumber = device;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += waveIn_RecordingStopped;
            waveIn.WaveFormat = recordingFormat;
            waveIn.StartRecording();
            
            isRecording = true;

            //writer = new WaveFileWriter("demo.wav", recordingFormat);           
        }

        public void StopRecording()
        {
            if (isRecording)
            waveIn.StopRecording();
            isRecording = false;
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            isRecording = false;
            waveIn.Dispose();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {         
            for(int i = 0; i<e.BytesRecorded; i++)
            {
                b[bLen] = e.Buffer[i];
                bLen++;
            }           
        }

        public byte[] getByteArray(ref int len)
        {
            len = bLen;
            return b;
        }     
       
    }
}
