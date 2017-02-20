using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QShut
{

    /// <summary>
    /// A simple application to schedule the PC to shut down in 'x' seconds.
    /// </summary>
    public partial class MainForm : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
            int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public MainForm()
        {
            InitializeComponent();

            // Segoe UI MDL2 close button icon
            BtnExit.Text = "\xE106";
        }

        private void BtnExit_Click(object sender, System.EventArgs e)
        {
            // Exit the application
            Application.Exit();
        }

        private void BtnShutdown_Click(object sender, System.EventArgs e)
        {
            // String passed to the console to commit the full shutdown command
            string strShutDown = @"shutdown -s -t " + TimeDifference();

            Process cmd = new Process();
            ProcessStartInfo cmdInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,    // Hide the console on start
                FileName = "cmd.exe",                       // Start the console
                Arguments = "/c " + strShutDown             // Pass the shutdown string
            };

            cmd.StartInfo = cmdInfo;                        // Pass the cmd process its start info
            cmd.Start();                                    // Start the cmd process
        }

        private void btnCancelShutdown_Click(object sender, EventArgs e)
        {
            // Same as above, but instead we pass the console 'shutdown -a' to cancel any pending shutdown requests
            Process cmd = new Process();
            ProcessStartInfo cmdInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c shutdown -a"
            };

            cmd.StartInfo = cmdInfo;
            cmd.Start();
        }

        public int TimeDifference()
        { 
            DateTime usrSelectedTime = TimePicker.Value;        // Retrieve the time the user has selected
            DateTime timeNow = DateTime.Now.ToLocalTime();      // Retrieve the current PC time
            double diffTimeInSeconds = (usrSelectedTime - timeNow).TotalSeconds;    // Calculate the difference between now and the time the user has selected
            int timeInInt = Convert.ToInt32(diffTimeInSeconds); // Convert the difference to int

            // If the user has selected a time before the current time, inform the user of their mistake
            if (usrSelectedTime < timeNow)
            {
                MessageBox.Show(@"You cannot pick a time earlier than the current time", "Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return timeInInt;
        }

        private void mainPanel_MouseDown(object sender, MouseEventArgs e)
        {   
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
