using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Display_test
{
    public delegate void OnInactivityDetected();

    // "Still Watching?" prompt. If nobody answers within 20 seconds it closes
    // itself and tells Form1 to return to the home screen.
    public partial class InActivityWindow : Form
    {
        private Timer timeChecker;
        private const int timerInterval = 20000;
        public OnInactivityDetected onInactivity;
        public Func<string, Task> WriteDebug;

        // Form1's inactivity timer, restarted on every touch/scroll/URL change.
        private Timer activityTimer;

        public InActivityWindow(OnInactivityDetected onInactivityDetected, Timer timerRef, Func<string, Task> writeDebug, int width)
        {
            InitializeComponent();
            activityTimer = timerRef;
            timeChecker = new Timer();
            timeChecker.Interval = timerInterval;
            timeChecker.Tick += autoCloseWindow;
            Shown += onFormShown;
            FormClosed += onFormClosed;
            onInactivity = onInactivityDetected;
            WriteDebug = writeDebug;
            Width = width;
            Height = width / 2;
            CenterToParent();
        }

        public void activityDetected(string m = "")
        {
            WriteDebug("ACTDET " + m);
            activityTimer.Stop();
            activityTimer.Start();
            if (timeChecker.Enabled)
            {
                DialogResult = DialogResult.Yes;
                Close();
            }
        }

        public void startTimer()
        {
            WriteDebug("TM STR");
            activityTimer.Start();
        }

        public void stopTimer()
        {
            WriteDebug("TM STP");
            activityTimer.Stop();
        }

        private void onFormShown(object obj, EventArgs args)
        {
            timeChecker.Stop();
            timeChecker.Start();
            CenterToParent();
            ((Form1)Owner).writeStat(Form1.statCodes.IAWOpened);
        }

        private void onFormClosed(object obj, EventArgs args)
        {
            timeChecker.Stop();
            ((Form1)Owner).writeStat(Form1.statCodes.IAWClosed);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            WriteDebug("BTN CLICK");
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void autoCloseWindow(object obj, EventArgs args)
        {
            WriteDebug("ACW");
            DialogResult = DialogResult.Cancel;
            timeChecker.Stop();
            Close();
            if (onInactivity != null)
            {
                onInactivity();
            }
        }

        private void lblText_Click(object sender, EventArgs e)
        {
            WriteDebug("LBL CLICK");
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void InActivityWindow_Load(object sender, EventArgs e)
        {
        }

        private void InActivityWindow_Click(object sender, EventArgs e)
        {
            WriteDebug("IAW CLICK");
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void InActivityWindow_Leave(object sender, EventArgs e)
        {
            WriteDebug("IAW FOCLV");
            DialogResult = DialogResult.Yes;
            Close();
        }

        // Scales text and the Yes button from a 960px-wide design.
        private void autoScale(object sender, EventArgs e)
        {
            float scale = Convert.ToSingle(Width) / 960f;
            Font font = new Font(new FontFamily("Microsoft Sans Serif"), 60f * scale, FontStyle.Bold);
            lblText.Font = font;
            btnYes.Width = (int)(Width / 2.5f);
            btnYes.Height = (int)(Height / 5f);
            btnYes.Left = Width / 2 - btnYes.Width / 2;
            btnYes.Top = (int)((Height / 2) / 2.5f) - btnYes.Height / 2;
            btnYes.Font = new Font(new FontFamily("Microsoft Sans Serif"), 60f * scale * 0.5f, FontStyle.Bold);
        }
    }
}
