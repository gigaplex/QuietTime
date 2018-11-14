using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietTime
{
    class CustomApplicationContext : ApplicationContext
    {
        private const int TimerInterval = 5000;

        private NotifyIcon notifyIcon;
        private SettingsForm settingsForm;
        private VolumeHandler volumeHandler = new VolumeHandler();
        private System.Timers.Timer timer;
        private Control synchronisingControl;

        public CustomApplicationContext()
        {
            notifyIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = QuietTime.Properties.Resources.NotifyIcon,
                Text = "QuietTime",
                Visible = true
            };

            //var defaultItem = notifyIcon.ContextMenuStrip.Items.Add("Open &Settings", null, ContextMenu_OnSettings);
            //notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, ContextMenu_OnExit);

            //defaultItem.Font = new Font(defaultItem.Font, defaultItem.Font.Style | FontStyle.Bold);

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            // Create an invisible control to use as the synchronising object, since the COM components behind VolumeHandler need to be called from the same thread
            synchronisingControl = new Control();
            synchronisingControl.CreateControl();

            timer = new System.Timers.Timer(TimerInterval)
            {
                Enabled = true,
                SynchronizingObject = synchronisingControl
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            volumeHandler.Mute = QuietTimeHandler.IsQuietTime(DateTime.Now);
        }

        protected override void ExitThreadCore()
        {
            timer?.Stop();
            settingsForm?.Close();
            if (notifyIcon != null)
                notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                notifyIcon?.Dispose();
                notifyIcon = null;

                settingsForm?.Dispose();
                settingsForm = null;

                volumeHandler?.Dispose();
                volumeHandler = null;

                timer?.Dispose();
                timer = null;
            }

            base.Dispose(disposing);
        }

        private void ShowSettingsForm()
        {
            if (settingsForm == null)
            {
                settingsForm = new SettingsForm();
                settingsForm.Closed += SettingsForm_Closed;
                settingsForm.Show();
            }
            else
            {
                settingsForm.Activate();
            }
        }

        private void ContextMenu_OnSettings(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void ContextMenu_OnExit(object sender, EventArgs e)
        {
            ExitThread();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowSettingsForm();
        }

        private void SettingsForm_Closed(object sender, EventArgs e)
        {
            settingsForm = null;
        }
    }
}
