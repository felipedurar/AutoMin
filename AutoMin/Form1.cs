using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace AutoMin
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public Point lastPos = new Point(0, 0);

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            notifyIcon1.Text = "AutoMin - " + (Status.Running ? "Running" : "Paused");

            if (Status.SettingsChanged)
            {
                LoadSettings();
                Status.CurrentIdleSecs = 0;
                Status.IsIdle = false;
                Status.SettingsChanged = false;
            }

            if (!Status.Running)
                return;

            if (lastPos.Equals(Cursor.Position))
                Status.CurrentIdleSecs++;
            else
            {
                if (Status.IsIdle)
                {
                    // Restore the Windows
                    Console.WriteLine("Undo Minimize All");
                    undoMinimizeAll();
                }

                Status.IsIdle = false;
                Status.CurrentIdleSecs = 0;
            }

            if (Status.CurrentIdleSecs > Status.TriggerSecs && !Status.IsIdle)
            {
                Status.IsIdle = true;
                Status.CurrentIdleSecs = 0;
                Console.WriteLine("Minimize All");
                minimizeAll();
            }

            lastPos = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        private void undoMinimizeAll()
        {
            IntPtr handle = getShellTray();
            if (handle == (IntPtr)0) return;

            IntPtr res = SendMessage(handle, 0x0111, (IntPtr)416, (IntPtr)0);
        }

        private void minimizeAll()
        {
            IntPtr handle = getShellTray();
            if (handle == (IntPtr)0) return;

            IntPtr res = SendMessage(handle, 0x0111, (IntPtr)419, (IntPtr)0);
        }

        private IntPtr getShellTray()
        {
            IntPtr handle = FindWindow("Shell_TrayWnd", null);
            if (handle == (IntPtr)0)
            {
                MessageBox.Show("Unable to find the Shell_Tray Handle", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (IntPtr)0;
            }
            return handle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Avoid opening when is already open
            string name = Assembly.GetExecutingAssembly().GetName().Name;
            List<Process> processes = Process.GetProcesses().Where(p => p.ProcessName.Contains(name)).ToList();
            if (processes.Count > 1)
                this.Close();

            // Ensure shell tray exists
            IntPtr handle = getShellTray();
            if (handle == (IntPtr)0) this.Close();

            // Load Settings
            LoadSettings();
        }

        public void LoadSettings()
        {
            // Load Settings
            string settingsPath = Status.GetSettingsFilePath();
            if (File.Exists(settingsPath))
            {
                string fData = File.ReadAllText(settingsPath);
                if (!int.TryParse(fData, out Status.TriggerSecs))
                {
                    MessageBox.Show("Error loading settings file");
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openSettings()
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSettings();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openSettings();
        }
    }
}
