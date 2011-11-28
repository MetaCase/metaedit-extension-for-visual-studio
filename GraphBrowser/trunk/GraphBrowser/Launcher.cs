﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;

namespace MetaCase.GraphBrowser
{
    public class Launcher
    {
        private static MetaEditAPI.MetaEditAPI _port;
        private static Boolean needStopAPI = false;
        private static Boolean isInitialized = false;
        public static Settings settings 
        {
            get 
            { 
                return Settings.GetSettings(); 
            } 
        }

        // Read only property Port handles the connection to MetaEdit+.
        public static MetaEditAPI.MetaEditAPI Port 
        {
            get
            {
                if (_port == null)
                {
                    _port = new MetaEditAPI.MetaEditAPI("http://" + settings.Host + ":" + settings.Port + "/MetaEditAPI");
                }              
                return _port;
            }
        }

        /**
         * Launcher method for doing initialization launch.
         */
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

        /**
         * Initializes API connection by checking if it's available and asking user
         * if MetaEdit+ should be launched. 
         * @return true if ME+ launched successfully else false.
         */
        public static Boolean initializeAPI(Boolean poll)
        {
            if (!IsApiOK())
            {
                int maxWaitMs = 500;
                if (launchMetaEdit())
                {
                    if (poll) maxWaitMs = 2500;
                    Poll(maxWaitMs);
                }
            }
            return isInitialized;
        }

        /**
         * Polls MetaEdit+ until connection is OK or the time is up.
         * @param maxWaitMs maximum wait time in milliseconds.
         */
        public static void Poll(int maxWaitMs)
        {
            int totalWaitMs = 0;
            int waitMs = 500;
            while (!IsApiOK() && ((totalWaitMs += waitMs) <= maxWaitMs))
            {
                Thread.Sleep(waitMs);
            }
        }

        /**
         * Method for checking if API is running. 
         * Simple calls MetaEdit+ and check if it answers.
         * @return true if API is ok, otherwise false.
         */
        public static Boolean IsApiOK()
        {
            String name;
            MetaEditAPI.METype metype = new MetaEditAPI.METype();
            metype.name = "Graph";
            try
            {
                name = Port.typeName(metype);
            }
            catch (Exception)
            {
                name = "";
            }
            isInitialized = name.Equals("Graph");
            return isInitialized;
        }

        /// <summary>
        /// Lauches MetaEdit+ logs inn opens one or more projects and starts API.
        /// </summary>
        /// <returns>True if launching succeeded.</returns>
        public static Boolean launchMetaEdit()
        {
            // Create the arguments
            String arguments = " currentDir: " + '"' + settings.WorkingDir + '"' + " " + "loginDB:user:password: " + settings.Database + " " +
            settings.Username + " " + settings.Password;
            String[] projects = settings.Projects;
			foreach (String project in projects) {
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

        /**
         * Stops api.
         */
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
