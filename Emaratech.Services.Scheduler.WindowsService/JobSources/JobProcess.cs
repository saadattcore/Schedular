using Emaratech.Services.Scheduler.WindowsService.Interfaces;
using Ionic.Zip;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.WindowsService.JobSources
{
    public class JobProcess : IJobSource
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JobProcess));

        public string JobInstanceId { get; set; }
        public string JobId { get; set; }
        public int JobVersion { get; set; }
        public string Source { get; set; }

        public async Task<object> Execute()
        {
            try
            {
                var jobFolder = DirectoryUtils.CreateFolder(JobId, JobVersion);
                if (!string.IsNullOrEmpty(jobFolder))
                {
                    var processName = Path.GetFileNameWithoutExtension(Source);
                    var filePath = Path.Combine(jobFolder, Source);
                    var tempFilePath = Path.Combine(jobFolder, "temp");
                    if (!File.Exists(filePath))
                    {
                        Stream content = await ApiFactory.Default.SchedulerApi
                            .GetJobContentAsync(JobId, JobVersion.ToString());

                        FileStream fs = File.Create(tempFilePath);
                        content.Seek(0, SeekOrigin.Begin);
                        content.CopyTo(fs);
                        fs.Close();

                        if (ZipFile.IsZipFile(tempFilePath))
                        {
                            using (var zip = ZipFile.Read(tempFilePath))
                            {
                                zip.ExtractAll(jobFolder, ExtractExistingFileAction.OverwriteSilently);
                            }
                            File.Delete(tempFilePath);
                        }
                        else
                        {
                            File.Move(tempFilePath, filePath);
                        }
                    }

                    StartProcess(filePath);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                try
                {
                    await ApiFactory.Default.SchedulerApi.UnlockJobAsync(JobId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return null;
        }

        private void StartProcess(string filePath)
        {
            Log.Info($"Starting process {filePath}");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = null;
            startInfo.RedirectStandardError = true;

            var process = new Process {EnableRaisingEvents = true};
            process.Exited += (sender, e) =>
            {
                Log.Info($"Process {filePath} exited");
                try
                {
                    string procError = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(procError))
                    {
                        Log.Debug(procError);
                    }

                    ApiFactory.Default.SchedulerApi.UnlockJobAsync(JobId);

                    ApiFactory.Default.SchedulerApi.UpdateJobInstanceAsync(JobInstanceId,
                        new Model.JobInstanceUpdate { IsExecuted = true });

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}