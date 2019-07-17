using System;

//TODO make antitheft work with my server
//NOTE: App will check server to see if it should delete itself port 8555
//Server delete response: jazyisawesome
//Server ok response: applepie
//Make the jazy.ia record last successful time of change

namespace Quick_Join_KCISCisco_RESTART
{
    public class KCISCiscoScript
    {
        public static void Main(string[] args)
        {
            KCISCiscoScript script = new KCISCiscoScript();
            script.AntiTheft();
        }
        public KCISCiscoScript()
        {
            /// Do some house work
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string sciprtName = "Quick_Join_KCISCisco.Resources.hi.bat";
            string scriptPath = @"C:\Windows\Temp\hi.bat";

            /// Copy the script to its intended location
            using (System.IO.Stream scriptResource = assembly.GetManifestResourceStream(sciprtName))
            using (System.IO.Stream outputDest = System.IO.File.Create(scriptPath))
            {
                //Using ensures that the objects are disposed (As opposed to making a long try{}catch{}finally{} statement)
                byte[] buffer = new byte[2048];
                int bytesread = 0;
                while ((bytesread = scriptResource.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //Write the script to its destination
                    outputDest.Write(buffer, 0, bytesread);
                }
            }

            /// Run the script
            try
            {
                System.Diagnostics.Process.Start(scriptPath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void AntiTheft()
        {
            ///Anti theft mechanism
            if (!System.IO.File.Exists("jazy.ia"))
            {
                //Uh oh Gotta delete this bad boy
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                try
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd", "/c \"timeout 1 & del /F /Q \"" + appPath + "\"\"");
                    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process.Start(psi);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                //Make sure it's hidden ;)
                try
                {
                    System.IO.File.SetAttributes("jazy.ia", System.IO.FileAttributes.Hidden);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
        }


    }
}
