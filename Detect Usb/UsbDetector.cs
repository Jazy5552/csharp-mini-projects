using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace Detect_Usb
{
    public class UsbDetector
    {

        public static void AddRemoveUSBHandler(EventArrivedEventHandler usb)
        {
            ManagementEventWatcher w = null;
            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;
            try
            {
                q = new WqlEventQuery();
                q.EventClassName = "__InstanceDeletionEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += usb;

                w.Start();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                if (w != null)
                {
                    w.Stop();
                }
            }

        }

        public static void AddInsertUSBHandler(EventArrivedEventHandler usb)
        {
            ManagementEventWatcher w = null;
            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;
            try
            {
                q = new WqlEventQuery();
                q.EventClassName = "__InstanceCreationEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += usb;

                w.Start();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                if (w != null)
                {
                    w.Stop();
                }
            }

        }
    }
}
