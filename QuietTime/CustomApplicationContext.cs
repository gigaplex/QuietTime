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
        private NotifyIcon notifyIcon;
        private SettingsForm settingsForm;
        private VolumeHandler volumeHandler = new VolumeHandler();

        public CustomApplicationContext()
        {
            notifyIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = QuietTime.Properties.Resources.NotifyIcon,
                Text = "QuietTime",
                Visible = true
            };

            var defaultItem = notifyIcon.ContextMenuStrip.Items.Add("Open &Settings", null, ContextMenu_OnSettings);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Mute", null, ContextMenu_Mute);
            notifyIcon.ContextMenuStrip.Items.Add("&Unmute", null, ContextMenu_Unmute);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, ContextMenu_OnExit);

            defaultItem.Font = new Font(defaultItem.Font, defaultItem.Font.Style | FontStyle.Bold);

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        }

        protected override void ExitThreadCore()
        {
            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (volumeHandler != null)
                {
                    volumeHandler.Dispose();
                    volumeHandler = null;
                }
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

        private void ContextMenu_Mute(object sender, EventArgs e)
        {
            volumeHandler.Mute();
        }

        private void ContextMenu_Unmute(object sender, EventArgs e)
        {
            volumeHandler.Unmute();
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
