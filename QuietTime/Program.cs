using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietTime
{
    static class Program
    {
        static bool IsProcessCurrentExe(Process process, Process currentProcess)
        {
            try
            {
                return process.MainModule.FileName == currentProcess.MainModule.FileName;
            }
            catch
            {
                // Commonly hits this path if running as a 32bit application while querying a 64bit process
                return false;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var currentProcess = Process.GetCurrentProcess();
            Process[] runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName)
                .Where(process => IsProcessCurrentExe(process, currentProcess)).ToArray(); // In case of potential process name clashes, check if they're the same exe
            
            if (runningProcesses.Length == 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var appContext = new CustomApplicationContext();
                Application.Run(appContext);
            }
        }
    }
}
