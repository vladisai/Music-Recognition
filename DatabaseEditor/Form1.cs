using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using MusicRecognitionClassLibrary;
using NAudio.Dsp;


namespace DatabaseEditor
{
    public partial class Form1 : Form
    {       

        private BackgroundWorker bw;

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "Ready";
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] fileNames;
            DialogResult result;
            if (comboBox1.SelectedIndex == 0)           
                result = openFileDialog1.ShowDialog();            
            else           
                result = folderBrowserDialog1.ShowDialog();      
            
            if (result == DialogResult.OK)
            {
                textBox1.Text = "";
                button2.Enabled = true;
                if (comboBox1.SelectedIndex == 0)
                    for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                        textBox1.Text += Path.GetFileName(openFileDialog1.FileNames[i]) + "; \n";
                else
                {
                    textBox1.Text = "Folder: " + Path.GetFileName(folderBrowserDialog1.SelectedPath);                    
                }
            }  
                        
        }

        private void button2_Click(object sender, EventArgs e)
        {

            bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };            
     
            bw.DoWork += DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_Completed;
            bw.RunWorkerAsync(getFileNames());
            

            toolStripStatusLabel1.Text = "Processing...";

            cancelButton.Enabled = true;
        }

        private void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Cancelled";
                progressBar1.Value = 0;               
                statusLabel.Text = "Cancelled";
            }
            else
            {                
                toolStripStatusLabel1.Text = "Complete";
                progressBar1.Value = 100;
                statusLabel.Text = "Done";
                
            }
            cancelButton.Enabled = false;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;            
            statusLabel.Text = e.UserState.ToString();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {            
            var files = (string[])e.Argument;        

            for (int i = 0; i < files.Length; i++)
            {
                if (bw.CancellationPending) { e.Cancel = true; return; }
                bw.ReportProgress((int)(i * 100.0 / files.Length), "Processing song '" + Path.GetFileNameWithoutExtension(files[i]) + "';  " + (i + 1).ToString() + "/" + files.Length.ToString());
                processFile(files[i]);                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Text = "Select " + comboBox1.SelectedItem.ToString();
        }

        private string[] getAllFiles(string path)
        {
            string[] res;            
            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);
            res = files;
            foreach(var folder in folders)
            {
                res = res.Concat(getAllFiles(folder)).ToArray();
            }
            return res;
        }

        private string[] getFileNames()
        {
            string[] res;
            if (comboBox1.SelectedIndex == 0)
            {
                res = openFileDialog1.FileNames;
            }
            else
            {
                res = getAllFiles(folderBrowserDialog1.SelectedPath);                
            }
            return res;
        }     

        private void processFile(string file)
        {                        
            if (Path.GetExtension(file) != ".mp3")
                return;

            string songTitle = Path.GetFileNameWithoutExtension(file);

            int bLen = 0;
            int channels = 0;
            byte[] b = mp3Processor.getByteArray(file, ref bLen, ref channels);
            Complex[][] comData = dataHandler.GetFFTArray(b, bLen, channels);
            Hash h = new Hash(comData);
            List<PeaksPair> peaks = h.getPeakPairs();
            b = null;
            
            string textToWrite = "";

            for (int i = 0; i < peaks.Count; i++)
                textToWrite += peaks[i].peakFrequency1.ToString() + "-" + peaks[i].peakFrequency2.ToString() + "-" +
                                   peaks[i].position + "-" + peaks[i].dTime + " ";

            File.WriteAllText("data\\" + songTitle + ".dat", textToWrite);

            b = null;                
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            bw.CancelAsync();
            cancelButton.Enabled = false;
        }
    }
}
