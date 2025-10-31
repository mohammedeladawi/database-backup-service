\# 🗄️ Database Backup Windows Service



A Windows Service that automatically performs scheduled SQL Server database backups at configurable intervals.  

The service supports error logging, dependency configuration, and console-mode debugging for development.



---



\## 🚀 Features



\- \*\*Automated Backups\*\* — Performs full SQL Server database backups at configurable intervals.  

\- \*\*Dynamic Configuration\*\* — All settings are managed in `App.config`.  

\- \*\*Detailed Logging\*\* — Logs every service event and backup operation.  

\- \*\*Error Handling\*\* — Catches and logs all connection or backup errors.  

\- \*\*Console Mode\*\* — Can run interactively for testing and debugging.  

\- \*\*Service Dependencies\*\* — Ensures SQL Server and related services start first.  



---



\## ⚙️ Configuration



All settings are stored inside your \*\*App.config\*\* file under `<appSettings>`:



```xml

<appSettings>

&nbsp; <add key="ConnectionString" value="Server=YOUR\_SERVER;Database=YOUR\_DATABASE;Integrated Security=True;" />

&nbsp; <add key="BackupFolder" value="C:\\DatabaseBackups" />

&nbsp; <add key="LogFolder" value="C:\\DatabaseBackups\\Logs" />

&nbsp; <add key="BackupIntervalMinutes" value="60" />

</appSettings>

