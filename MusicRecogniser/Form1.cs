using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using MusicRecognitionClassLibrary;
using NAudio.Dsp;

namespace MusicRecogniser
{
    public partial class Form1 : Form
    {
        private MatchFinder matchFinder;
        private AudioRecorder audioRecorder;

        private byte[] b;
        private int bLen;
        private int channels;

        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();  
            matchFinder = new MatchFinder("data\\");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Add("mp3 file");
            string[] devices = AudioRecorder.getAvailibleDevices();
            for(int i = 0; i<devices.Length; i++)
            {
                listBox1.Items.Add(devices[i]);
            }
            listBox1.SetSelected(0, true);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (audioRecorder != null)
            {            
                audioRecorder.StopRecording();
                audioRecorder = null;
            }

            if (listBox1.SelectedIndex == 0)
            {
                panel1.Visible = true;
                panel2.Visible = false;                
            }
            else
            {
                panel2.Visible = true;
                panel1.Visible = false;                
            }
            
            listBox2.Items.Clear();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (audioRecorder != null && audioRecorder.isRecording)
            {
                if (timer != null)
                    timer.Stop();

                timer = new System.Windows.Forms.Timer();

                timer.Interval = 1000;
                timer.Tick += timer_Tick;

                timer.Start();
            }
            else
            {
                viewMatches();
            }
        }

        private void viewMatches()
        {
            if (listBox1.SelectedIndex == 0)
            {
                bLen = 0;
                channels = 0;
                b = mp3Processor.getByteArray(textBox1.Text, ref bLen, ref channels);
            }

            if (audioRecorder != null && audioRecorder.isRecording)
            {
                bLen = audioRecorder.bLen;
                channels = 1;
                b = audioRecorder.b;
            }

            string[] res = matchFinder.getBestMatches(b, bLen, channels, 15);           

            listBox2.Items.Clear();
            for (int i = 0; i < res.Length; i++)
            {
                listBox2.Items.Add(res[i]);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            viewMatches();

            if (listBox1.SelectedIndex == 0 || audioRecorder == null)
                timer.Stop();
            else
            {
                bLen = audioRecorder.bLen;
                b = audioRecorder.b;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            audioRecorder = new AudioRecorder(listBox1.SelectedIndex-1);
            audioRecorder.StartRecording();
            button3.Enabled = false;
            button2.Enabled = true;
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {            
            audioRecorder.StopRecording();
            if (timer != null)
                timer.Stop();
            bLen = audioRecorder.bLen;
            channels = 1;
            b = audioRecorder.b;
            button3.Enabled = true;
            button2.Enabled = true;
            audioRecorder = null;
            button4.Enabled = false;
        }
    }
}
