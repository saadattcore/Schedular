namespace Emaratech.Services.Scheduler.Scheduler
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.schedulerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.schedulerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.schedulerServiceProcessInstaller.Password = null;
            this.schedulerServiceProcessInstaller.Username = null;
            // 
            // serviceInstaller1
            // 
            this.schedulerServiceInstaller.ServiceName = "SchedulerService";
            this.schedulerServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.schedulerServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.schedulerServiceInstaller,
            this.schedulerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller schedulerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller schedulerServiceInstaller;
    }
}