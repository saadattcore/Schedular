using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emaratech.Services.Scheduler.Api;
using Emaratech.Services.Scheduler.Model;
using RestSharp.Extensions;
using System.IO.Compression;

namespace Emaratech.Services.Scheduler.JobManager
{
    public partial class Form1 : Form
    {
        private const string SystemId = "0E56E4FE722847BCBD4F2569E2C87E14";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var job = comboBox1.SelectedItem as Job;
            var process = textBox1.Text;

            if (job != null && !string.IsNullOrEmpty(process)) { 
                
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    string input = folderBrowserDialog1.SelectedPath;

                    string output = $@"C:\EAP-JOBS\{job.JobId}_{job.Version}_in.zip";

                    ZipFile.CreateFromDirectory(input,output,CompressionLevel.Optimal,false);

                    var data = Convert.ToBase64String(File.ReadAllBytes(output));

                    var client = Api;

                    client.Configuration.Timeout = 1000000;

                    var version = client.UpdateJob(job.JobId, new JobUpdate()
                    {
                        Name = job.Name,
                        MaxLockSeconds = job.MaxLockSeconds,
                        JobSource = new JobSourceAdd()
                        {
                            Content = data,
                            Process = textBox1.Text
                        }
                    });

                    Refresh();
                    MessageBox.Show("Job Updated with version " + version);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var job = comboBox1.SelectedItem as Job;
            var content = Api.GetJobContent(job.JobId, job.Version?.ToString());
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.Combine(folderBrowserDialog1.SelectedPath,$"{job.JobId}_{job.Version}.zip");

                File.WriteAllBytes(fileName, content.ReadAsBytes());
            }
        }

        private void Refresh()
        {
            var jobs = Api.GetJobsBySystemId(SystemId, "1", "1000");
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(jobs.Data.ToArray());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Refresh();

        }

        private static SchedulerApi Api
        {
            get { return new SchedulerApi(ConfigurationManager.AppSettings["SchedulerApi"]); }
        }
    }
}
