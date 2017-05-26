﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PowershellStarter
{

    
    public partial class PowershellStarterService : ServiceBase
    {

        
        public string ScriptPath { get; set; }
        public string ScriptArguments { get; set; }
        public string output;
        public string errorOutput;
        public ProcessStartInfo process = new ProcessStartInfo();
        public Process p;

        public PowershellStarterService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("PowershellStarter"))
            {
                System.Diagnostics.EventLog.CreateEventSource(ConfigurationManager.AppSettings["EventLogSource"], ConfigurationManager.AppSettings["EventLog"]);

            }
            eventLog1.Source = ConfigurationManager.AppSettings["EventLogSource"];
            eventLog1.Log = ConfigurationManager.AppSettings["EventLog"];
        }

        protected virtual void onScriptExited(object sender, EventArgs e)
        {

            eventLog1.WriteEntry("Script is no longer running, terminating service...");
            Environment.FailFast("Script no longer running, service stopped.");


        }

        protected override void OnStart(string[] args)
        {

            string ScriptPath = ConfigurationManager.AppSettings["ScriptPath"];
            string ScriptParameters = ConfigurationManager.AppSettings["ScriptParameters"];
            
            process.CreateNoWindow = true;
            process.UseShellExecute = false;
            process.WorkingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            process.RedirectStandardOutput = true;
            process.RedirectStandardError = true;
            process.FileName = "C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe";
            process.Arguments = "-ExecutionPolicy Unrestricted -File " + ScriptPath + " " + ScriptParameters;




            Process PSProcess = new System.Diagnostics.Process();
            PSProcess.StartInfo = process;
            PSProcess.EnableRaisingEvents = true;
            PSProcess.Exited += new System.EventHandler(onScriptExited);
            
            PSProcess.OutputDataReceived += (sender, EventArgs) => eventLog1.WriteEntry(EventArgs.Data);
            PSProcess.ErrorDataReceived += (sender, EventArgs) => eventLog1.WriteEntry(EventArgs.Data);
            PSProcess.Start();
            PSProcess.BeginOutputReadLine();
            PSProcess.BeginErrorReadLine();







            eventLog1.WriteEntry("PowershellStarter Started powershell service with scriptpath:" + ScriptPath + " and parameter: " + ScriptParameters);

            


        }
        
        protected override void OnStop()
        {

            if (!p.HasExited) {

                p.Kill();
            }

            eventLog1.WriteEntry("PowershellStarter Stopped.");

        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("PowershellStarter does not support continue...");
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
