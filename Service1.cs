using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
// Add system.data.sqlClient

namespace DatabaseBackupService
{
    public partial class Service1 : ServiceBase
    {
        readonly string logFolder = ConfigurationManager.AppSettings["LogFolder"];
        readonly string backupFolder = ConfigurationManager.AppSettings["BackupFolder"];
        readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        readonly int backupIntervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["BackupIntervalMinutes"]);

        private Timer _timer;
        private bool _isRunning = false;

        public Service1()
        {
            InitializeComponent();

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
        }

        protected override void OnStart(string[] args)
        {
            Log($"[{DateTime.Now}] Service has started");


            // every 60 min backup
            _timer = new Timer(PerformBackup, null, TimeSpan.Zero, TimeSpan.FromMinutes(backupIntervalMinutes));
        }

        protected override void OnStop()
        {
            _timer?.Dispose();
            Log($"[{DateTime.Now}] Service has stoped");
        }


        private void PerformBackup(object state)
        {
            if (_isRunning) return;

            _isRunning = true;
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFilePath = Path.Combine(backupFolder, $"Backup_{timestamp}.bak");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"BACKUP DATABASE [{conn.Database}] TO DISK = '{backupFilePath}' WITH INIT";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Log($"Database backup successful: {backupFilePath}");
            }
            catch (Exception ex)
            {
                Log($"Error during backup: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }
        
        private void Log(string message)
        {
            try
            {
                // Ensure log directory exists
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string logFilePath = Path.Combine(logFolder, "service.log");

                // Format log message with timestamp
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

                // Append message to log file (creates file automatically if missing)
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);

                // Also write to console in interactive mode (debug mode)
                if (Environment.UserInteractive)
                    Console.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                // Avoid crashing service if logging fails
                Debug.WriteLine($"Logging failed: {ex.Message}");
            }

        }

        public void StartInConsole()
        {
            OnStart(null);
            Console.WriteLine("Press enter to stop the service...");
            Console.ReadLine();
            OnStop();
        }



    }
}
