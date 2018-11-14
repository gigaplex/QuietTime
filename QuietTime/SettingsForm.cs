using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietTime
{
    public partial class SettingsForm : Form
    {
        private string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private string registryValueName = "QuietTimeStartup";

        public SettingsForm()
        {
            InitializeComponent();

            Icon = QuietTime.Properties.Resources.NotifyIcon;

            // Initialize check box based off current registry setting
            using (var rootKey = Registry.CurrentUser)
            {
                using (var runKey = rootKey.OpenSubKey(registryKey))
                {
                    var value = runKey.GetValue(registryValueName);

                    if (value != null && value is string)
                    {
                        checkAutoStart.Checked = (string)value == Application.ExecutablePath;
                    }
                }
            }
            
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            using (var rootKey = Registry.CurrentUser)
            {
                using (var runKey = rootKey.OpenSubKey(registryKey, true))
                {
                    if (checkAutoStart.Checked)
                    {
                        runKey.SetValue(registryValueName, Application.ExecutablePath);
                    }
                    else
                    {
                        runKey.DeleteValue(registryValueName, false);
                    }
                }
            }

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
