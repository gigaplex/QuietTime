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
        private CustomApplicationContext appContext;

        public SettingsForm(CustomApplicationContext context)
        {
            appContext = context;
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
            
            if (appContext.Paused)
            {
                pauseButton.Text = "Resume";
            }
            else
            {
                pauseButton.Text = "Pause";
            }

            appContext.PausedStateChanged += AppContext_PausedStateChanged;
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

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (appContext.Paused)
            {
                appContext.Paused = false;
            }
            else
            {
                appContext.Paused = true;
            }
        }

        private void AppContext_PausedStateChanged(object sender, CustomApplicationContext.PausedEventArgs eventArgs)
        {
            if (eventArgs.NewPauseState)
            {
                pauseButton.Text = "Resume";
            }
            else
            {
                pauseButton.Text = "Pause";
            }
        }
    }
}
