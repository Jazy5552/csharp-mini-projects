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
using System.Threading;

namespace DirectorySync2
{
    public partial class Form1 : Form
    {
        static bool cancel = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //MASTER BUTTON
        {
            FolderBrowserDialog FBO = new FolderBrowserDialog();

            if (FBO.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FBO.SelectedPath;
            }
            /*Thread worker = new Thread(new ThreadStart(() =>
            {
                updateValidSync();
            }));
            worker.IsBackground = true;
            worker.Start();*/
        }

        private void button2_Click(object sender, EventArgs e) //SLAVE BUTTON
        {
            FolderBrowserDialog FBO = new FolderBrowserDialog();

            if (FBO.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = FBO.SelectedPath;
            }
            /*Thread worker = new Thread(new ThreadStart(() =>
            {
                updateValidSync();
            }));
            worker.IsBackground = true;
            worker.Start();*/
        }

        private void button3_Click(object sender, EventArgs e) //SYNCRONIZE BUTTON
        {
            string master = textBox1.Text;
            string slave = textBox2.Text;
            enableSync(false);
            if (!Directory.Exists(master) || !Directory.Exists(slave)) //Confusing logic... I know
            {
                clearListbox();
                writeLine("Error: Invalid master or slave address");
                return;
            }
            clearListbox();
            enableButtons(false);
            Thread worker = new Thread(new ThreadStart(() =>
            {
                synchronize(master, slave);
            }));
            worker.IsBackground = true;
            worker.Start();
        }

        private void button4_Click(object sender, EventArgs e) //VERSION/HELP BUTTON
        {
            MessageBox.Show("Program used to copy files from master to slave and remove files from slave that are not in the master directory; Effectively creating a sync between the directories.\n\nProgram made by Jazy. Open source/Free to take/Free to use in any diabolical way possible.\nContact: jazysapplepie@gmail.com\n\nDisclaimer: In case of any damage/injury/death caused by any of my programs keep in mind that I don't exist and never did. Otherwise, credit is always appreciated but not required.\nEnjoy.", "Directory Synchronizer");
            /*Thread worker = new Thread(new ThreadStart(() =>
            {
                updateValidSync();
            }));
            worker.IsBackground = true;
            worker.Start();*/
        }

        private void button5_Click(object sender, EventArgs e) //PREPARE FOR SYNC BUTTON
        {
            enableButtons(false);
            enableSync(false);
            Thread worker = new Thread(new ThreadStart(() =>
            {
                updateValidSync();
            }));
            worker.IsBackground = true;
            worker.Start();
        }

        private void button6_Click(object sender, EventArgs e) //CANCEL BUTTON
        {
            //DONE Add a cancel button
            cancel = true;
        }

        //Oh god these global variables are getting out of hand! I've run out of ideas! //TODO Fix this shit!
        //private string prevMaster = "";
        //private string prevSlave = "";
        private void updateValidSync() //Do some prep work. Add the anticipated actions to the list box and calculate dirsizes
        {
            string master = textBox1.Text;
            string slave = textBox2.Text;
            if (!Directory.Exists(master) || !Directory.Exists(slave)) //Confusing logic... I know
            {
                writeLine("Invalid master or slave directories!");
                enableSync(false);
                enableButtons(true);
                return;
            }
            enableCancel(true, "Skip");
            Properties.Settings.Default["savedMaster"] = master;
            Properties.Settings.Default["savedSlave"] = slave;
            Properties.Settings.Default.Save();
            clearListbox();
            status("Loading differences...");
            System.Diagnostics.Debug.WriteLine("Started: " + label2.Text);
            if (label2.Text.EndsWith(".")) //Check it isn't still calculating another directory
            {
                DirectorySize.DirSize.GetDirSizeAsync(master, masterdirectorySizeCallback, master);
            }
            if (label3.Text.EndsWith(".")) //Check it isn't still calculating another directory
            {
                DirectorySize.DirSize.GetDirSizeAsync(slave, slavedirectorySizeCallback, slave);
            }
            updateRecursively(master, slave); //Document files to be copied
            updateRecursivelyS(slave, master); //Document files to be removed
            cancel = false;
            enableCancel(false, "");
            enableSync(true);
            enableButtons(true);
            status("Ready to sync!");
        }

        private void masterdirectorySizeCallback(long dirsize, bool done, Object masterPath, Thread workerThread) //Callback from DirSize to update the dirsize counter next to the textboxes
        {
            if (workerThread == null)
            {
                //Shits dead dude! Pack yo shit and abandon ship
                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = "Dir Size.";
                });
                return;
            }

            string master = (string)masterPath;
            if (master != textBox1.Text)
            {
                workerThread.Abort(); //Ewww Disgusting multithreading....
                return;
            }

            lock (masterPath)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        label2.Text = bytestostring(dirsize);
                        if (done)
                        {
                            label2.Text = label2.Text + ".";
                        }
                    });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
        }
        private void slavedirectorySizeCallback(long dirsize, bool done, Object slavePath, Thread workerThread) //Callback from DirSize to update the dirsize counter next to the textboxes
        {
            if (workerThread == null)
            {
                //Shits dead dude! Pack yo shit and abandon ship
                this.Invoke((MethodInvoker)delegate
                {
                    label3.Text = "Dir Size.";
                });
            }

            string slave = (string)slavePath;
            if (slave != textBox2.Text)
            {
                workerThread.Interrupt();
                return;
            }
            lock (slavePath)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        label3.Text = bytestostring(dirsize);
                        if (done)
                        {
                            label3.Text = label3.Text + ".";
                        }
                    });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
        }

        private void updateRecursively(string master, string slave) //Searches for files to be copied to slave and documents them in the listbox
        {
            if (Directory.Exists(master))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Mdir in Mdirs)
                {
                    hold = false;
                    foreach (string Sdir in Sdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            updateRecursively(Mdir, Sdir);
                        }
                    }
                    if (!hold)
                    {
                        writeLine("d " + Mdir);
                    }
                    if (cancel)
                        return; //Job canceled!
                }

                foreach (string file in check_FilesToCopy(master, slave))
                {
                    writeLine(file);
                }

            }
        }
        private void updateRecursivelyS(string slave, string master) //Same as above but for removal from slave files
        {
            if (Directory.Exists(slave))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Sdir in Sdirs)
                {
                    hold = false;
                    foreach (string Mdir in Mdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            updateRecursivelyS(Sdir, Mdir);
                        }
                    }
                    if (!hold)
                    {
                        writeLine("-d- " + Sdir);
                    }
                    if (cancel)
                        return; //Job canceled!
                }

                foreach (string file in check_FilesToRemove(slave, master))
                {
                    writeLine("--" + file);
                }
            }
        }

        private void synchronize(string master, string slave) //Contains the steps to comence and carry out the syncronization process
        {
            //DONE Show progress USING THE BYTES INSTEAD OF THE SHIT IM DOING << I'm an idiot 
            //TODO The bytes search are taking too long. Think of better way of showing progress. (Maybe using dir count)
            enableCancel(true, "Cancel Sync");
            clearListbox();
            status("Prepping things up...");
            /*
            long BytesToBeRemoved = calculateBytesToBeRemoved(slave, master);
            long BytesToBeCopied = calculateBytesToBeCopied(master, slave);
            setProgress(0, 0, (int)((BytesToBeRemoved + BytesToBeCopied) / 1000));
            */
            //Would have to get ALL THE SUB DIRECTORIES! Seems more optimized
            int dirs = countDirectories(master) + countDirectories(slave);
            if (dirs == 0)
                dirs = 1;
            setProgress(0, 0, dirs);
            
            writeLine("Synchronizing...");
            status("Removing files from slave...");
            syncRecursivelyS(slave, master); //Remove files from slave first
            status("Copying master files to slave...");
            syncRecursively(master, slave); //Now copy the ones from master
            writeLine("Completed!");
            if (!cancel)
            {
                status("Directories are now identical");
                setProgress(100, 0, 100);
            }
            else
            {
                setProgress(0, 0, 100);
                status("Sync Canceled!");
            }
            cancel = false;
            enableSync(true);
            enableButtons(true);
            enableCancel(false, "");
        }

        private void syncRecursively(string master, string slave) //Search for files to be copied
        {
            if (Directory.Exists(master) && Directory.Exists(slave))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Mdir in Mdirs)
                {
                    hold = false;
                    foreach (string Sdir in Sdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            syncRecursively(Mdir, Sdir);
                        }
                    }
                    if (!hold)
                    {
                        try
                        {
                            //If the same master dir is not found in the slave then copy it over
                            copyDirectory(Mdir, slave + Path.DirectorySeparatorChar + Path.GetFileName(Mdir));
                            writeLine("d Created:" + slave + Path.DirectorySeparatorChar + Path.GetFileName(Mdir));
                            //progress((int)(DirectorySize.DirSize.GetDirSize(Mdir) / 1000));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            writeLine("ERROR creating directory: " + slave + Path.DirectorySeparatorChar + Path.GetFileName(master) + " == " + ex.Message);
                        }
                    }
                    progress(1);
                    if (cancel)
                        return; //Job canceled!
                }

                foreach (string file in check_FilesToCopy(master, slave))
                {
                    try
                    {
                        File.Copy(file, slave + Path.DirectorySeparatorChar + Path.GetFileName(file));
                        //progress((int)(new FileInfo(file).Length / 1000)); //Will add to bar value
                        writeLine("Created:" + slave + Path.DirectorySeparatorChar + Path.GetFileName(file));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        writeLine("Error copying: " + file + " == " + ex.Message);
                    }
                    if (cancel)
                        return; //Job canceled!
                }
            }
        } 
        private void syncRecursivelyS(string slave, string master) //Same as above but for removal from slave files
        {
            if (Directory.Exists(master) && Directory.Exists(slave))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string file in check_FilesToRemove(slave, master))
                {
                    try
                    {
                        //progress((int)(new FileInfo(file).Length / 1000)); //Will add to bar value
                        File.Delete(file);
                        writeLine("Removed:" + file);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        writeLine("Error deleting file: " + file + " == " + ex.Message);
                    }
                    if (cancel)
                        return; //Job canceled!
                }
                //Delete files first in order to empty directories for deletion

                foreach (string Sdir in Sdirs)
                {
                    hold = false;
                    foreach (string Mdir in Mdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            syncRecursivelyS(Sdir, Mdir);
                        }
                    }
                    if (!hold)
                    {
                        try
                        {
                            //progress((int)(DirectorySize.DirSize.GetDirSize(Sdir) / 1000));
                            deleteDirectory(Sdir);
                            writeLine("d Removed:" + Sdir);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            writeLine("Error deleting directory: " + Sdir + " == " + ex.Message);
                        }
                    }
                    progress(1);
                    if (cancel)
                        return; //Job canceled!
                }
            }
        }

        /*private long calculateBytesToBeCopied(string master, string slave) //Calculate bytes to be copied
        {
            long totalbytes = 0;
            if (Directory.Exists(master) && Directory.Exists(slave))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Mdir in Mdirs)
                {
                    hold = false;
                    long MdirSize = DirectorySize.DirSize.GetDirSize(Mdir);
                    foreach (string Sdir in Sdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir) && DirectorySize.DirSize.GetDirSize(Sdir) == MdirSize)
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            totalbytes += calculateBytesToBeCopied(Mdir, Sdir);
                        }
                    }
                    if (!hold)
                    {
                        //Calculate size of directory to copy
                        totalbytes += MdirSize;
                    }
                }

                foreach (string file in check_FilesToCopy(master, slave))
                {
                    totalbytes += new FileInfo(file).Length;
                }

            }
            return totalbytes;
        }*/
        /*private long calculateBytesToBeRemoved(string slave, string master) //Calculate bytes that will be removed for progress bar
        {
            long totalbytes = 0;
            if (Directory.Exists(master) && Directory.Exists(slave))
            {
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string file in check_FilesToRemove(slave, master))
                {
                    totalbytes += new FileInfo(file).Length;
                }

                foreach (string Sdir in Sdirs)
                {
                    hold = false;
                    long SdirSize = DirectorySize.DirSize.GetDirSize(Sdir); //Optimization
                    foreach (string Mdir in Mdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir) && DirectorySize.DirSize.GetDirSize(Mdir) == SdirSize)
                        {
                            hold = true;
                            //When it finds a dir that exists both in master and slave then let it dig deeeeper
                            totalbytes += calculateBytesToBeRemoved(Sdir, Mdir);
                        }
                    }
                    if (!hold)
                    {
                        totalbytes += SdirSize;
                    }
                }
            }
            return totalbytes;
        }*/

        private void deleteDirectory(string path)//Recursive Delete
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string dir in dirs)
                deleteDirectory(dir);
            foreach (string file in files)
                File.Delete(file);

            Directory.Delete(path);
        }
        private void copyDirectory(string source, string dest)//Recursive Copy
        {
            if (!Directory.Exists(source)) //If no source then gtfo
                return;
            if (!Directory.Exists(dest)) //If no dest then make it!
            {
                try
                {
                    Directory.CreateDirectory(dest);
                    System.Diagnostics.Debug.WriteLine("Created: " + dest);
                }
                catch (Exception ex)
                {
                    writeLine("Error creating directory: " + dest + " == " + ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            foreach (string dir in Directory.GetDirectories(source))
            {
                copyDirectory(source + Path.DirectorySeparatorChar + Path.GetFileName(dir), dest + Path.DirectorySeparatorChar + Path.GetFileName(dir));
            }

            foreach (string file in Directory.GetFiles(source))
            {
                try
                {
                    File.Copy(file, dest + Path.DirectorySeparatorChar + Path.GetFileName(file));
                }
                catch (Exception ex)
                {
                    writeLine("Error copying: " + file + " == " + ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

            }
        }

        private string bytestostring(long bytes) //Convertes a number of bytes to KB/MB/GB
        {
            string denote;
            double size;
            if (bytes / 1000000000 > 0) //GBs
            {
                denote = " GB";
                size = (double)bytes / 1000000000;
            }
            else if (bytes / 1000000 > 0) //MBs
            {
                denote = " MB";
                size = (double)bytes / 1000000;
            }
            else if (bytes / 1000 > 0) //KBs
            {
                denote = " KB";
                size = (double)bytes / 1000;
            }
            else
            {
                denote = " Bytes";
                size = bytes;
            }
            return string.Format("{0:0.##}" + denote, size);
        }

        /*private List<string> check_DirsToCopy(string master, string slave)
        {
            List<string> dirsToCopy = null;
            if (Directory.Exists(master))
            {
                dirsToCopy = new List<string>();
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Mdir in Mdirs)
                {
                    hold = false;
                    foreach (string Sdir in Sdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                            hold = true;
                    }
                    if (!hold)
                        dirsToCopy.Add(Mdir); //Dir to be added to slave
                }
            }
            return dirsToCopy;
        }*/
        private List<string> check_FilesToCopy(string master, string slave) //Runs through master and checks for files that arn't in the slave
        {
            List<string> filesToCopy = null;
            if (Directory.Exists(master))
            {
                filesToCopy = new List<string>();
                bool hold;
                string[] Mfiles = Directory.GetFiles(master);
                string[] Sfiles = Directory.GetFiles(slave);

                foreach (string Mfile in Mfiles)
                {
                    hold = false;
                    foreach (string Sfile in Sfiles)
                    {
                        if (Path.GetFileName(Mfile) == Path.GetFileName(Sfile))
                        {
                            long Sf = new FileInfo(Sfile).Length;
                            long Mf = new FileInfo(Mfile).Length;
                            if (Sf == Mf) //Check the file sizes as well
                            {
                                hold = true; //The file was found in the slave. Dont copy
                            }
                        }
                    }
                    if (!hold)
                        filesToCopy.Add(Mfile); //File to be added to slave
                }
            }
            return filesToCopy;
        }
        /*private List<string> check_DirsToRemove(string slave, string master)
        {
            List<string> dirsToRemove = null;
            if (Directory.Exists(slave))
            {
                dirsToRemove = new List<string>();
                bool hold;
                string[] Mdirs = Directory.GetDirectories(master);
                string[] Sdirs = Directory.GetDirectories(slave);

                foreach (string Sdir in Sdirs)
                {
                    hold = false;
                    foreach (string Mdir in Mdirs)
                    {
                        if (Path.GetFileName(Mdir) == Path.GetFileName(Sdir))
                            hold = true;
                    }
                    if (!hold)
                        dirsToRemove.Add(Sdir); //Dir to be removed from slave
                }
            }
            return dirsToRemove;
        }*/
        private List<string> check_FilesToRemove(string slave, string master) //Runs through the slave and checks for files that arn't in the master
        {
            List<string> filesToRemove = null;
            if (Directory.Exists(slave))
            {
                filesToRemove = new List<string>();
                bool hold;
                string[] Mfiles = Directory.GetFiles(master);
                string[] Sfiles = Directory.GetFiles(slave);

                foreach (string Sfile in Sfiles)
                {
                    hold = false;
                    foreach (string Mfile in Mfiles)
                    {
                        if (Path.GetFileName(Mfile) == Path.GetFileName(Sfile))
                        {
                            long Sf = new FileInfo(Sfile).Length;
                            long Mf = new FileInfo(Mfile).Length;
                            if (Sf == Mf) //Check the file sizes as well
                            {
                                hold = true; //The file was found in the master. No need to remove from slave.
                            }
                        }
                    }
                    if (!hold)
                        filesToRemove.Add(Sfile); //File to be removed from slave
                }
            }
            return filesToRemove;
        }

        private Object lok = new Object(); //Object used to create sync when accessing the form thread
        private void writeLine(string msg) //Adds an item to the listbox
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    listBox1.Items.Add(msg);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    listBox1.SelectedIndex = -1;
                });
            }
        }
        private void clearListbox()
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    listBox1.Items.Clear();
                });
            }
        }
        private void enableSync(bool a) //enables/disables the sync button
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    button3.Enabled = a;
                });
            }
        }
        private void enableButtons(bool a) //enables/disables most buttons
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    button1.Enabled = a;
                    button2.Enabled = a;
                    button5.Enabled = a;
                    textBox1.Enabled = a;
                    textBox2.Enabled = a;
                });
            }
        }
        private void enableCancel(bool a, string text) //sets and enables/disables the cancel button
        {
            this.Invoke((MethodInvoker)delegate
            {
                button6.Enabled = a;
                button6.Text = text;
            });
        }
        private void status(string msg) //changes the tool strip status text
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabel1.Text = msg;
                });
            }
        }
        private void progress(int p) //increments the value of the progress bar by p
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (p + toolStripProgressBar1.Value > toolStripProgressBar1.Maximum)
                    {
                        System.Diagnostics.Debug.WriteLine("Progress over max.");
                        return;
                    }
                    toolStripProgressBar1.Value += p;
                });
            }
        }
        private void setProgress(int p, int min, int max) //set the progress value/min/max
        {
            lock (lok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (p > toolStripProgressBar1.Maximum || p < toolStripProgressBar1.Minimum)
                    {
                        System.Diagnostics.Debug.WriteLine("Progress over max.");
                        return;
                    }
                    toolStripProgressBar1.Maximum = max;
                    toolStripProgressBar1.Minimum = min;
                    toolStripProgressBar1.Value = p;
                });
            }
        }
        private int countDirectories(string dir) //Counts the number of directories
        {
            int count = 0;
            if (Directory.Exists(dir) && !cancel)
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (string d in dirs)
                {
                    count += countDirectories(d);
                }
                count += dirs.Length;
            }
            return count;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Loads saved master and slave directories
            textBox1.Text = (string)Properties.Settings.Default["savedMaster"];
            textBox2.Text = (string)Properties.Settings.Default["savedSlave"];
        }
    }
}
