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
using MusicRecognitionClassLibrary;

namespace RecognitionEfficiencyEstimator
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw;
        private MatchFinder matchFinder; 
        private const string logFile = "log.txt";        

        public Form1()
        {
            InitializeComponent();
            matchFinder = new MatchFinder("data\\");          
        }

        private string[] getFileNames()
        {
            string path = "testRecordings";
            string[] res;            
            var files = Directory.GetFiles(path);          
            res = files;         
            return res;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (StartButton.Text == "Start")
            {
                logBox.Items.Clear();
                ResultLabel.Text = "";
                progressBar1.Value = 0;

                bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

                bw.DoWork += DoWork;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.RunWorkerCompleted += bw_Completed;
                bw.RunWorkerAsync(getFileNames());

                StartButton.Text = "Cancel";
            }
            else
            {           
                bw.CancelAsync();
                StartButton.Text = "Cancelling...";
                StartButton.Enabled = false;
            }
        }

        private void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                logBox.Items.Clear();
                progressBar1.Value = 0;
                ResultLabel.Text = "";
            }
            else
            {                
                progressBar1.Value = 100;

                System.IO.File.AppendAllText(logFile, ResultLabel.Text + System.Environment.NewLine);                             
                File.AppendAllLines(logFile, ParamsParser.getAllParams());
                File.AppendAllText(logFile, "-------------------------------------" + System.Environment.NewLine + System.Environment.NewLine);
            }
            StartButton.Text = "Start";
            StartButton.Enabled = true;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;        
            string[] s = e.UserState.ToString().Split(';');
            logBox.Items.Add(s[0]);
            logBox.Items.Add(s[1]);
            logBox.Items.Add(s[2]);
            logBox.Items.Add("--------------------------------");
            logBox.SetSelected(logBox.Items.Count - 1, true);
            logBox.SetSelected(logBox.Items.Count - 1, false);
            ResultLabel.Text = s[3] + "% correct";
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var files = (string[])e.Argument;
            int correct = 0;

            for (int i = 0; i < files.Length; i++)
            {
                if (bw.CancellationPending) { e.Cancel = true; return; }
                string rep = "";
                string match = matchFinder.getBestMatch(files[i]);
                match = match.Substring(0, match.LastIndexOf(' '));
                if (Path.GetFileNameWithoutExtension(files[i])  != match)
                {
                    rep = "result: WRONG";
                }
                else
                {
                    rep = "result: OK";
                    correct++;
                }

                bw.ReportProgress((int) ((i+1)*100.0/files.Length),
                                  "song '" + Path.GetFileNameWithoutExtension(files[i]) +"'    -     " + rep + ";" + 
                                  (i + 1).ToString() + "/" + files.Length.ToString() + ";" +
                                  correct.ToString() + " correct out of " + (i + 1).ToString() + " processed;"+
                                  ((int)(correct*100/(i+1))).ToString()+";" + files.Length.ToString()
                                  );

            }
        }

    }
}
