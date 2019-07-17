using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace EncryptDiary
{
    public partial class Form1 : Form
    {
        private bool textChange = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textChange && MessageBox.Show("Save changes before exit?", "Save changes?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                saveToolStripMenuItem_Click(this, null);
            }
            textChange = false;
            SetTextboxText("");
            Status("");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            string path = "";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
            }
            if (File.Exists(path))
            {
                Open(path);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(toolStripStatusLabel1.Text))
            {
                Save(toolStripStatusLabel1.Text);
            }
            else
            {
                SaveAs();
            }
        }

        private void Open(string path)
        {
            try
            {
                string holder = File.ReadAllText(path, Encoding.ASCII);

                textBox1.Text = "";
                SetTextboxText(Encrypter.Encrypter.Decrypt(holder, GetKey()));
                Status(path);
                textChange = false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                Status("Error: " + e.Message);
            }
        }

        private void Save(string path)
        {
            try
            {
                string holder = textBox1.Text;
                
                File.WriteAllText(path, Encrypter.Encrypter.Encrypt(holder, GetKey()));
                Status(path);
                textChange = false;
            }
            catch (Exception e)
            {
                Status(e.Message);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Save(sfd.FileName);
            }
        }

        private void Status(string stat)
        {
            toolStripStatusLabel1.Text = stat;
        }

        private void SetTextboxText(string text)
        {
            textBox1.AppendText(text);
        }

        private int GetKey()
        {
            return Encrypter.Encrypter.StringtoInt(textBox2.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textChange = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Font = new Font(FontFamily.GenericSansSerif, 12);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Prompt to save and encrypt
            if (textChange && MessageBox.Show("Save changes before exit?", "Save changes?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                saveToolStripMenuItem_Click(this, null);
            }
        }

        private void appInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO App info/disclaimer
        }

        private void spellCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet...", "Coming soon", MessageBoxButtons.OK);
        }

        private void nightModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.BackColor == Color.Black)
            {
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;
            }
            else
            {
                //this.BackColor = Color.Black;
                //TODO Make inverted colors (Night mode)
                textBox1.BackColor = Color.Black;
                textBox1.ForeColor = Color.White;
            }
        }
    }
}
