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
        private static MetaEditAPIPortTypeClient _port;
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
        public static MetaEditAPIPortTypeClient Port 
        {
            get
            {
                if (_port == null)
                {
                    // Create the binding.
                    BasicHttpBinding _binding = new BasicHttpBinding();
                    _binding.Name = "MetaEditAPISoapBinding";
                    // Timespan for timeouts.
                    TimeSpan TimeOutSpan = new TimeSpan(0,1,0);
                    _binding.CloseTimeout = TimeOutSpan;
                    _binding.OpenTimeout = TimeOutSpan;
                    _binding.ReceiveTimeout = TimeOutSpan;
                    _binding.SendTimeout = TimeOutSpan;
                    _binding.AllowCookies = false;
                    _binding.BypassProxyOnLocal = true;
                    _binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                    _binding.MaxBufferSize = 65536; 
                    _binding.MaxBufferPoolSize = 524288;
                    _binding.MaxReceivedMessageSize = 65536;
                    _binding.MessageEncoding = WSMessageEncoding.Text;
                    _binding.TextEncoding = Encoding.UTF8;
                    _binding.TransferMode = TransferMode.Buffered;
                    _binding.UseDefaultWebProxy = true;
                    
                    // Create the EndpointAddress
                    EndpointAddress _address = new EndpointAddress("http://" + settings.Host + ":" + settings.Port + "/MetaEditAPI");
                    // Connect to MetaEdit+
                    _port = new MetaEditAPIPortTypeClient(_binding, _address);
                    
                }              
                return _port;
            }
        }

        /**
         * Launcher method for doing initialization launch.
         */
        public static bool DoInitialLaunch()
        {
            return initializeAPI();
        }

        /**
         * Initializes API connection by checking if it's available and asking user
         * if MetaEdit+ should be launched. 
         * @return true if ME+ launched successfully else false.
         */
        public static Boolean initializeAPI()
        {
            if (!IsApiOK())
            {
                int maxWaitMs = 500;
                if (launchMetaEdit())
                {
                    maxWaitMs = 2500;
                    poll(maxWaitMs);
                }
            }
            return isInitialized;
        }

        /**
         * Polls MetaEdit+ until connection is OK or the time is up.
         * @param maxWaitMs maximum wait time in milliseconds.
         */
        public static void poll(int maxWaitMs)
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
            METype metype = new METype();
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
                System.Diagnostics.ProcessStartInfo stratInfo = new System.Diagnostics.ProcessStartInfo();
                stratInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                stratInfo.FileName = settings.ProgramDir;
                stratInfo.Arguments = arguments;
                process.StartInfo = stratInfo;
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
                MENull menull = new MENull();
                Port.stopAPI(menull);
                needStopAPI = false;
            }
        }   
    }
}
