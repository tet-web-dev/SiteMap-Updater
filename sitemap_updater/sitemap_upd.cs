using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace sitemap_updater
{
    public partial class sitemap_upd : ServiceBase
    {
        // config file init location
        private IConfigurationRoot Config()
        {
            var b = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = b.Build();
            return config;
        }      
        // path to sitemap
        private string sitemap_path()
        {
            return Config().GetSection("sitemap_path").Value;
        }

        // sitemap file name
        private string sitemap_filename()
        {
            return Config().GetSection("sitemap_filename").Value;
        }
        
        // number of seconds between update check
        private int polltime_minutes()
        {
            int p = 0;
            int.TryParse(Config().GetSection("polltime_minutes").Value, out p);
            return p * 60000;
        }

        private string _sitemap;
        private string _sitemap_temp;
        private int _polltime_minutes;
        private string _wtf;
        private int eventId = 1;

        // MS Doc for creating service suggested adding this
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        // MS Doc for creating service suggested adding this
        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        // MS Doc for creating service suggested adding this
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        public sitemap_upd(string[] args)
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("SitemapUpdSource")) {
                System.Diagnostics.EventLog.CreateEventSource("SiteMapUpdSource", "SiteMapUpdLog");
            }
            eventLog1.Source = "SiteMapUpdSource";
            eventLog1.Log = "SiteMapUpdLog";
        }

        protected override void OnStart(string[] args)
        {
            _sitemap = sitemap_path() + sitemap_filename();
            _sitemap_temp = sitemap_path() + "sitemap.temp";
            _polltime_minutes = polltime_minutes();
            
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("Service Start. Sitemap file: [" + _sitemap +"] pooling every: " + (_polltime_minutes == 0 ? "30 minutes (via app default) " : (_polltime_minutes / 60000) .ToString() )+ " minutes");
            Timer timer = new Timer();
            timer.Interval = _polltime_minutes == 0 ? 1800000 : _polltime_minutes;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
 
            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("SiteMapUpd Stopped");

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            eventLog1.WriteEntry("Executing SP", EventLogEntryType.Information, eventId++);
            List<string> doc = new List<string>();
            
            // Header lines of the sitemap file
            doc.Add("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
            doc.Add("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            // body of the sitemap file
            doc = new SqlOp().ExecSpSiteMap(doc);
            // closing line(s) of the sitemap file
            doc.Add("</urlset>");

            if (!File.Exists(_sitemap))
            {
                eventLog1.WriteEntry("First Run...Creating Site Map.", EventLogEntryType.Information, eventId++);
                File.WriteAllLines(_sitemap, doc);
                return;
            }

            File.WriteAllLines(_sitemap_temp, doc);
            // compare
            if (FileEquals(_sitemap, _sitemap_temp))
            {
                eventLog1.WriteEntry("No Diff", EventLogEntryType.Information, eventId++);
            } else {
                eventLog1.WriteEntry("Updating", EventLogEntryType.Information, eventId++);
                File.Delete(_sitemap);
                try
                {
                    File.WriteAllLines(_sitemap, doc);
                    eventLog1.WriteEntry("UPDATED!!!", EventLogEntryType.Information, eventId++);
                }
                catch (Exception e) {
                    eventLog1.WriteEntry("UPDATE Failed!!!  " + e.Message, EventLogEntryType.Information, eventId++);
                }
            }
        }

        // method to compare for changes
        static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
