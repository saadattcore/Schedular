using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Scheduler.WindowsService
{
    public static class DirectoryUtils
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(DirectoryUtils));
        private static string DefaultFolderPath =ConfigurationManager.AppSettings["DefaultJobPath"]?? "C:\\EAP-JOBS";

        public static string CreateFolder(string jobId, int jobVersion)
        {
            var path = Path.Combine(DefaultFolderPath, jobId, jobVersion.ToString());
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                LOG.Error($"Cannot create folder {path}: {e.Message}");
                return null;
            }
            return path;
        }
    }
}
