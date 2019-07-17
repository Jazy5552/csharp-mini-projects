using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace WMITestConsolApplication
{

    class Program
    {
        static void Main(string[] args)
        {
            Detect_Usb.UsbDetector.AddInsertUSBHandler(USBInserted);
            Detect_Usb.UsbDetector.AddRemoveUSBHandler(USBRemoved);
            while (true)
            {
            }

        }

        static void USBInserted(object sender, EventArgs e)
        {
            EventArrivedEventArgs ea = (EventArrivedEventArgs)e;
            Console.WriteLine("A USB device inserted:" + ea.NewEvent.GetText(TextFormat.CimDtd20) + "\nTostring:" + ea.NewEvent.Properties.ToString()); ;

        }

        static void USBRemoved(object sender, EventArgs e)
        {

            Console.WriteLine("A USB device removed");

        }
    }

}