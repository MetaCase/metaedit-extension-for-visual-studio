using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;

namespace MetaCase.GraphBrowser
{
    public class Launcher
    {
        private static MEAPI _port;
        private static Boolean needStopAPI      = false;
        private static Boolean isInitialized    = false;        
        public static bool connectionAlive      { get; set; }
        public static Settings settings 
        {
            get 
            { 
                return Settings.GetSettings(); 
            } 
        }

        // Read only property Port handles the connection to MetaEdit+.
        public static MEAPI Port 
        {
            get
            {
                if (_port == null)
                {
                    _port = new MEAPI("http://" + settings.Host + ":" + settings.Port + "/MetaEditAPI"); 
                    //_port.SetTimeOut(100);
                }              
                return _port;
            }
        }

        ///<summary>
        /// Launcher method for doing initialization launch.
        ///</summary>
        public static bool DoInitialLaunch()
        {
            if (Settings.GetSettings().CheckIfMerExists() || IsApiOK())
            {
                return initializeAPI(true);
            }
            else
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }
            return IsApiOK();
        }

        ///<summary>
        /// Initializes API connection by checking if it's available and asking user
        /// if MetaEdit+ should be launched. 
        ///</summary>
        ///<returns>
        /// true if ME+ launched successfully.
        ///</returns>
        public static Boolean initializeAPI(Boolean poll)
        {
            if (!IsApiOK())
            {
                int maxWaitMs = 500;
                if (launchMetaEdit())
                {
                    if (poll)
                    {
                        maxWaitMs = 2500;
                        Poll(maxWaitMs);
                    }
                }
            }
            return isInitialized;
        }

        /// <summary>
        /// Polls MetaEdit+ until connection is OK or the time is up.
        /// </summary>
        /// <returns>maxWaitMs maximum wait time in milliseconds.</returns>
        public static void Poll(int maxWaitMs)
        {
            int totalWaitMs = 0;
            int waitMs = 500;
            connectionAlive = true;
            while (!IsApiOK() && ((totalWaitMs += waitMs) <= maxWaitMs))
            {
                Thread.Sleep(waitMs);
            }
        }

        /// <summary>
        /// Method for checking if API is running. 
        /// Simply calls MetaEdit+ and check if it answers.
        /// If no API is connected just return without checking anything until user clicks
        /// "update" or "start metaedit+"
        /// </summary>
        /// <returns>true if API is ok, otherwise false.</returns>
        public static Boolean IsApiOK()
        {
            // Return if known that there is no connection alive.
            if (!connectionAlive) return connectionAlive;

            string name;
            MetaEditAPI.METype metype = new MetaEditAPI.METype();
            metype.name = "Graph";
            try
            {            
                name = Port.typeName(metype);
            }
            catch (System.Net.WebException e)
            {
                if (e.Status == System.Net.WebExceptionStatus.ConnectFailure)
                {
                    connectionAlive = false;
                }
                else if (e.Status == System.Net.WebExceptionStatus.Timeout)
                {
                    connectionAlive = true;
                }
                name = "";
            }
            isInitialized = name.Equals("Graph");
            return isInitialized;
        }

        /// <summary>
        /// Lauches MetaEdit+ logs in opens one or more projects and starts API.
        /// </summary>
        /// <returns>True if launching succeeded.</returns>
        public static Boolean launchMetaEdit()
        {
            // Create the arguments
            string arguments = " currentDir: " + '"' + settings.WorkingDir + '"' + " " + "loginDB:user:password: " + settings.Database + " " +
            settings.Username + " " + settings.Password;
            string[] projects = settings.Projects;
			foreach (string project in projects) {
				if (!project.Equals("")) {
                    arguments += " setProject: " + '"' + project + '"';
				}
			}
            arguments += " startAPIHostname:port:logEvents: " + settings.Host + " " + settings.Port + " " + settings.Logging;
            
            //Then launch MetaEdit+
            try
            {
                needStopAPI = true;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = settings.ProgramPath;
                startInfo.Arguments = arguments;
                process.StartInfo = startInfo;
                process.Start();

                return true;
            }
            catch (Exception e)
            {
                DialogProvider.ShowMessageDialog("Could not start MetaEdit+: " + e.Message + "\nPlease start MetaEdit+ API and click OK to proceed", "Launch error");
                return false;
            }
        }

        /// <summary>
        /// Stops API.
        /// </summary>
        public static void stopApi()
        {
            if (needStopAPI)
            {
                MetaEditAPI.MENull menull = new MetaEditAPI.MENull();
                Port.stopAPI(menull);
                needStopAPI = false;
            }
        }   
    }
}
