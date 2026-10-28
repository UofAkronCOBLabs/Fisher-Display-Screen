using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace Display_test
{
    public enum CurrentPage
    {
        HomePage,
        FirstLevelWebpage,
        SecondLevelButtonsPage
    }

    public partial class Form1 : Form
    {
        public enum statCodes
        {
            PageClose,
            Alumni,
            ExpBusiness,
            SocialMedia,
            DebugOn,
            DebugOff,
            BrowserUrlChange,
            Back,
            ProgramStart,
            ProgramClose,
            ProgramLostFocus,
            ProgramGainedFocus,
            IAWOpened,
            IAWClosed
        }

        private CurrentPage currentPage;

        // Idle time (ms) on a web page before the "Still Watching?" prompt appears.
        private int inactivityCheckDuration = 120000;

        private InActivityWindow inActivityWindow;

        private const int WM_TOUCH = 0x0246;
        private const int WM_TOUCHUPDATE = 0x0245;

        private bool debugEnabled;
        private string backAddr = "";
        private string firstAddr = "";
        private bool buttonsRight = true;
        private Timer timerRef;

        // Usage stats are appended to Documents\displaytest_output\data.csv
        private string statsFile;
        private object statsLock = new object();

        private List<string> Debugs = new List<string>();
        private int clicks;

        // Injected into every loaded page so taps/scrolls inside the browser count as activity.
        private const string script = "if(typeof WINFORMS_SCR_LOADED === 'undefined') { document.addEventListener(\"click\", function(e) { console.log(\"WINFORMS CLICK ACTIVITY\"); }); document.addEventListener(\"scroll\", function(e) { console.log(\"WINFORMS SCROLL ACTIVITY\"); }); WINFORMS_SCR_LOADED = true; }";

        public Form1()
        {
            currentPage = CurrentPage.HomePage;
            timerRef = new Timer();
            timerRef.Interval = inactivityCheckDuration;
            timerRef.Tick += onTimerTick;
            inActivityWindow = new InActivityWindow(closeWebpageAuto, timerRef, DebugIfAble, Width / 4);
            setupStats();
            InitializeComponent();
            lblDebug.Hide();
            btnHome.SendToBack();
            btnBack.SendToBack();
            btnMove.SendToBack();
            lblDebug.Text = "";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        // Scales fonts, margins and the nav buttons from a 3840px-wide design.
        private void autoScale(object sender, EventArgs e)
        {
            float scale = Width / 3840f;

            Font touchFont = new Font(new FontFamily("Calibri"), 60f * scale, FontStyle.Bold);
            lblTouch.Font = touchFont;
            picTouch.Margin = new Padding((int)Math.Ceiling(60f * scale));

            Font handleFont = new Font(new FontFamily("Calibri"), 60f * scale * 0.6f, FontStyle.Bold);
            lblIG.Font = handleFont;
            lblTwitter.Font = handleFont;
            lblFB.Font = handleFont;
            lblHashtags.Font = handleFont;
            lblHashtags.Padding = new Padding((int)Math.Ceiling(60f * scale * 0.3f));

            Padding buttonMargin = new Padding((int)Math.Ceiling(12f * scale));
            btnAlumni.Margin = buttonMargin;
            btnExpBusiness.Margin = buttonMargin;
            btnSocialMedia.Margin = buttonMargin;

            picTouch.Margin = new Padding((int)Math.Ceiling(60f * scale));

            int logoSize = (int)Math.Ceiling(60f * scale * 12f);
            picLogo.Size = new Size(logoSize, logoSize);

            int navSize = (int)(180f * scale);
            btnBack.Width = navSize;
            btnBack.Height = navSize;
            btnHome.Width = navSize;
            btnHome.Height = navSize;

            int edge = (int)(120f * scale);
            int[] backPos = new int[] { Width - edge - navSize, Height - edge - navSize };
            btnBack.Left = backPos[0];
            btnBack.Top = backPos[1];
            btnHome.Left = backPos[0] - navSize;
            btnHome.Top = backPos[1];

            btnMove.Width = navSize / 2;
            btnMove.Height = navSize / 2;
            btnMove.Left = Width - edge - btnMove.Width;
            btnMove.Top = Height - edge - btnMove.Height - navSize;
            buttonsRight = true;
            btnMove.BackgroundImage = Properties.Resources.leftarrow;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: reduces flicker when swapping layers
                return cp;
            }
        }

        private void setupStats()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "displaytest_output");
            Directory.CreateDirectory(dir);
            statsFile = Path.Combine(dir, "data.csv");
            lock (statsLock)
            {
                if (File.Exists(statsFile))
                {
                    // Stop logging once the file passes ~1 GB.
                    if (new FileInfo(statsFile).Length > 1000000000)
                    {
                        statsFile = null;
                        return;
                    }
                }
                else
                {
                    try
                    {
                        File.WriteAllText(statsFile, "Timestamp,Action Code,Details" + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString() + "] File access error: Unable to begin formatting of stats file. Exception: " + ex.Message);
                    }
                }
                try
                {
                    File.AppendAllText(statsFile, string.Format("{0},{1}{2}", DateTime.Now.ToString("s"), statCodes.ProgramStart.ToString(), Environment.NewLine));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] File access error: Unable to write line to stats file. Exception: " + ex.Message);
                }
            }
        }

        public void writeStat(statCodes code)
        {
            Task.Run(() =>
            {
                lock (statsLock)
                {
                    try
                    {
                        File.AppendAllText(statsFile, string.Format("{0},{1}{2}", DateTime.Now.ToString("s"), code.ToString(), Environment.NewLine));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString() + "] File access error: Unable to write line to stats file. Exception: " + ex.Message);
                    }
                }
            });
        }

        public void writeStat(statCodes code, string str)
        {
            Task.Run(() =>
            {
                lock (statsLock)
                {
                    try
                    {
                        File.AppendAllText(statsFile, string.Format("{0},{1},{2}{3}", DateTime.Now.ToString("s"), code.ToString(), str, Environment.NewLine));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString() + "] File access error: Unable to write line to stats file. Exception: " + ex.Message);
                    }
                }
            });
        }

        // Hidden debug toggle: tap the logo 5 times within 3 seconds.
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (clicks == 0)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    if (clicks < 5)
                    {
                        clicks = 0;
                    }
                });
            }
            clicks++;
            if (clicks >= 5)
            {
                clicks = 0;
                if (debugEnabled)
                {
                    DebugDisable();
                }
                else
                {
                    DebugEnable();
                }
            }
        }

        private async Task DebugIfAble(string msg)
        {
            if (!debugEnabled)
            {
                return;
            }
            Debugs.Add(msg);
            Invoke((Action)(() =>
            {
                lblDebug.Text = string.Join(Environment.NewLine, Debugs.Distinct());
                lblDebug.BringToFront();
                lblDebug.Show();
            }));
            await Task.Delay(6000);
            Debugs.Remove(msg);
            Invoke((Action)(() =>
            {
                if (Debugs.Count == 0)
                {
                    lblDebug.Hide();
                    lblDebug.Text = "";
                }
                else
                {
                    lblDebug.Text = string.Join(Environment.NewLine, Debugs.Distinct());
                }
            }));
        }

        private async Task DebugEnable()
        {
            // Only logs the event; the shipped build never turns the overlay on.
            writeStat(statCodes.DebugOn);
        }

        private async Task DebugDisable()
        {
            writeStat(statCodes.DebugOff);
            debugEnabled = false;
            Debugs.Clear();
            lblDebug.BackColor = Color.LightCoral;
            lblDebug.Text = "OFF";
            lblDebug.BringToFront();
            lblDebug.Show();
            await Task.Delay(6000);
            lblDebug.Hide();
            lblDebug.Text = "";
            lblDebug.BackColor = Color.PaleGoldenrod;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 't')
            {
                DebugIfAble("TM " + timerRef.Enabled.ToString() + timerRef.ToString());
            }
        }

        private void btnAlumni_Click(object sender, EventArgs e)
        {
            writeStat(statCodes.Alumni);
            showWebPage("https://www.uakron.edu/cba/departments/marketing/alumni-stories");
        }

        private void btnExpBusiness_Click(object sender, EventArgs e)
        {
            writeStat(statCodes.ExpBusiness);
            showWebPage("https://www.uakron.edu/cba/outcomes/experiential-learning");
        }

        private void btnSocialMedia_Click(object sender, EventArgs e)
        {
            writeStat(statCodes.SocialMedia);
            showWebPage("https://keyhole.co/hashtag-tracking/media-wall/1JZR73/digitaldisplaysmarketing?page=1&perPage=25");
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            closeWebpage();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (chromium.CanGoBack)
            {
                // Back from the first page of a section returns to the home screen.
                if (chromium.Address == firstAddr)
                {
                    closeWebpage();
                    return;
                }
                backAddr = chromium.Address;
                chromium.Back();
            }
            else if (!chromium.IsLoading)
            {
                closeWebpage();
            }
        }

        // Swaps the Home/Back buttons between the right and left corners (reachability).
        private void btnMove_Click(object sender, EventArgs e)
        {
            inActivityWindow.activityDetected("BTNMOVE");
            int width = btnBack.Width;
            if (buttonsRight)
            {
                int left = Width - btnBack.Left - width;
                btnMove.Left = left;
                btnBack.Left = left + width;
                btnHome.Left = left;
                buttonsRight = false;
                btnMove.BackgroundImage = Properties.Resources.rightarrow;
            }
            else
            {
                int left = btnHome.Left;
                btnMove.Left = Width - left - width / 2;
                btnBack.Left = Width - left - width;
                btnHome.Left = Width - left - width * 2;
                buttonsRight = true;
                btnMove.BackgroundImage = Properties.Resources.leftarrow;
            }
        }

        private void onTimerTick(object sender, EventArgs args)
        {
            navigateBackAfterInactivity();
        }

        private void navigateBackAfterInactivity()
        {
            inActivityWindow.stopTimer();
            inActivityWindow = new InActivityWindow(closeWebpageAuto, timerRef, DebugIfAble, Width / 4);
            if (inActivityWindow.ShowDialog(this) == DialogResult.Yes)
            {
                inActivityWindow.startTimer();
            }
        }

        private void activity_event(object sender, EventArgs e)
        {
            inActivityWindow.activityDetected("EVNT");
        }

        public void webBrowser2_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                e.Browser.ExecuteScriptAsync(script);
            }

            if (e.IsLoading && !chromium.CanGoBack)
            {
                btnBack.BackgroundImage = Properties.Resources.backbutton_grayed;
            }
            else
            {
                btnBack.BackgroundImage = Properties.Resources.backbutton;
            }

            if (backAddr != "" && !e.IsLoading)
            {
                // Page redirected straight back to where we came from: skip past it.
                if (chromium.Address == backAddr)
                {
                    Invoke((Action)(() => btnBack_Click(null, null)));
                    return;
                }
                backAddr = "";
            }
        }

        public void webBrowser2_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Equals("WINFORMS CLICK ACTIVITY"))
            {
                Invoke((Action)(() => inActivityWindow.activityDetected("JS CLK")));
            }
            else if (e.Message.Equals("WINFORMS SCROLL ACTIVITY"))
            {
                Invoke((Action)(() => inActivityWindow.activityDetected("JS SCR")));
            }
        }

        private void activity_event(object sender, ScrollEventArgs e)
        {
            inActivityWindow.activityDetected("EVNT SCR");
        }

        private void activity_event(object sender, MouseEventArgs e)
        {
            inActivityWindow.activityDetected("EVNT MOU");
        }

        private void activity_event(object sender, AddressChangedEventArgs e)
        {
            Invoke((Action)(() =>
            {
                writeStat(statCodes.BrowserUrlChange, e.Address);
                inActivityWindow.activityDetected("URL CHNG");
            }));
        }

        private void activity_event(object sender, FormClosedEventArgs e)
        {
            writeStat(statCodes.ProgramClose);
        }

        private void activity_event_activated(object sender, EventArgs e)
        {
            writeStat(statCodes.ProgramGainedFocus);
        }

        private void activity_event_deactivated(object sender, EventArgs e)
        {
            writeStat(statCodes.ProgramLostFocus);
        }

        private void showWebPage(string url)
        {
            initBrowser();
            currentPage = CurrentPage.FirstLevelWebpage;
            chromium.Load(url);
            firstAddr = url;
            chromium.BringToFront();
            btnBack.BringToFront();
            btnHome.BringToFront();
            btnMove.BringToFront();
            tlpMain.SendToBack();
            inActivityWindow.startTimer();
        }

        private void closeWebpage(bool auto = false)
        {
            writeStat(statCodes.PageClose, auto ? "auto" : "back");
            btnHome.SendToBack();
            btnBack.SendToBack();
            btnMove.SendToBack();
            tlpMain.BringToFront();
            lblDebug.BringToFront();
            inActivityWindow.stopTimer();
            Controls.Remove(chromium);
            if (chromium != null)
            {
                chromium.Dispose();
                chromium = null;
            }
            backAddr = "";
            if (!buttonsRight)
            {
                btnMove_Click(null, null);
            }
        }

        private void closeWebpageAuto()
        {
            closeWebpage(true);
        }

        // Keeps each home-screen tile image at its aspect ratio by padding the sides.
        private void gridbuttonResize(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int imageWidth = (button.Size.Height - 16) * button.BackgroundImage.Width / button.BackgroundImage.Height;
            Padding margin = button.Margin;
            int extra = button.Size.Width + button.Margin.Horizontal - imageWidth;
            if (extra <= 32)
            {
                margin.All = 16;
            }
            else
            {
                margin.Top = 16;
                margin.Bottom = 16;
                margin.Left = extra / 2;
                margin.Right = extra / 2;
            }
            button.Margin = margin;
        }

        // A fresh browser is created for every visit and disposed on return to Home.
        private void initBrowser()
        {
            chromium = new ChromiumWebBrowser();
            chromium.ActivateBrowserOnCreation = false;
            chromium.Dock = DockStyle.Fill;
            chromium.Location = new Point(0, 0);
            chromium.Margin = new Padding(2);
            chromium.MinimumSize = new Size(10, 10);
            chromium.Name = "chromium";
            chromium.Size = new Size(1370, 749);
            chromium.TabIndex = 4;
            chromium.AddressChanged += activity_event;
            chromium.LoadingStateChanged += webBrowser2_LoadingStateChanged;
            chromium.ConsoleMessage += webBrowser2_ConsoleMessage;
            chromium.Click += activity_event;
            chromium.LifeSpanHandler = new ChromiumLifeSpanHandler();
            Controls.Add(chromium);
        }
    }
}
