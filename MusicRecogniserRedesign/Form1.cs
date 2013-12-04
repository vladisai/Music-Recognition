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
using NAudio.Dsp;

namespace MusicRecogniserRedesign
{
    public partial class Form1 : Form
    {
        private MatchFinder matchFinder;
        private AudioRecorder audioRecorder;

        private byte[] b;
        private int bLen;
        private int channels;

        private Timer timer;

        public Form1()
        {
            InitializeComponent();
            matchFinder = new MatchFinder("data\\");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] devices = AudioRecorder.getAvailibleDevices();
            for (int i = 0; i < devices.Length; i++)
            {
                comboBox1.Items.Add(devices[i]);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {            
            if (audioRecorder != null && audioRecorder.isRecording)
            {
                if (timer != null)
                    timer.Stop();
                audioRecorder.StopRecording();
               
                bLen = audioRecorder.bLen;
                channels = 1;
                b = audioRecorder.b;
                audioRecorder = null;

                button1.Text = "Начать запись и распознавание";               
            }
            else
            {
                audioRecorder = new AudioRecorder(comboBox1.SelectedIndex);
                audioRecorder.StartRecording();       

                timer = new Timer();

                timer.Interval = 1000;
                timer.Tick += timer_Tick;

                timer.Start();

                viewMatches();

                button1.Text = "Остановить запись и распознавание";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (audioRecorder != null)
            {
                audioRecorder.StopRecording();
                audioRecorder = null;
            }
            listBox1.Items.Clear(); 
        }

        private void viewMatches()
        {

            if (audioRecorder != null && audioRecorder.isRecording)
            {
                bLen = audioRecorder.bLen;
                channels = 1;
                b = audioRecorder.b;
            }

            string[] res = matchFinder.getBestMatches(b, bLen, channels, 15);

            listBox1.Items.Clear();
            for (int i = 0; i < res.Length; i++)
            {
                listBox1.Items.Add(res[i]);
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
    }
}
