using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices; //For DLL import below
using System.Threading;


namespace AutoClicker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private volatile bool stopdat = false;

        KeyboardHook hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();
            // register the event that is fired after the key press.
            hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(AutoClicker.ModifierKeys.Control | AutoClicker.ModifierKeys.Shift, Keys.S);
            hook.RegisterHotKey(AutoClicker.ModifierKeys.Control | AutoClicker.ModifierKeys.Shift, Keys.W); //Hold down
            hook.RegisterHotKey(AutoClicker.ModifierKeys.Control | AutoClicker.ModifierKeys.Shift, Keys.Q); //Release
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            //Stop the clicker
            if (e.Key == Keys.S)
                stopdat = true;
            else if (e.Key == Keys.W)
            {
                if (leftRadio.Checked)
                    click(MOUSEEVENTF_LEFTDOWN);
                else
                    click(MOUSEEVENTF_RIGHTDOWN);
            }
            else if (e.Key == Keys.Q)
            {
                if (leftRadio.Checked)
                    click(MOUSEEVENTF_LEFTUP);
                else
                    click(MOUSEEVENTF_RIGHTUP);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (clicksBox.Text == "") return;
            else if (startButton.Text != "Start")
            {
                stopdat = true;
                return;
            }
            else if (stopdat)
                return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                startCountDown();
                long numOfClicks = 0;
                try
                {
                    numOfClicks = Convert.ToInt64(clicksBox.Text);
                }
                catch (FormatException nono)
                {
                    System.Diagnostics.Debug.WriteLine(nono.ToString());
                    this.Invoke(new MethodInvoker(() => { startButton.Text = "Start"; clicksBox.Text = "Error!"; }));
                }
                catch (Exception idgaf)
                {
                    System.Diagnostics.Debug.WriteLine(idgaf.ToString());
                }

                int mouseEvent;
                if (numOfClicks < 0) //Going to hold down the mouse button
                {
                    numOfClicks *= -1;
                    if (leftRadio.Checked)
                        mouseEvent = MOUSEEVENTF_LEFTDOWN;
                    else
                        mouseEvent = MOUSEEVENTF_RIGHTDOWN;


                    click(mouseEvent);
                    for (int i = 0; i < numOfClicks; i++)
                    {
                        if (stopdat)
                            break;
                        new System.Threading.ManualResetEvent(false).WaitOne(1000);
                        try
                        {
                            this.Invoke(new MethodInvoker(() =>
                            {
                                if (i + 1 < numOfClicks)
                                    startButton.Text = "Running for " + (numOfClicks - i) + " seconds.";
                            }));
                        }
                        catch (Exception closed)
                        {
                            System.Diagnostics.Debug.WriteLine(closed.ToString());
                            break;
                        }
                    }
                    if (leftRadio.Checked)
                        mouseEvent = MOUSEEVENTF_LEFTUP;
                    else
                        mouseEvent = MOUSEEVENTF_RIGHTUP;
                    click(mouseEvent);
                }
                else if (numOfClicks > 0) //Click that sucker however many times it takes!
                {
                    if (leftRadio.Checked)
                        mouseEvent = MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP;
                    else
                        mouseEvent = MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP;

                    for (int i = 0; i < numOfClicks; i++)
                    {
                        if (stopdat)
                            break;
                        try
                        {
                            this.Invoke(new MethodInvoker(() => { startButton.Text = "Clicking!! OMERGWAD " + i; }));
                        }
                        catch (Exception closed)
                        {
                            System.Diagnostics.Debug.WriteLine(closed.ToString());
                            break;
                        }
                        click(mouseEvent);
                    }
                }
                try
                {
                    this.Invoke(new MethodInvoker(() => { startButton.Text = "Start"; }));
                }
                catch (Exception closed)
                {
                    System.Diagnostics.Debug.WriteLine(closed.ToString());
                }
                stopdat = false;
            }));
            thread.IsBackground = true;
            if (!stopdat)
                thread.Start();
            else
            {
                stopdat = false;
                this.Invoke(new MethodInvoker(() => { startButton.Text = "Start"; }));
            }
        }

        private void click(int mouseEvent)
        {
            int mX = Cursor.Position.X;
            int mY = Cursor.Position.Y;
            mouse_event(mouseEvent, mX, mY, 0, 0);
        }

        private void clicksBox_TextChanged(object sender, EventArgs e)
        {
            if (clicksBox.Text == "0")
            {
                clicksBox.Text = "1";
            } //Heheh I'm so mean >:D
        }

        private void startCountDown()
        {
            for (int i = 5; i > 0; i--)
            {
                if (stopdat)
                    break;
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                        {
                            startButton.Text = "Starting in " + i + " seconds.";
                        }));
                }
                catch (Exception theyClosed)
                {
                    System.Diagnostics.Debug.WriteLine(theyClosed.ToString());
                    break;
                }
                new System.Threading.ManualResetEvent(false).WaitOne(1000);
            }
        }

        private void save()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "This program is used to click extermely fast a specific number of times or to hold down the respective mouse button " +
                "a specific amount of time. At any time you may hit CTRL + ALT + S simultaneously to stop" +
                " the program, or clicking the start button again will also reset the program. Also hit CTRL +" +
                " ALT + Q/W to release the downed mouse button or hold it down respectively.\n\nThis " +
                "program was made by Jazy and is open source. You may request the source from me if it is" +
                "not already available at jazysapplepie@gmail.com.\n\nI ironed out most of the bugs but the " +
                "is still pretty nasty so if you encounter any bugs I'd appreciate it if you contacted me about it. " +
                "Enjoy :D", "Auto Clicker by Jazy");
        }
    }
}
