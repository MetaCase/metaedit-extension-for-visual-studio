using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Subclass that enables to connect the MetaEdit+ API server with
    /// Url parameter and setting timeout.
    /// </summary>
    public class MEAPI : MetaEditAPI.MetaEditAPI
    {
        public MEAPI() 
            : base()
        { }

        public MEAPI(string Url)
            : base()
        {
            this.Url = Url;
        }

        public MEAPI(string Url, int _timeout) 
            : base()
        {
            this.Url = Url;
            this.Timeout = _timeout;
        }

        public void _SetTimeOut(int _timeout)
        {
            this.Timeout = _timeout;
        }

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);

        /// <summary>
        /// Let other programs raise their window over Visual Studio,
        /// e.g. when MetaEdit+ opens a diagram or dialog we don't want it obscured.
        /// Only remains in force until the user clicks in a non-VS, non-MetaEdit+ window.
        /// </summary>
        public static void AllowSetForegroundWindow() 
        {
            AllowSetForegroundWindow(-1);
        }
    }
}
