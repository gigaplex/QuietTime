using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietTime
{
    public class CustomApplicationContext : ApplicationContext
    {
        private const int VolumeTimerInterval = 5000; // 5 seconds
        private const int ResumeTimerInterval = 5 * 60 * 60 * 1000; // 5 hours

        private NotifyIcon notifyIcon;
        private SettingsForm settingsForm;
        private VolumeHandler volumeHandler = new VolumeHandler();
        private System.Timers.Timer volumeTimer;
        private System.Timers.Timer resumeTimer;
        private Control synchronisingControl;

        private bool m_paused = false;
        public bool Paused {
            get
            {
                return m_paused;
            }
            set
            {
                if (m_paused != value)
                {
                    m_paused = value;
                    var volumeTimer = this.volumeTimer;
                    if (volumeTimer != null)
                    {
                        if (value)
                            volumeTimer.Stop();
                        else
                            volumeTimer.Start();
                    }

                    var resumeTimer = this.resumeTimer;
                    if (resumeTimer != null)
                    {
                        if (value)
                            resumeTimer.Start();
                        else
                            resumeTimer.Stop();
                    }

                    PausedStateChanged?.Invoke(this, new PausedEventArgs(value));
                }
            }
        }

        public class PausedEventArgs : EventArgs
        {
            public bool NewPauseState { get; private set; }

            public PausedEventArgs(bool newPauseState)
            {
                NewPauseState = newPauseState;
            }
        }

        public delegate void PausedStateChangedEventHandler(object source, PausedEventArgs eventArgs);

        public event PausedStateChangedEventHandler PausedStateChanged;

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
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, ContextMenu_OnExit);

            defaultItem.Font = new Font(defaultItem.Font, defaultItem.Font.Style | FontStyle.Bold);

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            // Create an invisible control to use as the synchronising object, since the COM components behind VolumeHandler need to be called from the same thread
            synchronisingControl = new Control();
            synchronisingControl.CreateControl();

            volumeTimer = new System.Timers.Timer(VolumeTimerInterval)
            {
                Enabled = true,
                SynchronizingObject = synchronisingControl
            };
            volumeTimer.Elapsed += VolumeTimer_Elapsed;
            volumeTimer.Start();

            resumeTimer = new System.Timers.Timer(ResumeTimerInterval)
            {
                Enabled = true,
                SynchronizingObject = synchronisingControl
            };
            resumeTimer.Elapsed += ResumeTimer_Elapsed;
            resumeTimer.AutoReset = false;
        }

        private void VolumeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            volumeHandler.Mute = QuietTimeHandler.IsQuietTime(DateTime.Now);
        }

        private void ResumeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Paused = false;
        }

        protected override void ExitThreadCore()
        {
            volumeTimer?.Stop();
            resumeTimer?.Stop();
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

                volumeTimer?.Dispose();
                volumeTimer = null;

                resumeTimer?.Dispose();
                resumeTimer = null;
            }

            base.Dispose(disposing);
        }

        private void ShowSettingsForm()
        {
            if (settingsForm == null)
            {
                settingsForm = new SettingsForm(this);
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
