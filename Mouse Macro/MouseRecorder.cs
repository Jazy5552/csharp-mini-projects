using System;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;

namespace Mouse_Macro
{
    class MouseRecorder
    {
        private bool recording = false;
        //TODO Make struct that will hold thread information

        public MouseRecorder()
        {
            
        }

        public void StartRecording()
        {
            lock(this)
            {
                if (!recording)
                {
                    recording = true;
                    record();
                }
            }
        }

        private void record()
        {
            Thread worker = new Thread(new ThreadStart(() =>
            {
                //TODO record mouse and check for recording. When done call callback
            }));
        }
    }
}
