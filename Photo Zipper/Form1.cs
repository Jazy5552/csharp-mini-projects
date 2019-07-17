using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_Zipper
{
    public partial class Form1 : Form
    {
        Photos pics;
        long compressionLevel;
        BackgroundWorker bgw2 = new BackgroundWorker(); //For thumbnails

        public Form1()
        {
            pics = null;
            compressionLevel = 40;
            InitializeComponent();
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Select files
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Select zip file
            saveFileDialog1.FileName = DateTime.Now.ToString("M-dd-yy") + "-Photos.zip";
            saveFileDialog1.ShowDialog();
        }

        private async Task returnButtonText(String oriText)
        {
            await Task.Delay(5000);
            button2.Text = oriText;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //Select the photos
            pics = new Photos(openFileDialog1.FileNames);
            //Enable zip saving and change first button text
            button1.Text = "Reselect Photos";
            button2.Enabled = true;
            //Show number of selected photos USING FIXED TEXT!!
            label1.Text = "Photos Selected: " + pics.GetPhotoPaths().Count;
            //Fill grid view BACKGROUND
            bgw2.WorkerSupportsCancellation = true;
            bgw2.CancelAsync();
            bgw2.DoWork += workerDoWork;
            bgw2.RunWorkerAsync();
            /*//Fill list box BACKGROUND
            textBox1.Text = "";
            bgw.DoWork += delegate
            {
                foreach (String file in pics.GetPhotoPaths())
                {
                    textBox1.Invoke(new MethodInvoker( () =>
                    {
                        textBox1.Text += file + Environment.NewLine;
                    }));
                }
            };
            */
            //Estimate file size BACKGROUND
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                long expectedSize = pics.GetExpectedTotalSize(compressionLevel) / 1000;
                long realSize = pics.GetRealTotalSize() / 1000;
                this.Invoke(new MethodInvoker(() =>
                {
                    toolStripStatusLabel1.Text = realSize + " KB => " + expectedSize + " KB";
                    if (expectedSize > 25000)
                    {
                        //If over 25MB (Email limit) show warning WARN MAGIC NUMBER!
                        toolStripStatusLabel1.BackColor = Color.Red;
                        toolStripStatusLabel1.Text += "  Photos will be over the 25MB limit. Try increasing compression.";
                    }
                    else
                    {
                        toolStripStatusLabel1.BackColor = Color.White;
                    }
                }));
            };
            bgw.RunWorkerAsync();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //Select the filename of the zip file
            pics.ZipPhotos(saveFileDialog1.FileName, compressionLevel);
            //Show Done
            Task result = returnButtonText(button2.Text);
            button2.Text = "DONE";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            compressionLevel = (trackBar1.Value - 10) * -10; //Fuzzy logic
            System.Diagnostics.Debug.Print(compressionLevel + "");
            toolStripStatusLabel1.Text = "To calculate size difference reselect files";
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://" + toolStripStatusLabel2.Text);
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            //Populate listbox with thumbnails
            BackgroundWorker worker = sender as BackgroundWorker;
            panel1.Invoke(new MethodInvoker(() => { panel1.AutoScroll = true; panel1.Controls.Clear(); }));
            List<String> picLocs = pics.GetPhotoPaths();
            PictureBox[] pictures = new PictureBox[picLocs.Count];
            int y = 10;
            int x = 10;
            for (int index = 0; index < pictures.Length; index++)
            {
                if (worker.CancellationPending)
                    break;
                
                //Image positions
                if (index % 7 == 0 && index != 0)
                {
                    y = y + 170;
                    x = 10;
                }
                else if (index != 0)
                {
                    x = x + 140;
                }

                String l = picLocs[index];
                pictures[index] = new PictureBox();
                pictures[index].Location = new Point(x, y);
                pictures[index].SizeMode = PictureBoxSizeMode.Zoom;
                pictures[index].Size = new Size(120, 160);
                pictures[index].Image = new Bitmap(l);
                pictures[index].MouseHover += (s, ev) =>
                {
                    ToolTip tt = new ToolTip();
                    tt.SetToolTip(s as Control,l);
                };
                panel1.Invoke(new MethodInvoker(() => { panel1.Controls.Add(pictures[index]); }));
                System.Diagnostics.Debug.Print("Loaded " + picLocs[index]);
            }
        }
    }
}
