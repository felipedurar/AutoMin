using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMin
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Status.TriggerSecs;
            checkBox1.Checked = Status.Running;

            Status.Running = false;
            Status.SettingsChanged = true;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Status.Running = checkBox1.Checked;
            Status.SettingsChanged = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string settingsPath = Status.GetSettingsFilePath();
                if (File.Exists(settingsPath))
                    File.Delete(settingsPath);

                File.WriteAllText(settingsPath, ((int)numericUpDown1.Value).ToString());

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred saving the settings file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
